using UnityEngine;
using Random = UnityEngine.Random;

namespace CDK.GameObjects {
    public class CRandomizeScaleOnEnable : MonoBehaviour {

        [SerializeField, Min(0.01f)] private float _minValue = 0.01f;
        [SerializeField, Min(0.01f)] private float _maxValue = 2f;
        
        private Vector3 _initialScale;

        private void Awake() {
            this._initialScale = this.transform.localScale;
        }

        private void OnEnable() {
            this.SetScale(this._initialScale * Random.Range(this._minValue, this._maxValue));
        }

        private void OnDisable() {
            this.SetScale(this._initialScale);
        }


        private void SetScale(Vector3 value) {
            this.transform.localScale = value;
        }
    }
}