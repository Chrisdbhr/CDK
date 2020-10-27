using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace CDK {
    public class CSplashScreen : MonoBehaviour {

        #region <<---------- Properties and Fields ---------->>
        
        [Header("Active scene will be the first of the list")]
        [SerializeField] private CSceneField[] _firstScenes;

        [SerializeField] private float _timeToWaitBeforeNextScene = 2f;
        [NonSerialized] private bool _pressedSkip;

        #endregion <<---------- Properties and Fields ---------->>

        


        #region <<---------- Events ---------->>

        [SerializeField] private UnityEvent OnTriggerFadeOut;

        #endregion <<---------- Events ---------->>


        
        
        #region <<---------- MonoBehaviour ---------->>
        
        private IEnumerator Start() {
            var currentScene = this.gameObject.scene;
            
            CSceneField firstScene = null;
            AsyncOperation firstSceneAsyncOperation = null;

            foreach (var scene in this._firstScenes) {
                var asyncOp = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
                asyncOp.allowSceneActivation = false;
                
                if (firstScene == null) {
                    firstScene = scene;
                    firstSceneAsyncOperation = asyncOp;
                }
            }

            float remainingTime = this._timeToWaitBeforeNextScene;
            while (remainingTime > 0f) {
                remainingTime -= Time.deltaTime; // must not be time scaled
            }

            yield return null;
            
            this.OnTriggerFadeOut?.Invoke();
            
            yield return new WaitForSeconds(1f);

            firstSceneAsyncOperation.allowSceneActivation = true;
            
            var firstSceneName = SceneManager.GetSceneByName(firstScene);
            SceneManager.SetActiveScene(firstSceneName);

            SceneManager.UnloadSceneAsync(currentScene);
        }

        private void Update() {
            this._pressedSkip 
                    = Input.GetKeyDown(CInputKeys.INTERACT) 
                    || Input.GetKeyDown(CInputKeys.MENU_INVENTORY)
                    || Input.GetKeyDown(CInputKeys.MENU_PAUSE)
                    ;
        }
        
        #endregion <<---------- MonoBehaviour ---------->>

    }
}
