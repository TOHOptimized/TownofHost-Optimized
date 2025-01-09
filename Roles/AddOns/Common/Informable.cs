/* using static UnityEngine.GraphicsBuffer;
using static TOHE.Utils;
using static TOHE.Translator;

namespace TOHE.Roles.AddOns.Common;

public static class Informable
{
    private const int Id = 31100;

    public static OptionItem ImpCanBeInformable;
    public static OptionItem CrewCanBeInformable;
    public static OptionItem NeutralCanBeInformable;

    public static Dictionary<byte, string> InformableNotify = [];
    public static void SetupCustomOptions()
    {
        Options.SetupAdtRoleOptions(Id, CustomRoles.Informable, canSetNum: true);
        ImpCanBeInformable = BooleanOptionItem.Create(Id + 10, "ImpCanBeInformable", true, TabGroup.Addons, false).SetParent(Options.CustomRoleSpawnChances[CustomRoles.Informable]);
        CrewCanBeInformable = BooleanOptionItem.Create(Id + 11, "CrewCanBeInformable", true, TabGroup.Addons, false).SetParent(Options.CustomRoleSpawnChances[CustomRoles.Informable]);
        NeutralCanBeInformable = BooleanOptionItem.Create(Id + 12, "NeutralCanBeInformable", true, TabGroup.Addons, false).SetParent(Options.CustomRoleSpawnChances[CustomRoles.Informable]);
    }
    public static void Init()
    {
        InformableNotify = [];
    }
    public static void Clear()
    {
        InformableNotify.Clear();
    }
    public static void OnReportDeadBody(PlayerControl reporter, NetworkedPlayerInfo deadBody)
    {
        if (deadBody.Object.Is(CustomRoles.Informable) && deadBody != null && deadBody.Object != null && !deadBody.Object.IsAlive() && reporter.PlayerId != deadBody.PlayerId)
        {
            string msg;
            msg = string.Format(Translator.GetString("InformableNotice"), deadBody.Object.GetRealName(), deadBody.Object.GetDisplayRoleAndSubName(deadBody.Object, false));
            InformableNotify.Add(reporter.PlayerId, msg);
        }
    }
}
*/