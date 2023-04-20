using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CDK {
    public class COutOfBoundsDestroyer : MonoBehaviour {

        private static COutOfBoundsDestroyer _instance;
        
        private CompositeDisposable _disposables;
        private const float ValueLimit = 100000f;



        
        public static void ActiveSceneChangedListener(Scene oldScene, Scene newScene) {
            if (newScene.name == CSceneManager.TemporarySceneName) return;
            SceneManager.activeSceneChanged -= ActiveSceneChangedListener;
            CreateMonitor();
        }


        #region <<---------- Mono Behaviour ---------->>

        private void Awake() {
            if (_instance != null) {
                Debug.LogError("Will not create a duplicated instance!", _instance);
                this.gameObject.CDestroy();
                return;
            }
            
            this._disposables?.Dispose();
            this._disposables = new CompositeDisposable();

            this.name = "Out of Bounds Monitor";
            this.transform.SetAsLastSibling();

            this.CStartCoroutine(MonitorRoutine());
        }
        
        private void OnDestroy() {
            this._disposables?.Dispose();

            if (_instance == this) {
                SceneManager.activeSceneChanged += ActiveSceneChangedListener;
            }
        }

        #endregion <<---------- Mono Behaviour ---------->>


        

        #region <<---------- General ---------->>

        IEnumerator MonitorRoutine() {
            do {
                yield return null;
                var allTransforms = FindObjectsOfType<Transform>();
                for (int i = 0; i < allTransforms.Length; i++) {
                    if (allTransforms[i] == null || allTransforms[i] == null) {
                        yield return null;
                        continue;
                    }
                    if (IsPositionInvalid(allTransforms[i].transform.localPosition)) {
                        Debug.LogWarning($"Disabling GameObject '{allTransforms[i].name}' that is out of bounds! Position: {allTransforms[i].localPosition}");
                        if(allTransforms[i].TryGetComponent<CHealthComponent>(out var h)) {
                            h.Kill();
                        }
                        allTransforms[i].gameObject.SetActive(false);
                    }
                    yield return null;
                }
            } while (this.enabled);
        }
        
        public static void CreateMonitor() {
            if (_instance != null) {
                Debug.LogError("Will not StartMonitoring because a monitor object is already created!");
                return;
            }
            _instance = new GameObject("Out of Bounds Monitor").AddComponent<COutOfBoundsDestroyer>();
        }

        bool IsPositionInvalid(Vector3 v) {
            return v.y >= ValueLimit || v.y <= -ValueLimit 
                || v.x >= ValueLimit || v.x <= -ValueLimit
                || v.z >= ValueLimit || v.z <= -ValueLimit;
        }
        
        #endregion <<---------- General ---------->>

    }
}