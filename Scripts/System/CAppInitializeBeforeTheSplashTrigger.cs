using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

#if FMOD
using FMODUnity;
#endif

#if REWIRED
using Rewired;
#endif

namespace CDK {
    public class CAppInitializeBeforeTheSplashTrigger : MonoBehaviour {

        [SerializeField] CSceneField _nextScene;
        [SerializeField] TextMeshProUGUI _textLoadState;




        #region <<---------- MonoBehaviour ---------->>

        IEnumerator Start() {
            Debug.Log("Initializing scene that preloads initial assets.");

            this.CStartCoroutine(DelayedShowText());

            #if REWIRED
            SetStateText("Waiting for Rewired to get ready...");
            while (!ReInput.isReady) yield return null;
            #endif
            #if FMOD
            SetStateText("Waiting for FMOD to Load Master Banks...");
            while (!RuntimeManager.HaveMasterBanksLoaded) yield return null;
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