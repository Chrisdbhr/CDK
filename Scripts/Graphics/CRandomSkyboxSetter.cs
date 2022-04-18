using System;
using UnityEngine;

namespace CDK.Graphics {
    public class CRandomSkyboxSetter : MonoBehaviour {

        [SerializeField] private Material[] _skyboxes;
        
        
        private void Awake() {
            RenderSettings.skybox = this._skyboxes.CRandomElement();
            DynamicGI.UpdateEnvironment();
        }
    }
}