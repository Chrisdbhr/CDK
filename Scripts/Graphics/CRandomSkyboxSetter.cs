using EasyButtons;
using UnityEngine;

namespace CDK.Graphics {
    public class CRandomSkyboxSetter : MonoBehaviour {

        [SerializeField] private Material[] _skyboxes;
        
        
        
        
        private void Awake() {
            ChooseSkybox();
        }

        [Button]
        public void ChooseSkybox() {
            RenderSettings.skybox = this._skyboxes.CRandomElement();
            DynamicGI.UpdateEnvironment();
        }
    }
}