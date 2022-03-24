using System;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.PropertyVariants;
using UnityEngine.Localization.PropertyVariants.TrackedObjects;
using UnityEngine.Localization.PropertyVariants.TrackedProperties;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace CDK.Localization {
    [Serializable]
    [DisplayName("Audio Source")]
    [CustomTrackedObject(typeof(AudioSource), false)]
    public class TrackedAudioSource : TrackedObject
    {
        public override AsyncOperationHandle ApplyLocale(Locale variantLocale, Locale defaultLocale)
        {
            var audioClipProperty = GetTrackedProperty("m_audioClip");
            if (audioClipProperty == null)
                return default;

            // Check if the Asset is stored in an Asset Table
            if (audioClipProperty is LocalizedAssetProperty localizedAssetProperty &&
                localizedAssetProperty.LocalizedObject is LocalizedAudioClip localizedAudioClip)
            {
                localizedAudioClip.LocaleOverride = variantLocale;
                var loadHandle = localizedAudioClip.LoadAssetAsync();
                if (loadHandle.IsDone)
                    AudioClipLoaded(loadHandle);
                else
                {
                    loadHandle.Completed += AudioClipLoaded;
                    return loadHandle;
                }
            }
            // Check if the Asset is stored locally
            else if (audioClipProperty is UnityObjectProperty localAssetProperty)
            {
                if (localAssetProperty.GetValue(variantLocale.Identifier, defaultLocale.Identifier, out var clip))
                    SetAudioClip(clip as AudioClip);
            }

            return default;
        }

        void AudioClipLoaded(AsyncOperationHandle<AudioClip> loadHandle)
        {
            SetAudioClip(loadHandle.Result);
        }

        void SetAudioClip(AudioClip clip)
        {
            var source = (AudioSource)Target;
            source.Stop();
            source.clip = clip;
            if (clip != null)
                source.Play();
        }

        public override bool CanTrackProperty(string propertyPath)
        {
            // We only care about the Audio clip
            return propertyPath == "m_audioClip";
        }
    }


}