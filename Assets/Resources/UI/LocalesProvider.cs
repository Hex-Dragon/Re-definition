using System.Collections.Generic;
using UnityEngine.Localization.Settings;

public class LocalesProvider : ILocalesProvider{
    public List<UnityEngine.Localization.Locale> Locales { get; } = new List<UnityEngine.Localization.Locale>();

    public UnityEngine.Localization.Locale GetLocale(UnityEngine.Localization.LocaleIdentifier id) => Locales.Find(l => l.Identifier == id);
    public void AddLocale(UnityEngine.Localization.Locale locale) => Locales.Add(locale);
    public bool RemoveLocale(UnityEngine.Localization.Locale locale) => Locales.Remove(locale);
}
