using AmongUs.GameOptions;
using TOHE.Modules;
using TOHE.Roles.Crewmate;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using TOHE.Roles.Core;

namespace TOHE.Roles.Neutral;

internal class Godzilla : RoleBase
{
    private static Dictionary<byte, SystemTypes> RoomsToDestroy = new();
    private static Dictionary<byte, long> DestroyTimestamps = new();
    
    //===========================SETUP================================\\
    public override CustomRoles Role => CustomRoles.Godzilla;
    private const int Id = 35300;
    public override CustomRoles ThisRoleBase => CustomRoles.Shapeshifter;
    public override Custom_RoleType ThisRoleType => Custom_RoleType.NeutralChaos;
  
    //==================================================================\\

    public static OptionItem GodzillaCanKill;
    public static OptionItem DestroyCooldown;
    public static OptionItem GodzillaKillCD;
    public static OptionItem WarningTimeBeforeDestroying;

    public static bool InDestroy = false;
    public static SystemTypes ActiveRoom;
    private static long DestroyStartTime;

    public override void SetupCustomOption()
    {
        Options.SetupRoleOptions(Id, TabGroup.NeutralRoles, CustomRoles.Godzilla);
        GodzillaCanKill = BooleanOptionItem.Create(Id + 10, GeneralOption.CanKill, false, TabGroup.NeutralRoles, false)
            .SetParent(Options.CustomRoleSpawnChances[CustomRoles.Godzilla]);
        GodzillaKillCD = FloatOptionItem.Create(Id + 11, GeneralOption.KillCooldown, new(0f, 180f, 2.5f), 20f, TabGroup.NeutralRoles, false)
            .SetParent(GodzillaCanKill)
            .SetValueFormat(OptionFormat.Seconds);
        DestroyCooldown = FloatOptionItem.Create(Id + 12, "DestroyCooldown", new(5f, 180f, 2.5f), 20f, TabGroup.NeutralRoles, false)
            .SetParent(Options.CustomRoleSpawnChances[CustomRoles.Godzilla])
            .SetValueFormat(OptionFormat.Seconds);
        WarningTimeBeforeDestroying = FloatOptionItem.Create(Id + 13, "WarningTimeBeforeDestroying", new(1f, 10f, 1f), 5f, TabGroup.NeutralRoles, false)
            .SetParent(Options.CustomRoleSpawnChances[CustomRoles.Godzilla])
            .SetValueFormat(OptionFormat.Seconds);
    }

    public override bool CanUseKillButton(PlayerControl pc) => GodzillaCanKill.GetBool() && pc.IsAlive();
    
    public override void SetKillCooldown(byte id)
    {
        if (GodzillaCanKill.GetBool())
            Main.AllPlayerKillCooldown[id] = GodzillaKillCD.GetFloat();
        else
            Main.AllPlayerKillCooldown[id] = 300f;
    }

    public override void ApplyGameOptions(IGameOptions opt, byte playerId)
    {
        AURoleOptions.ShapeshifterCooldown = DestroyCooldown.GetFloat();
    }
    public override void OnFixedUpdate(PlayerControl player, bool lowLoad, long nowTime, int timerLowLoad)
    {
        var remainingTime = DestroyStartTime + (long)WarningTimeBeforeDestroying.GetFloat() - nowTime;
        if (!InDestroy) return;
        foreach (var target in Main.AllAlivePlayerControls)
        {
            var roomName = Translator.GetString(ActiveRoom.ToString());
            target.Notify(string.Format(Translator.GetString("GodzillaRoomWarning"), roomName, remainingTime));
        }
    }
    public override void UnShapeShiftButton(PlayerControl shapeshifter)
    {
        var playerRole = shapeshifter.GetCustomRole();
        Logger.Info("Godzilla is destroying a room", playerRole.ToString());

        // Get all ship rooms that exist on the current map
        var shipRoom = ShipStatus.Instance.AllRooms;
        var currentMapRooms = shipRoom.Select(room => room.RoomId).ToList();
        
        // Filter out HeliSabotage and rooms that don't exist on current map
        var validRooms = SystemTypeHelpers.AllTypes
            .Where(x => x != SystemTypes.HeliSabotage && currentMapRooms.Contains(x))
            .ToList();
            
        if (validRooms.Count == 0)
        {
            shapeshifter.Notify(Translator.GetString("NoValidRoomsToDestroy"));
            return;
        }

        var roomToDestroy = validRooms[IRandom.Instance.Next(0, validRooms.Count)];
        RoomsToDestroy[shapeshifter.PlayerId] = roomToDestroy;
        ActiveRoom = roomToDestroy;
        DestroyTimestamps[shapeshifter.PlayerId] = Utils.GetTimeStamp() + (long)WarningTimeBeforeDestroying.GetFloat();

        // Send warning to all players
        InDestroy = true;
        DestroyStartTime = Utils.GetTimeStamp();

        // Schedule the room destruction
        _ = new LateTask(() =>
        {
            InDestroy = false;
            DestroyRoom(shapeshifter, roomToDestroy);
            RoomsToDestroy.Remove(shapeshifter.PlayerId);
            DestroyTimestamps.Remove(shapeshifter.PlayerId);
        }, WarningTimeBeforeDestroying.GetFloat(), "Godzilla Room Destruction");
    }

    private void DestroyRoom(PlayerControl godzilla, SystemTypes room)
    {
        Logger.Info($"Destroying room: {room}", "Godzilla");
        CustomSoundsManager.RPCPlayCustomSoundAll("Boom");

        foreach (var target in Main.AllPlayerControls)
        {
            if (!target.IsAlive() || Medic.IsProtected(target.PlayerId) || target.inVent || 
                target.IsTransformedNeutralApocalypse() || target.Is(CustomRoles.Solsticer)) 
                continue;

            // Check if player is in the room
            if (target.GetPlainShipRoom()?.RoomId == room)
            {
                target.SetDeathReason(PlayerState.DeathReason.Destroyed);
                target.RpcMurderPlayer(target);
                target.SetRealKiller(godzilla);
            }
        }
    }

    public override void OnReportDeadBody(PlayerControl reporter, NetworkedPlayerInfo target)
    {
        RoomsToDestroy.Clear();
        DestroyTimestamps.Clear();
    }

    public override void SetAbilityButtonText(HudManager hud, byte playerId)
    {
        hud.AbilityButton.OverrideText(Translator.GetString("GodzillaShapeshiftText"));
    }
}
