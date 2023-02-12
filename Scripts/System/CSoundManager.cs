using System;
using System.Collections;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UniRx;
using UnityEngine;
using Debug = UnityEngine.Debug;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace CDK {
    public class CSoundManager : MonoBehaviour {

        #region <<---------- Properties and Fields ---------->>

        #if FMOD 
        public static StudioListener MainListener;
        #endif

        public const float SoundSpeedInKm = 1224f;
        
        private Dictionary<EventReference, (EventInstance instance, Transform connectedTransform)> _playingSounds;
        private Dictionary<EventReference, CRetainable> _pausedSounds;
        private CBlockingEventsManager _blockingEventsManager;

        #endregion <<---------- Properties and Fields ---------->>



        
        #region <<---------- Mono Behaviour ---------->>
        
        private void Awake() {
            this._blockingEventsManager = CDependencyResolver.Get<CBlockingEventsManager>();
            DontDestroyOnLoad(this);
            this._playingSounds = new Dictionary<EventReference, (EventInstance instance, Transform connectedTransform)>();
            this._pausedSounds = new Dictionary<EventReference, CRetainable>();
        }

        private void LateUpdate(){
            foreach (var playingSound in this._playingSounds) {
                if (playingSound.Value.connectedTransform == null) {
                    this.StopPlaying(playingSound.Key);
                    break;
                }
                var r = playingSound.Value.instance.set3DAttributes(playingSound.Value.connectedTransform.To3DAttributes());
                #if UNITY_EDITOR
                if (r != RESULT.OK) {
                    Debug.LogError($"Issue settings 3d attributes on sound '{playingSound.Key.Path}': {r}");
                }
                #endif
            }
        }
        
        #endregion <<---------- Mono Behaviour ---------->>

       
        

        #region <<---------- General ---------->>

        public EventInstance StartAndPlay(EventReference soundRef, Transform connectedTransform) {
            try{
                #if UNITY_EDITOR
                if (this._playingSounds.ContainsKey(soundRef)) {
                    Debug.LogWarning($"Replacing already created sound.");
                }
                #endif
                
                var soundInstance = RuntimeManager.CreateInstance(soundRef);
                this._playingSounds[soundRef] = (soundInstance, connectedTransform);

                Vector3 soundPosition = default;
                if (connectedTransform) {
                    soundPosition = connectedTransform.position;
                    var r = soundInstance.set3DAttributes(new FMOD.ATTRIBUTES_3D {
                         forward = connectedTransform.forward.ToFMODVector(),
                         position = soundPosition.ToFMODVector(),
                         up = connectedTransform.up.ToFMODVector()
                    });
                    #if UNITY_EDITOR
                    if (r != RESULT.OK) {
                        Debug.LogError($"Issue settings 3d attributes on sound '{soundRef.Path}': {r}");
                    }
                    #endif
                }

                this.PlayAlreadyStarted(soundRef, soundPosition);

                return soundInstance;
            }
            catch (Exception e) {
                Debug.LogError(e);
            }

            return default;
        }

        public void PlayAlreadyStarted(EventReference soundRef, Vector3 soundPosition) {
            try {
                if (!this._playingSounds.TryGetValue(soundRef, out var sound)) {
                    Debug.LogWarning($"Tried to get a sound that is not playing anymore.");
                    return;
                }

                if (!sound.instance.isValid()) {
                    Debug.LogError($"Tried to play a invalid sound instance.");
                    return;
                }

                this.CStartCoroutine(PlaySoundRoutine(sound.instance, soundPosition));
            }
            catch (Exception e) {
                Debug.LogError(e);
            }
        }

        public void StopPlaying(EventReference soundRef, STOP_MODE stopMode = STOP_MODE.IMMEDIATE) {
            try {
                if (!this._playingSounds.TryGetValue(soundRef, out var sound)) {
                    Debug.LogWarning($"Tried to get a sound that is not playing anymore.");
                    return;
                }

                if (!this._playingSounds.Remove(soundRef)) {
                    Debug.LogError($"Issue trying to remove sound ref from '{nameof(this._playingSounds)}'");
                }
                
                var r = sound.instance.stop(stopMode);
                #if UNITY_EDITOR
                if (r != RESULT.OK) {
                    Debug.LogError($"Issue when trying to stop sound: {r}");
                }
                #endif

                var rr = sound.instance.release();
                #if UNITY_EDITOR
                if (rr != RESULT.OK) {
                    Debug.LogError($"Issue when trying to release sound '{soundRef.Path}': {r}");
                }
                #endif
            }
            catch (Exception e) {
                Debug.LogError(e);
            }
        }

        /// <summary>
        /// Requisitions are Retained and Released.
        /// </summary>
        public void RequestPauseState(EventReference soundRef, bool shouldPause) {
            try {
                if (!this._playingSounds.TryGetValue(soundRef, out var sound)) {
                    Debug.LogWarning($"Tried to get a sound that is not playing anymore.");
                    return;
                }
                
                if (!sound.instance.isValid()) {
                    Debug.LogError($"Sound instance '{soundRef}' is not valid");
                    return;
                }

                // analyse pause
                if (!this._pausedSounds.ContainsKey(soundRef)) this._pausedSounds[soundRef] = new CRetainable();

                var retainable = this._pausedSounds[soundRef];
                if (shouldPause) {
                    retainable.Retain(soundRef);
                }
                else {
                    retainable.Release(soundRef);
                }
                
                var r = sound.instance.setPaused(retainable.IsRetained);
                #if UNITY_EDITOR
                if (r != RESULT.OK) {
                    Debug.LogError($"Issue when trying to pause sound: {r}");
                }
                #endif
                return;
            }
            catch (Exception e) {
                Debug.LogError(e);
            }
        }

        public void SetParameter(EventReference soundRef, string paramName, float value) {
            try {
                if (!this._playingSounds.TryGetValue(soundRef, out var sound)) {
                    Debug.LogWarning($"Tried to get a sound that is not playing anymore.");
                    return;
                }
                
                if (!sound.instance.isValid()) {
                    Debug.LogError($"Sound instance '{soundRef}' is not valid");
                    return;
                }

                var r = sound.instance.setParameterByName(paramName, value);
                #if UNITY_EDITOR
                if (r != RESULT.OK) {
                    Debug.LogError($"Issue when trying to set sound parameter '{paramName}' to '{value}' from '{soundRef.Path}': {r}");
                }
                #endif
            }
            catch (Exception e) {
                Debug.LogError(e);
            }
        }
        
        public float GetParameter(EventReference soundRef, string paramName) {
            try {
                if (!this._playingSounds.TryGetValue(soundRef, out var sound)) {
                    Debug.LogWarning($"Tried to get a sound that is not playing anymore.");
                    return default;
                }
                
                if (!sound.instance.isValid()) {
                    Debug.LogError($"Sound instance '{soundRef}' is not valid");
                    return default;
                }

                var r = sound.instance.getParameterByName(paramName, out float value);
                #if UNITY_EDITOR
                if (r != RESULT.OK) {
                    Debug.LogError($"Issue when trying to get sound parameter '{paramName}' from '{soundRef.Path}': {r}");
                }
                #endif
                return value;
            }
            catch (Exception e) {
                Debug.LogError(e);
            }

            return default;
        }

        public bool IsPlaying(EventReference soundRef) {
            try {
                if (!this._playingSounds.TryGetValue(soundRef, out var sound)) {
                    return false;
                }
                
                if (!sound.instance.isValid()) {
                    return false;
                }

                var r = sound.instance.getPlaybackState(out var state);
                #if UNITY_EDITOR
                if (r != RESULT.OK) {
                    Debug.LogError($"Issue when trying to get if sound '{soundRef.Path}' is playing: {r}");
                }
                #endif
                return state == PLAYBACK_STATE.PLAYING;
            }
            catch (Exception e) {
                Debug.LogError(e);
            }

            return false;
        }
        
        #endregion <<---------- General ---------->>



        #region <<---------- Distance Based ---------->>

         
        #if FMOD
        private IEnumerator PlaySoundRoutine(EventInstance eventInstance, Vector3 point) {
            if (point == default || MainListener == null) {
                this.StartEventInstance(eventInstance);
                yield break;
            }
            
            var distance = Vector3.Distance(point, MainListener.transform.position);
            var factor = (float)(SoundSpeedInKm * 0.000001d);

            var delaySeconds = distance * factor;

            if (delaySeconds.CImprecise() <= 0f) {
                this.StartEventInstance(eventInstance);
                yield break;
            }
            
            Debug.Log($"Distance from listener: '{distance}', delay will be '{delaySeconds}' seconds", this);
            
            yield return new WaitForSeconds(delaySeconds);

            this.StartEventInstance(eventInstance);
        }

        void StartEventInstance(EventInstance eventInstance) {
            var r = eventInstance.start();
            #if UNITY_EDITOR
            if (r != RESULT.OK) {
                Debug.LogError($"Issue starting to start (play) sound: {r}");
            }
            #endif
        }
        #endif

        #endregion <<---------- Distance Based ---------->>

       
    }
}