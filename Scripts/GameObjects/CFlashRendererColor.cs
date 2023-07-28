using System;
using UnityEngine;

namespace CDK {
    public class CFlashRendererColor : MonoBehaviour {

        [SerializeField] private Renderer _targetRenderer;
        [SerializeField] private float _colorReturnSpeed = 0.25f;
        [SerializeField] private string _shaderKeyword = "_Color";
        private int _shaderKeyboardId;
        private Material _targetMaterial;
        private Color _initialColor = Color.white;


        
        
        #region <<---------- MonoBehaviour ---------->>

        private void Awake() {
            try {
                this._shaderKeyboardId = Shader.PropertyToID(this._shaderKeyword);
                this._targetRenderer.CAssertIfNull("Target renderer cannot be null!", this);
                this._targetMaterial = this._targetRenderer.material;
                this._initialColor = this._targetMaterial.GetColor(this._shaderKeyboardId);
            }
            catch (Exception e) {
                this.enabled = false;
                Debug.LogException(e);
            }
        }

        private void Update() {
            this.SetColor(this._targetMaterial.GetColor(this._shaderKeyboardId).CLerp(this._initialColor, this._colorReturnSpeed * CTime.DeltaTimeScaled));
        }
        
        #if UNITY_EDITOR
        private void OnValidate() {
            if(!this._targetRenderer) this._targetRenderer = this.GetComponent<Renderer>();
        }
        #endif

        #endregion <<---------- MonoBehaviour ---------->>



        private void SetColor(Color color) {
            if (!this.enabled) return;
            this._targetMaterial.SetColor(this._shaderKeyboardId, color);
        }
        
        
        public void FlashColor(Color color) {
            this.SetColor(color);
        }

        public void FlashRed() {
            this.SetColor(Color.red);
        }
        
    }
}