using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

public class SelectLanguage : MonoBehaviour, IPointerClickHandler
{
    void Start()
    {
        var localizationSettings = LocalizationSettings.GetInstanceDontCreateDefault();

        // Use included settings if one is available.
        if (localizationSettings != null)
        {
            Debug.Log("Using included localization data...");
            return;
        }

        Debug.Log("Creating default localization data...");

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

    static void OnSelectedLocaleChanged(UnityEngine.Localization.Locale newLocale)
    {
        Debug.Log("OnSelectedLocaleChanged: The locale just changed to " + newLocale.ToString());

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var availableLocales = LocalizationSettings.AvailableLocales;
        switch (name)
        {
            case "Chinese":
                LocalizationSettings.SelectedLocale = availableLocales.GetLocale("zh-CN");
                break;
            case "English":
                LocalizationSettings.SelectedLocale = availableLocales.GetLocale("en-US");
                break;
        }
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene() {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("SceneMain");
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone) {
            yield return null;
        }
    }
}
