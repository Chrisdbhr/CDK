using System;
using System.Collections;
using FMODUnity;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK.Scripts.Lights {
    [RequireComponent(typeof(Light))]
    public class CFlickeringLight : MonoBehaviour {
        
        #region <<---------- Properties and Fields ---------->>
        
        [CMinMaxSlider(0.01f, 1f)]
        [SerializeField] private Vector2 _intensityMultiplierRange = new Vector2(0.5f, 0.5f);
        
        [CMinMaxSlider(0.01f, 300f)]
        [SerializeField] private Vector2 _intervalTimeRange = new Vector2(0.1f, 5f);
        
        [CMinMaxSlider(0.01f, 0.2f)]
        [SerializeField] private Vector2 _timeFlickering = new Vector2(0.01f, 1f);
        
        #if FMOD
        [SerializeField] private EventReference _soundOnFlick;
        [SerializeField] private EventReference _soundOnReturnToNormal;
        #endif
        
        [SerializeField] private CUnityEvent _onLightFlick;
        [SerializeField] private CUnityEvent _onLightReturnToNormal;

        float _initialIntensity;
        Light _light;
        Coroutine _flickerCoroutine;
        
        #endregion <<---------- Properties and Fields ---------->>

        #region MonoBehaviour

        [MenuItem("CONTEXT/Light/Add Flickering Light component")]
        private static void AddFlickeringLightComponent(MenuCommand command) {
            if(!(command.context is Light light)) return;
            light.gameObject.AddComponent<CFlickeringLight>();
        }
        
        private void Awake() {
            this._light = this.GetComponent<Light>();
            _initialIntensity = _light.intensity;
        }

        private void OnEnable() {
            this.CStartCoroutine(this.Flicker());
        }
        
        private void OnDisable() {
            this.CStopCoroutine(this._flickerCoroutine);
        }

        #endregion
        
        private IEnumerator Flicker() {
            while (this.enabled) {
                yield return new WaitForSeconds(Random.Range(this._intervalTimeRange.x, this._intervalTimeRange.y));
                Flick();
                yield return new WaitForSeconds(Random.Range(this._timeFlickering.x, this._timeFlickering.y));
                ReturnToNormal(); 
            }
        }

        void Flick() {
            this._light.intensity = this._initialIntensity * Random.Range(this._intensityMultiplierRange.x, this._intensityMultiplierRange.y);
            _onLightFlick?.Invoke();
            #if FMOD
            this._soundOnFlick.CDoIfNotNull(e=>RuntimeManager.PlayOneShotAttached(e, this.gameObject));
            #endif
        }

        void ReturnToNormal() {
            this._light.intensity = _initialIntensity;
            _onLightReturnToNormal?.Invoke();
            #if FMOD
            this._soundOnReturnToNormal.CDoIfNotNull(e=>RuntimeManager.PlayOneShotAttached(e, this.gameObject));
            #endif
        }
    }
}