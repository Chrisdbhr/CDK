using System;
using UnityEngine;

namespace CDK {
    public class CFlashRendererColor : MonoBehaviour {

        #region <<---------- Properties and Fields ---------->>

        [SerializeField] private Renderer _targetRenderer;
        [SerializeField] private float _colorReturnSpeed = 0.25f;
        [SerializeField] private string _shaderKeyword = "_Color";
        private int _shaderKeywordId;
        private Material _targetMaterial;
        private Color _initialColor = Color.white;

        #endregion <<---------- Properties and Fields ---------->>


        
        
        #region <<---------- MonoBehaviour ---------->>

        private void Awake() {
            try {
                this._targetRenderer.CAssertIfNull("Target renderer cannot be null!", this);
                this._targetMaterial = this._targetRenderer.material;
                this._shaderKeywordId = GetValidShaderKeyword(_shaderKeyword);
                this._initialColor = this._targetMaterial.GetColor(this._shaderKeywordId);
            }
            catch (Exception e) {
                this.enabled = false;
                Debug.LogException(e);
            }
        }

        private void Update() {
            SetColor(
                this._targetMaterial.GetColor(this._shaderKeywordId).CLerp(this._initialColor, this._colorReturnSpeed * CTime.DeltaTimeScaled)
            );
        }
        
        #if UNITY_EDITOR
        private void OnValidate() {
            if(!this._targetRenderer) this._targetRenderer = this.GetComponent<Renderer>();
        }
        #endif

        #endregion <<---------- MonoBehaviour ---------->>


        int GetValidShaderKeyword(string initialKeyword) {
            if (initialKeyword.CIsNullOrEmpty()) {
                Debug.LogError("Shader keyword cannot be null or empty!");
                return -1;
            }

            // list of possible keywords for material main colors:
            // https://docs.unity3d.com/Manual/SL-ShaderReplacement.html

            string[] possibleKeywords =
            {
                "_Color",
                "_BaseColor",
                "_BaseColorMap",
                "_BaseColorMapST",
                "_BaseColorMapUV",
                "_BaseColorMapUVSec",
                "_BaseColorMapUVSecScale",
                "_BaseColorMapUVSecOffset",
                "_BaseColorMapUVSecRotation",
                "_ColorMap",
                "_ColorMapST",
                "_ColorMapUV",
                "_ColorMapUVSec",
                "_ColorMapUVSecScale",
                "_ColorMapUVSecOffset",
                "_ColorMapUVSecRotation",
                "_MainColor",
                "_BaseMap",
                "_BaseMapST",
                "_BaseMapUV",
                "_BaseMapUVSec",
                "_BaseMapUVSecScale",
                "_BaseMapUVSecOffset",
                "_BaseMapUVSecRotation",
                "_MainTex",
                "_MainTexST",
                "_MainTexUV",
                "_MainTexUVSec",
                "_MainTexUVSecScale",
                "_MainTexUVSecOffset",
                "_MainTexUVSecRotation",
                "_BaseColorMap",
                "_BaseColorMapST",
                "_BaseColorMapUV",
                "_BaseColorMapUVSec",
                "_BaseColorMapUVSecScale",
                "_BaseColorMapUVSecOffset",
                "_BaseColorMapUVSecRotation",
                "_ColorMap",
                "_ColorMapST",
                "_ColorMapUV",
                "_ColorMapUVSec"
            };

            foreach (var key in possibleKeywords) {
                if (this._targetMaterial.HasColor(key)) {
                    Debug.Log("Getting shader keyword: " + key + " for material: " + this._targetMaterial.name + " on object: " + this.name);
                    return Shader.PropertyToID(key);
                }
            }

            Debug.LogError("Shader keyword not found");
            return -1;
        }

        private void SetColor(Color color) {
            if (!this.enabled) return;
            this._targetMaterial.SetColor(this._shaderKeywordId, color);
        }

        public void FlashColor(Color color) {
            this.SetColor(color);
        }

        public void FlashRed() {
            this.FlashColor(Color.red);
        }
        
    }
}