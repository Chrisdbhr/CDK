using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CDK {
    public class CSceneAreaReceiver : MonoBehaviour {

        #region <<---------- Properties and Fields ---------->>

        public CCharacter_Base Character;
        private readonly List<CSceneArea> _activeSceneAreas = new List<CSceneArea>();

        #endregion <<---------- Properties and Fields ---------->>



        
        #region <<---------- Mono Behaviour ---------->>

        private void Awake() {
            if (this.Character == null) this.Character = this.GetComponentInChildren<CCharacter_Base>();
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        private void OnDestroy() {
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }

        private void Update() {
            this.Character.MaxMovementSpeed = this.GetMaxMovementSpeed();
        }

        #endregion <<---------- Mono Behaviour ---------->>




        #region <<---------- Callback ---------->>

        private void OnActiveSceneChanged(Scene oldScene, Scene newScene) {
            if (this._activeSceneAreas.RemoveAll(a => a == null) > 0) {
                Debug.Log($"Active Scene Changed. Removed null SceneArea from '{this.name}'");
            }
        }

        #endregion <<---------- Callback ---------->>

        
        

        #region <<---------- Scene Areas ---------->>
        
        public void AddSceneArea(CSceneArea area) {
            this._activeSceneAreas.Add(area);
        }
		
        public void RemoveSceneArea(CSceneArea area) {
            this._activeSceneAreas.Remove(area);
        }

        private CMovementSpeed GetMaxMovementSpeed() {
            return this._activeSceneAreas.Count > 0 
            ? this._activeSceneAreas.Min(a => a.MaximumMovementState) 
            : CMovementSpeed.Sprint;
        }

        #endregion <<---------- Scene Areas ---------->>
        
    }
}