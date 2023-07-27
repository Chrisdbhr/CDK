using System;
using UnityEngine;

namespace CDK {
    public class CFlashRendererColor : MonoBehaviour {

        [SerializeField] private Renderer _targetRenderer;
        [SerializeField] private float _colorReturnSpeed = 1f;
        [SerializeField] private string _shaderKeyword = "_Color";
        private int _shaderKeyboardId;
        private Material _targetMaterial;
        private Color _initialColor;


        
        
        #region <<---------- MonoBehaviour ---------->>

        private void Awake() {
            this._shaderKeyboardId = Shader.PropertyToID(this._shaderKeyword);
            this._targetRenderer.CAssertIfNull("Target renderer cannot be null!", this);
            this._targetMaterial = this._targetRenderer.material;
            this._initialColor = this._targetMaterial.GetColor(this._shaderKeyboardId);
        }

        private void Update() {
            if (this._targetMaterial.GetColor(this._shaderKeyboardId) == this._initialColor) return;
            this._targetMaterial.SetColor(this._shaderKeyboardId, this._targetMaterial.GetColor(this._shaderKeyboardId).CLerp(this._initialColor, this._colorReturnSpeed * CTime.DeltaTimeScaled));
        }
        
        #if UNITY_EDITOR
        private void OnValidate() {
            if(!this._targetRenderer) this._targetRenderer = this.GetComponent<Renderer>();
        }
        #endif

        #endregion <<---------- MonoBehaviour ---------->>

        
        
        
        public void FlashColor(Color color) {
            this._targetMaterial.color = color;
        }

        public void FlashRed() {
            this._targetMaterial.color = Color.red;
        }
        
    }
}