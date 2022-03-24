using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.PropertyVariants;
using UnityEngine.Localization.PropertyVariants.TrackedObjects;
using UnityEngine.Localization.PropertyVariants.TrackedProperties;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace CDK.Localization {
    
    [Serializable]
    [DisplayName("Text Mesh Pro UGUI")]
    [CustomTrackedObject(typeof(TextMeshProUGUI), false)]
    public class TrackedTextMeshProUGUI : TrackedObject {

        private const string Property = "m_fontAsset";
        
        public override AsyncOperationHandle ApplyLocale(Locale variantLocale, Locale defaultLocale) {
            var textFontProperty = GetTrackedProperty(Property);
            if (textFontProperty == null) return default;

            // Check if the Asset is stored in an Asset Table
            if (textFontProperty is LocalizedAssetProperty localizedFontAssetProperty && localizedFontAssetProperty.LocalizedObject is LocalizedFontAsset localizedTMPText) {
                localizedTMPText.LocaleOverride = variantLocale;
                var loadHandle = localizedTMPText.LoadAssetAsync();
                if (loadHandle.IsDone) TMPTextLoaded(loadHandle);
                else {
                    loadHandle.Completed += TMPTextLoaded;
                    return loadHandle;
                }
            }
            // Check if the Asset is stored locally
            else if (textFontProperty is UnityObjectProperty localAssetProperty) {
                if (localAssetProperty.GetValue(variantLocale.Identifier, defaultLocale.Identifier, out var text)) {
                    SetTMPProperties(text as TMP_FontAsset);
                }
            }

            return default;
        }

        void TMPTextLoaded(AsyncOperationHandle<TMP_FontAsset> loadHandle) {
            SetTMPProperties(loadHandle.Result);
        }

        void SetTMPProperties(TMP_FontAsset fontAsset) {
            var text = (TextMeshProUGUI)Target;
            text.font = fontAsset;
        }

        public override bool CanTrackProperty(string propertyPath) {
            return propertyPath == Property;
        }
    }
}