#if EasyButtons
using EasyButtons;
#endif
using UnityEngine;

namespace CDK.Graphics {
    public class CRandomSkyboxSetter : MonoBehaviour {

        [SerializeField] private Material[] _skyboxes;
        
        
        
        
        private void Awake() {
            ChooseSkybox();
        }

        #if EasyButtons
        [Button]
        #endif
        public void ChooseSkybox() {
            RenderSettings.skybox = this._skyboxes.CRandomElement();
            DynamicGI.UpdateEnvironment();
        }
    }
}