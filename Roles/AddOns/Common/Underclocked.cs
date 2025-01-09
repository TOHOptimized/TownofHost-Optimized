using static TOHE.Options;

namespace TOHE.Roles.AddOns.Common;

public static class Underclocked
{
    private const int Id = 30000;

    public static OptionItem UnderclockedIncrease;

    public static void SetupCustomOptions()
    {
        SetupAdtRoleOptions(Id, CustomRoles.Underclocked, canSetNum: true);
        UnderclockedIncrease = FloatOptionItem.Create(Id + 10, "UnderclockedIncrease", new(5f, 100f, 2.5f), 40f, TabGroup.Addons, false).SetParent(CustomRoleSpawnChances[CustomRoles.Underclocked])
            .SetValueFormat(OptionFormat.Percent);
    }
}
