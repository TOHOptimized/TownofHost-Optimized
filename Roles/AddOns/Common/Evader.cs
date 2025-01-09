using static TOHE.Options;

namespace TOHE.Roles.AddOns.Common;

public static class Evader
{
    private const int Id = 28600;

    public static OptionItem ImpCanBeEvader;
    public static OptionItem CrewCanBeEvader;
    public static OptionItem NeutralCanBeEvader;

    public static void SetupCustomOptions()
    {
        SetupAdtRoleOptions(Id, CustomRoles.Evader, canSetNum: true);
        ImpCanBeEvader = BooleanOptionItem.Create(Id + 10, "ImpCanBeEvader", true, TabGroup.Addons, false).SetParent(CustomRoleSpawnChances[CustomRoles.Evader]);
        CrewCanBeEvader = BooleanOptionItem.Create(Id + 11, "CrewCanBeEvader", true, TabGroup.Addons, false).SetParent(CustomRoleSpawnChances[CustomRoles.Evader]);
        NeutralCanBeEvader = BooleanOptionItem.Create(Id + 12, "NeutralCanBeEvader", true, TabGroup.Addons, false).SetParent(CustomRoleSpawnChances[CustomRoles.Evader]);
    }
}
