using System.Text;
using UnityEngine;
using TOHE.Roles.Core;
using TOHE.Roles.AddOns;
using static TOHE.Options;
using static TOHE.Utils;
using static TOHE.Translator;

namespace TOHE.Roles.Crewmate;

internal class TaskManager : RoleBase
{
    //===========================SETUP================================\\
    public override CustomRoles Role => CustomRoles.TaskManager;
    private const int Id = 7200;
    public static bool HasEnabled => CustomRoleManager.HasEnabled(CustomRoles.TaskManager);
    public override CustomRoles ThisRoleBase => CustomRoles.Crewmate;
    public override Custom_RoleType ThisRoleType => Custom_RoleType.CrewmateBasic;
    //==================================================================\\

    private static OptionItem CanCompleteTaskAfterDeath;
    private static OptionItem CanGetHelpfulAddons;
    private static OptionItem CanGetHarmfulAddons;
    private static OptionItem CanGetMixedAddons;
    private static OptionItem CanSeeAllCompletedTasks;

    private static List<CustomRoles> Addons = [];
    private static readonly Dictionary<int, byte> Target = [];

    public override void SetupCustomOption()
    {
        SetupRoleOptions(Id, TabGroup.CrewmateRoles, CustomRoles.TaskManager);
        CanCompleteTaskAfterDeath = BooleanOptionItem.Create(Id + 2, "TaskManager_OptionCanCompleteTaskAfterDeath", false, TabGroup.CrewmateRoles, false)
            .SetParent(CustomRoleSpawnChances[CustomRoles.TaskManager]);
        CanGetHelpfulAddons = BooleanOptionItem.Create(Id + 3, "TaskManager_OptionCanGetHelpfulAddons", true, TabGroup.CrewmateRoles, false)
            .SetParent(CustomRoleSpawnChances[CustomRoles.TaskManager]);
        CanGetHarmfulAddons = BooleanOptionItem.Create(Id + 4, "TaskManager_OptionCanGetHarmfulAddons", false, TabGroup.CrewmateRoles, false)
            .SetParent(CustomRoleSpawnChances[CustomRoles.TaskManager]);
        CanGetMixedAddons = BooleanOptionItem.Create(Id + 5, "TaskManager_OptionCanGetMixedAddons", false, TabGroup.CrewmateRoles, false)
            .SetParent(CustomRoleSpawnChances[CustomRoles.TaskManager]);
        CanSeeAllCompletedTasks = BooleanOptionItem.Create(Id + 6, "TaskManager_OptionCanSeeAllCompletedTasks", false, TabGroup.CrewmateRoles, false)
            .SetParent(CustomRoleSpawnChances[CustomRoles.TaskManager]);
        OverrideTasksData.Create(Id + 10, TabGroup.CrewmateRoles, CustomRoles.TaskManager);
    }
    public override void Init()
    {
        Addons.Clear();
        Target.Clear();

        if (CanGetHelpfulAddons.GetBool())
        {
            Addons.AddRange(GroupedAddons[AddonTypes.Helpful]);
        }
        if (CanGetHarmfulAddons.GetBool())
        {
            Addons.AddRange(GroupedAddons[AddonTypes.Harmful]);
        }
        if (CanGetMixedAddons.GetBool())
        {
            Addons.AddRange(GroupedAddons[AddonTypes.Mixed]);
        }

        Addons = Addons.Where(role => role.GetMode() != 0).ToList();
    }
    public override bool OnTaskComplete(PlayerControl taskManager, int completedTaskCount, int totalTaskCount)
    {
        if (!taskManager.IsAlive() && !CanCompleteTaskAfterDeath.GetBool()) return true;

        List<NetworkedPlayerInfo.TaskInfo> allNotCompletedTasks = [];
        var randomPlayer = Main.AllAlivePlayerControls.Where(pc => pc.Is(Custom_Team.Crewmate) && Utils.HasTasks(pc.Data, false)).ToList().RandomElement();

        if (randomPlayer != null)
            allNotCompletedTasks = randomPlayer.Data.Tasks.ToArray().Where(pcTask => !pcTask.Complete).ToList();

        if (allNotCompletedTasks.Any())
        {
            Target[randomPlayer.PlayerId] = taskManager.PlayerId;
            randomPlayer.RpcCompleteTask(allNotCompletedTasks.RandomElement().Id);
            randomPlayer.Notify(GetString("TaskManager_CompletedRandomTaskForPlayer"));
        }
        else if (taskManager.IsAlive())
        {
            if (Addons.Count == 0)
            {
                taskManager.Notify(GetString("TaskManager_FailGetAddon"));
            }
            else
            {
                taskManager.RpcSetCustomRole(Addons.RandomElement());
                taskManager.Notify(GetString("TaskManager_YouGetAddon"));
            }
        }
        return true;
    }
    public static bool GetTaskManager(byte targetId, out byte taskManager)
    {
        taskManager = Target.GetValueOrDefault(targetId, byte.MaxValue);
        return taskManager != byte.MaxValue;
    }
    public static void ClearData(byte targetId)
    {
        Target[targetId] = byte.MaxValue;
    }
    public override string GetProgressText(byte playerId, bool comms)
    {
        if (!CanSeeAllCompletedTasks.GetBool()) return string.Empty;
    
        var ProgressText = new StringBuilder();
        var TextColor = GetRoleColor(CustomRoles.TaskManager);

        ProgressText.Append(GetTaskCount(playerId, comms));
        ProgressText.Append(ColorString(TextColor, ColorString(Color.white, " - ") + $"({GameData.Instance.CompletedTasks}/{GameData.Instance.TotalTasks})"));
        return ProgressText.ToString();
    }
}
