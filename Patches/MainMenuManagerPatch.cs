using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TOHE.Translator;
using Object = UnityEngine.Object;

namespace TOHE;

[HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start)), HarmonyPriority(Priority.First)]
public class MainMenuManagerStartPatch
{
    public static GameObject amongUsLogo;
    public static GameObject Ambience;
    public static SpriteRenderer ToheLogo { get; private set; }

    private static void Postfix(MainMenuManager __instance)
    {
        amongUsLogo = GameObject.Find("LOGO-AU");

        var rightpanel = __instance.gameModeButtons.transform.parent;
        var logoObject = new GameObject("titleLogo_TOHE");
        var logoTransform = logoObject.transform;

        ToheLogo = logoObject.AddComponent<SpriteRenderer>();
        logoTransform.parent = rightpanel;
        logoTransform.localPosition = new(-0.16f, 0f, 1f);
        logoTransform.localScale *= 1.2f;

        if ((Ambience = GameObject.Find("Ambience")) != null)
        {
            Ambience.SetActive(false);
        }
    }
}
[HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.LateUpdate))]
class MainMenuManagerLateUpdatePatch
{
    private static int lateUpdate = 590;
    //private static GameObject LoadingHint;

    private static void Postfix(MainMenuManager __instance)
    {
        if (__instance == null) return;

        if (lateUpdate <= 600)
        {
            lateUpdate++;
            return;
        }
        lateUpdate = 0;

        var PlayOnlineButton = __instance.PlayOnlineButton;
        if (PlayOnlineButton != null)
        {
            if (RunLoginPatch.isAllowedOnline && !Main.hasAccess)
            {
                var PlayLocalButton = __instance.playLocalButton;
                if (PlayLocalButton != null) PlayLocalButton.gameObject.SetActive(false);

                PlayOnlineButton.gameObject.SetActive(false);
                DisconnectPopup.Instance.ShowCustom(GetString("NoAccess"));
            }
        }
    }
}
[HarmonyPatch(typeof(MainMenuManager))]
public static class MainMenuManagerPatch
{
    private static PassiveButton template;
    private static PassiveButton gitHubButton;
    private static PassiveButton donationButton;
    private static PassiveButton discordButton;
    private static PassiveButton websiteButton;
    //private static PassiveButton patreonButton;

    [HarmonyPatch(nameof(MainMenuManager.Start)), HarmonyPostfix, HarmonyPriority(Priority.Normal)]
    public static void Start_Postfix(MainMenuManager __instance)
    {
        if (template == null) template = __instance.quitButton;

        // FPS
        Application.targetFrameRate = Main.UnlockFPS.Value ? 165 : 60;

        __instance.screenTint.gameObject.transform.localPosition += new Vector3(1000f, 0f);
        __instance.screenTint.enabled = false;
        __instance.rightPanelMask.SetActive(true);
        // The background texture (large sprite asset)
        __instance.mainMenuUI.FindChild<SpriteRenderer>("BackgroundTexture").transform.gameObject.SetActive(false);
        // The glint on the Among Us Menu
        __instance.mainMenuUI.FindChild<SpriteRenderer>("WindowShine").transform.gameObject.SetActive(false);
        __instance.mainMenuUI.FindChild<Transform>("ScreenCover").gameObject.SetActive(false);

        GameObject leftPanel = __instance.mainMenuUI.FindChild<Transform>("LeftPanel").gameObject;
        GameObject rightPanel = __instance.mainMenuUI.FindChild<Transform>("RightPanel").gameObject;
        rightPanel.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        GameObject maskedBlackScreen = rightPanel.FindChild<Transform>("MaskedBlackScreen").gameObject;
        maskedBlackScreen.GetComponent<SpriteRenderer>().enabled = false;
        maskedBlackScreen.transform.localScale = new Vector3(7.35f, 4.5f, 4f);

        __instance.mainMenuUI.gameObject.transform.position += new Vector3(-0.2f, 0f);

        leftPanel.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        leftPanel.gameObject.FindChild<SpriteRenderer>("Divider").enabled = false;
        leftPanel.GetComponentsInChildren<SpriteRenderer>(true).Where(r => r.name == "Shine").ToList().ForEach(r => r.enabled = false);

        GameObject splashArt = new("SplashArt");
        splashArt.transform.position = new Vector3(0, 0f, 600f);
        var spriteRenderer = splashArt.AddComponent<SpriteRenderer>();
        var folder = new System.Text.StringBuilder("TOHE.Resources.Background.");
        IRandom rand = IRandom.Instance;
        if (rand.Next(0, 100) < 30) folder.Append("PrevArtWinner");
        else folder.Append("CurrentArtWinner");
        var assembly = System.Reflection.Assembly.GetExecutingAssembly();
        string[] fileNames = assembly.GetManifestResourceNames().Where(resourceName => resourceName.StartsWith(folder.ToString()) && resourceName.EndsWith(".png")).ToArray();
        int choice = rand.Next(0, fileNames.Length);

        spriteRenderer.sprite = Utils.LoadSprite(fileNames[choice], 150f);

        if (template == null) return;

        // donation Button
        if (donationButton == null)
        {
            donationButton = CreateButton(
                "donationButton",
                new(-1.8f, -1.1f, 1f),
                new(0, 255, 255, byte.MaxValue),
                new(75, 255, 255, byte.MaxValue),
                (UnityEngine.Events.UnityAction)(() => Application.OpenURL(Main.DonationInviteUrl)),
                GetString("SupportUs")); //"Donation"
        }
        donationButton.gameObject.SetActive(Main.ShowDonationButton);

        // GitHub Button
        if (gitHubButton == null)
        {
            gitHubButton = CreateButton(
                "GitHubButton",
                new(-1.8f, -1.5f, 1f),
                new(153, 153, 153, byte.MaxValue),
                new(209, 209, 209, byte.MaxValue),
                (UnityEngine.Events.UnityAction)(() => Application.OpenURL(Main.GitHubInviteUrl)),
                GetString("GitHub")); //"GitHub"
        }
        gitHubButton.gameObject.SetActive(Main.ShowGitHubButton);

        // Discord Button
        if (discordButton == null)
        {
            discordButton = CreateButton(
                "DiscordButton",
                new(-1.8f, -1.9f, 1f),
                new(88, 101, 242, byte.MaxValue),
                new(148, 161, byte.MaxValue, byte.MaxValue),
                (UnityEngine.Events.UnityAction)(() => Application.OpenURL(Main.DiscordInviteUrl)),
                GetString("Discord")); //"Discord"
        }
        discordButton.gameObject.SetActive(Main.ShowDiscordButton);

        // Website Button
        if (websiteButton == null)
        {
            websiteButton = CreateButton(
                "WebsiteButton",
                new(-1.8f, -2.3f, 1f),
                new(251, 81, 44, byte.MaxValue),
                new(211, 77, 48, byte.MaxValue),
                (UnityEngine.Events.UnityAction)(() => Application.OpenURL(Main.WebsiteInviteUrl)),
                GetString("Website")); //"Website"
        }
        websiteButton.gameObject.SetActive(Main.ShowWebsiteButton);

        var howToPlayButton = __instance.howToPlayButton;
        var freeplayButton = howToPlayButton.transform.parent.Find("FreePlayButton");

        if (freeplayButton != null) freeplayButton.gameObject.SetActive(false);

        howToPlayButton.transform.SetLocalX(0);

    }

