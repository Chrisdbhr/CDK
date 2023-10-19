using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

#if FMOD
using FMODUnity;
#endif

#if Rewired
using Rewired;
#endif

#if UnityLocalization
using UnityEngine.Localization.Settings;
#endif

namespace CDK {
    public class CAppInitializeBeforeTheSplashTrigger : MonoBehaviour {

        [SerializeField] private CSceneField _nextScene;
        [SerializeField] private TextMeshProUGUI _textLoadState;




        #region <<---------- MonoBehaviour ---------->>

        private IEnumerator Start() {
            Debug.Log("Initializing scene that preloads initial assets.");

            this.CStartCoroutine(DelayedShowText());

            #if Rewired
            SetStateText("Waiting for Rewired to get ready...");
            while (!ReInput.isReady) {
                yield return null;
            }
            #endif
            #if FMOD
            SetStateText("Waiting for FMOD to Load Master Banks...");
            while (!RuntimeManager.HaveMasterBanksLoaded) {
                yield return null;
            }
            #endif

            #if UnityLocalization
            SetStateText("Waiting for Localization System get ready...");
            yield return LocalizationSettings.InitializationOperation;
            #endif

            yield return null;

            SetStateText($"Starting to load next scene: '{this._nextScene}'");
            var asyncOp = SceneManager.LoadSceneAsync(this._nextScene, LoadSceneMode.Single);
            asyncOp.allowSceneActivation = true;
        }

        #endregion <<---------- MonoBehaviour ---------->>




        #region <<---------- General ---------->>

        private IEnumerator DelayedShowText() {
            yield return new WaitForSeconds(2f);
            _textLoadState.enabled = true;
        }

        void SetStateText(string text) {
            _textLoadState.text = text;
            Debug.Log(text);
        }

        #endregion <<---------- General ---------->>
    }
}