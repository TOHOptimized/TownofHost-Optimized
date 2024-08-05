using System.Collections.Generic;
using System.Linq;

namespace TOHE;

public class DevUser(string code = "", string color = "null", string tag = "null", bool isUp = false, bool isDev = false, bool deBug = false, bool colorCmd = false, string upName = "Unknown")
{
    public string Code { get; set; } = code;
    public string Color { get; set; } = color;
    public string Tag { get; set; } = tag;
    public bool IsUp { get; set; } = isUp;
    public bool IsDev { get; set; } = isDev;
    public bool DeBug { get; set; } = deBug;
    public bool ColorCmd { get; set; } = colorCmd;
    public string UpName { get; set; } = upName;

    public bool HasTag() => Tag != "null";
    public string GetTag() => Color == "null" ? $"<size=2>{Tag}</size>\r\n" : $"<color={Color}><size=2>{(Tag == "#Dev" ? Translator.GetString("Developer") : Tag)}</size></color>\r\n";
}

public static class DevManager
{
    public static DevUser DefaultDevUser = new();
    public static List<DevUser> DevUserList = [];

    public static void Init()
    {
        DevUserList =
        [

            // Karped stays bcs he is cool
            new(code: "actorour#0029", color: "#ffc0cb", tag: "Original Developer", isUp: true, isDev: true, deBug: true, colorCmd: true, upName: "KARPED1EM"),

            // Gurge also stays bcs he is cool too
            new(code: "neatnet#5851", color: "#FFFF00", tag: "The 200IQ guy", isUp: true, isDev: false, deBug: false, colorCmd: false, upName: "The 200IQ guy"),
        
            // Lime
            new(code: "latevoice#4590", color: "#00ff00", tag: "The entire circus", isUp: true, isDev: true, deBug: true, colorCmd: true, upName: "Lime"),

            // Ape
            new(code: "simianpair#1270", color: "#0e2f44", tag: "Executive", isUp: true, isDev: true, deBug: true, colorCmd: true, upName: "Ape"),

            // Andries
            new(code: "teemothy#6171", color: "#3e5f8a", tag: "★✦⋆ Andries/Coder ⋆✦★", isUp: true, isDev: true, deBug: true, colorCmd: true, upName: "Andries"),

            // Dailyhare
            new(code: "noshsame#8116", color: "#011efe", tag: "Bait Killer", isUp: true, isDev: true, deBug: true, colorCmd: true, upName: "Dailyhare"),

            // BXO
            new(code: "justgust#5169", color: "#07296c", tag: "Verified Host", isUp: true, isDev: false, deBug: true, colorCmd: true, upName: "BXO"),

            // Apoc
            new(code: "crunchwide#1938", color: "#ffa500", tag: "Verified Host", isUp: true, isDev: false, deBug: true, colorCmd: true, upName: "Apoc"),

            // Plague
            new(code: "trunksun#2271", color: "#808000", tag: "Verified Host", isUp: true, isDev: false, deBug: true, colorCmd: true, upName: "THE PLAGUE"),

        ];
    }

    public static bool IsDevUser(this string code) => DevUserList.Any(x => x.Code == code);
    public static DevUser GetDevUser(this string code) => code.IsDevUser() ? DevUserList.Find(x => x.Code == code) : DefaultDevUser;
}
