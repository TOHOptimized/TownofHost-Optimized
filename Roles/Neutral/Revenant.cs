﻿using TOHE.Roles.Core;
using static TOHE.Translator;

namespace TOHE.Roles.Neutral;
internal class Revenant : RoleBase
{
    //===========================SETUP================================\\
    public override CustomRoles Role => CustomRoles.Revenant;
    private const int Id = 30200;
    public static bool HasEnabled => CustomRoleManager.HasEnabled(CustomRoles.Revenant);
    public override CustomRoles ThisRoleBase => CustomRoles.Crewmate;
    public override Custom_RoleType ThisRoleType => Custom_RoleType.NeutralBenign;
    //==================================================================\\

    public override void SetupCustomOption()
    {
        Options.SetupRoleOptions(Id, TabGroup.NeutralRoles, CustomRoles.Revenant);
    }

    public override bool OnCheckMurderAsTarget(PlayerControl killer, PlayerControl target)
    {
        CustomRoles role = killer.GetCustomRole();

        

        target.RpcChangeRoleBasis(role);
        target.RpcSetCustomRole(role);
        target.GetRoleClass()?.OnAdd(target.PlayerId);

        killer.RpcMurderPlayer(killer);
        killer.SetRealKiller(target);

        target.Notify(string.Format(GetString("RevenantTargeted"), Utils.GetRoleName(role)));

        return false;
    }
}
