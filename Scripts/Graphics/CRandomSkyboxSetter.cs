using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace CDK.Graphics {
    public class CRandomSkyboxSetter : MonoBehaviour {

        [SerializeField] private bool _updateGiToo = true;
        [SerializeField] private Material[] _skyboxes;
        
        
        
        
        private void Awake() {
            ChooseSkybox();
        }

        #if ODIN_INSPECTOR
        [Button]
        #endif
        public void ChooseSkybox() {
            RenderSettings.skybox = this._skyboxes.CRandomElement();
            if(_updateGiToo) DynamicGI.UpdateEnvironment();
        }
    }
}