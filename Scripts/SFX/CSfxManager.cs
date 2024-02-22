using System;
using System.Collections.Generic;
using UnityEngine;

namespace CDK.SFX {
    public class CSfxManager : MonoBehaviour {
        
        #region <<---------- Monobehaviour Singleton ---------->>

        public static CSfxManager get {
            get {
                if (CSingletonHelper.CannotCreateAnyInstance() || _instance != null) return _instance;
                return (_instance = CSingletonHelper.CreateInstance<CSfxManager>("Manager - Sfx"));
            }
        }
        private static CSfxManager _instance;

        #endregion <<---------- Monobehaviour Singleton ---------->>
        
        Dictionary<ParticleSystem, ParticleSystem> _sfxInstances;


        private void Awake() {
            _sfxInstances = new();
        }

        public void PlaySfx(ParticleSystem key, Vector3 position, Quaternion rotation = default) {
            if (!_sfxInstances.TryGetValue(key, out var instance) || instance == null) {
                var newInstance = Instantiate(key, position, rotation);
                _sfxInstances[key] = newInstance;
                return;
            }
            instance.transform.SetPositionAndRotation(position, rotation);
            instance.Play(true);
        }
        
    }
}