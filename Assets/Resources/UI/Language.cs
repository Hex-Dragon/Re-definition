using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Language : MonoBehaviour
{
    public Rect windowRect = new Rect(640, 240, 300, 200);
    public Color selectColor = Color.yellow;
    public Color defaultColor = Color.gray;
    Vector2 m_ScrollPos;
    Dictionary<UnityEngine.Localization.Locale, string> m_Labels = new Dictionary<UnityEngine.Localization.Locale, string>();

    public bool useActiveLocalizationSettings = false;

    void Start()
    {
        var localizationSettings = LocalizationSettings.GetInstanceDontCreateDefault();

        // Use included settings if one is available.
        if (useActiveLocalizationSettings && localizationSettings != null)
        {
            Debug.Log("Using included localization data");
            return;
        }

        Debug.Log("Creating default localization data");

        // Create our localization settings. If a LocalizationSettings asset has been created and configured in the editor then we can leave this step out.
        localizationSettings = ScriptableObject.CreateInstance<LocalizationSettings>();

        // Replace the default Locales Provider with something we can manage locally for the example
        var localesProvider = new LocalesProvider();
        localizationSettings.SetAvailableLocales(localesProvider);

        // Add the locales we support
        localesProvider.AddLocale(UnityEngine.Localization.Locale.CreateLocale(new UnityEngine.Localization.LocaleIdentifier("zh-CN")));
        localesProvider.AddLocale(UnityEngine.Localization.Locale.CreateLocale(new UnityEngine.Localization.LocaleIdentifier("en-US")));

        var startupSelectors = localizationSettings.GetStartupLocaleSelectors();
        startupSelectors.Clear();
        startupSelectors.Add(new SpecificLocaleSelector { LocaleId = "zh-CN" });

        // Listen to any Locale changed events
        localizationSettings.OnSelectedLocaleChanged += OnSelectedLocaleChanged;

        // Make this our new LocalizationSettings
        LocalizationSettings.Instance = localizationSettings;
    }

    static void OnButtonClick()
    {
        SceneManager.LoadScene("SceneMain");
        Debug.Log("OnButtonClick: Change the scene to SceneMain");
    }

    static void OnSelectedLocaleChanged(UnityEngine.Localization.Locale newLocale)
    {
        Debug.Log("OnSelectedLocaleChanged: The locale just changed to " + newLocale.ToString());

    }

    void OnGUI()
    {
        windowRect = GUI.Window(GetHashCode(), windowRect, DrawWindowContents, "Select Language");
    }

    string GetLocaleLabel(UnityEngine.Localization.Locale locale)
    {
        // Cache our generated labels.
        if (m_Labels.TryGetValue(locale, out var label))
            return label;

        // Create a label which shows the English name and the native name.
        var cultureInfo = locale.Identifier.CultureInfo;

        // If the Locale is custom then it may not have a CultureInfo
        if (cultureInfo != null)
        {
            // We will show a label in the form "<EnglishName>(<NativeName>)" when the Native name is not the same as the English name.
            if (cultureInfo.EnglishName != cultureInfo.NativeName)
                label = $"{cultureInfo.EnglishName}({cultureInfo.NativeName})";
            else
                label = cultureInfo.EnglishName;
        }
        else
        {
            label = locale.ToString();
        }

        m_Labels[locale] = label;
        return label;
    }

    void DrawWindowContents(int id)
    {
        // We need to wait for the Locales to be ready. If we are using the default LocalesProvider then they may still be loading.
        // SelectedLocaleAsync will wait for the Locales to be ready before selecting the SelectedLocale.
        if (!LocalizationSettings.SelectedLocaleAsync.IsDone)
        {
            GUILayout.Label("Initializing Locales: " + LocalizationSettings.SelectedLocaleAsync.PercentComplete);
            GUI.DragWindow();
            return;
        }

        // If we wanted to wait for everything to be initialized including preloading tables then we could check LocalizationSettings.InitializationOperation instead.
        //if (!LocalizationSettings.InitializationOperation.IsDone)
        //{
        //    GUILayout.Label("Initializing Localization: " + LocalizationSettings.InitializationOperation.PercentComplete);
        //    GUI.DragWindow();
        //    return;
        //}

        var availableLocales = LocalizationSettings.AvailableLocales;
        if (availableLocales.Locales.Count == 0)
        {
            GUILayout.Label("No Locales included in the active Localization Settings.");
        }
        else
        {
            // We will use a color to indicate the SelectedLocale
            var originalColor = GUI.contentColor;

            m_ScrollPos = GUILayout.BeginScrollView(m_ScrollPos);
            for (int i = 0; i < availableLocales.Locales.Count; ++i)
            {
                var locale = availableLocales.Locales[i];

                GUI.contentColor = LocalizationSettings.SelectedLocale == locale ? selectColor : defaultColor;
                if (GUILayout.Button(GetLocaleLabel(locale)))
                {
                    LocalizationSettings.SelectedLocale = locale;
                    OnButtonClick();
                }
            }
            GUILayout.EndScrollView();

            GUI.contentColor = originalColor;
        }

        GUI.DragWindow();
    }
}

