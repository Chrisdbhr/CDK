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

        public ParticleSystem PlaySfx(ParticleSystem key, Transform target) {
            if (target == null) {
                Debug.LogError("Tried to play sfx on a null target! Playing on Vector3.zero instead.");
                return PlaySfx(key, Vector3.zero);
            }
            return PlaySfx(key, target.position, target.rotation);
        }

        public ParticleSystem PlaySfx(ParticleSystem key, Vector3 position, Quaternion rotation = default) {
            ParticleSystem part;
            if (!_sfxInstances.TryGetValue(key, out part) || part == null) {
                part = this.CInstantiate(key, position, rotation);
                _sfxInstances[key] = part;
            }
            else {
                part.transform.SetPositionAndRotation(position, rotation);
            }
            part.Play(true);
            return part;
        }
        
    }
}