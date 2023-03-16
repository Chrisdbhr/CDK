using System;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

#if !DISABLESTEAMWORKS
using Steamworks;
#endif

namespace CDK.Localization {
    [DisplayName("CDK Startup Selection")]
    [Serializable]
    public class StartupSelectorExample : IStartupLocaleSelector {
 
        public Locale GetStartupLocale(ILocalesProvider availableLocales) {
            Debug.Log("Initializing Localization System.");
            
            // From player prefs
            var prefs = CPlayerPrefs.Current;
            if (!prefs.Language.CIsNullOrEmpty()) {
                var locale = GetLocaleByString(prefs.Language);
                if (locale != null) return locale;
            }

            #if !DISABLESTEAMWORKS && STEAMWORKS_NET
            // From Steam
            try {
                if (CSteamManager.Initialized) {
                    var locale = GetLocaleByString(SteamApps.GetCurrentGameLanguage());
                    if (locale != null) return locale;
                }
            }
            catch (Exception e) {
                Debug.LogException(e);
            }
            #endif
            
            // Return the Locale that matches the language field or null if one does not exist.
            return GetLocalEquivalentFromSystem();
        }

        private static Locale GetLocalEquivalentFromSystem() {
            var systemCulture = System.Globalization.CultureInfo.CurrentCulture;
            foreach (var locale in LocalizationSettings.AvailableLocales.Locales) {
                var currentCulture = locale.Identifier.CultureInfo;
                if (Equals(currentCulture, systemCulture) ||
                    Equals(currentCulture, systemCulture.Parent)) {
                    Debug.Log($"Detected {systemCulture} and auto selected language {currentCulture}");
                    return locale;
                }
            }

            Debug.Log($"Could not auto select language for {systemCulture}");
            return null;
        }

        private static Locale GetLocaleByString(string localeStr) {
            return LocalizationSettings.AvailableLocales.GetLocale(new LocaleIdentifier(localeStr));
        }
        
    }
}