    public static PassiveButton CreateButton(string name, Vector3 localPosition, Color32 normalColor, Color32 hoverColor, UnityEngine.Events.UnityAction action, string label, Vector2? scale = null)
    {
        var button = Object.Instantiate(template, MainMenuManagerStartPatch.ToheLogo.transform);
        button.name = name;
        Object.Destroy(button.GetComponent<AspectPosition>());
        button.transform.localPosition = localPosition;

        button.OnClick = new();
        button.OnClick.AddListener(action);

        var buttonText = button.transform.Find("FontPlacer/Text_TMP").GetComponent<TMP_Text>();
        buttonText.DestroyTranslator();
        buttonText.fontSize = buttonText.fontSizeMax = buttonText.fontSizeMin = 3.5f;
        buttonText.enableWordWrapping = false;
        buttonText.text = label;
        var normalSprite = button.inactiveSprites.GetComponent<SpriteRenderer>();
        var hoverSprite = button.activeSprites.GetComponent<SpriteRenderer>();
        normalSprite.color = normalColor;
        hoverSprite.color = hoverColor;

        var container = buttonText.transform.parent;
        Object.Destroy(container.GetComponent<AspectPosition>());
        Object.Destroy(buttonText.GetComponent<AspectPosition>());
        container.SetLocalX(0f);
        buttonText.transform.SetLocalX(0f);
        buttonText.horizontalAlignment = HorizontalAlignmentOptions.Center;

        var buttonCollider = button.GetComponent<BoxCollider2D>();
        if (scale.HasValue)
        {
            normalSprite.size = hoverSprite.size = buttonCollider.size = scale.Value;
        }

        buttonCollider.offset = new(0f, 0f);

        return button;
    }
    public static void Modify(this PassiveButton passiveButton, UnityEngine.Events.UnityAction action)
    {
        if (passiveButton == null) return;
        passiveButton.OnClick = new Button.ButtonClickedEvent();
        passiveButton.OnClick.AddListener(action);
    }
    public static T FindChild<T>(this MonoBehaviour obj, string name) where T : Object
    {
        string name2 = name;
        return obj.GetComponentsInChildren<T>().First((T c) => c.name == name2);
    }
    public static T FindChild<T>(this GameObject obj, string name) where T : Object
    {
        string name2 = name;
        return obj.GetComponentsInChildren<T>().First((T c) => c.name == name2);
    }
    public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        IEnumerator<TSource> enumerator = source.GetEnumerator();
        while (enumerator.MoveNext())
        {
            action(enumerator.Current);
        }

        enumerator.Dispose();
    }

    [HarmonyPatch(nameof(MainMenuManager.OpenGameModeMenu))]
    [HarmonyPatch(nameof(MainMenuManager.OpenAccountMenu))]
    [HarmonyPatch(nameof(MainMenuManager.OpenCredits))]
    [HarmonyPostfix]
    public static void OpenMenu_Postfix()
    {
        if (MainMenuManagerStartPatch.ToheLogo != null) MainMenuManagerStartPatch.ToheLogo.gameObject.SetActive(false);
    }
    [HarmonyPatch(nameof(MainMenuManager.ResetScreen)), HarmonyPostfix]
    public static void ResetScreen_Postfix()
    {
        if (MainMenuManagerStartPatch.ToheLogo != null) MainMenuManagerStartPatch.ToheLogo.gameObject.SetActive(true);
    }
}
