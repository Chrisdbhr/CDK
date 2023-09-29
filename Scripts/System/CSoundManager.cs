using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if FMOD
using FMOD;
using FMOD.Studio;
using FMODUnity;
using STOP_MODE = FMOD.Studio.STOP_MODE;
#endif
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace CDK {
    public class CSoundManager : MonoBehaviour {

        #region <<---------- Singleton ---------->>

        public static CSoundManager get { 
            get{
                if (CSingletonHelper.CannotCreateAnyInstance() || _instance != null) return _instance;
                return (_instance = CSingletonHelper.CreateInstance<CSoundManager>("Sound Manager"));
            }
        }
        private static CSoundManager _instance;

        #endregion <<---------- Singleton ---------->>




        #region <<---------- Enum ---------->>

        struct PlayingSoundData {
            public EventInstance instance { get; }
            public bool is3d { get; }
            public Transform connectedTransform { get; }
            public bool autoPauseManagment { get; }

            public PlayingSoundData(EventInstance instance, bool is3d, Transform connectedTransform, bool autoPauseManagment) {
                this.instance = instance;
                this.is3d = is3d;
                this.connectedTransform = connectedTransform;
                this.autoPauseManagment = autoPauseManagment;
            }
        }


        #endregion <<---------- Enum ---------->>

        
        
        #region <<---------- Properties and Fields ---------->>

        [SerializeField, Min(0.0015f)] float _occlusionPowerByDistance = 0.0025f;
        [SerializeField] bool _debug;
        LayerMask _occlusionLayerMask = 1;
        RaycastHit[] _raycastHitResults = new RaycastHit[1024];

        private AudioSource OneShotAudioSource;
        
#if FMOD
        public static StudioListener MainListener {
            get {
                if (mainListener == null) {
                    mainListener = GameObject.FindObjectOfType<StudioListener>();
                }
                return mainListener;
            }
            set {
                mainListener = value;
            }
        }
        private static StudioListener mainListener;
#endif

        public const float SoundSpeedInKm = 1224f;

#if FMOD
        private Dictionary<EventReference, PlayingSoundData> _playingUniqueSounds;
        private Dictionary<PlayingSoundData, CRetainable> _pausedSounds;
        private List<PlayingSoundData> _playingSounds;
#endif
        private const string OcclusionParameter = "Occlusion";
        private const float OcclusionComputingInterval = 0.1f;
        private float _timeLastOcclusionCalculation;

        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- Mono Behaviour ---------->>

        private void Awake() {
            DontDestroyOnLoad(this);
            this.OneShotAudioSource = this.gameObject.AddComponent<AudioSource>();
            this.OneShotAudioSource.playOnAwake = false;
            #if FMOD
            this._playingUniqueSounds = new ();
            this._pausedSounds = new ();
            this._playingSounds = new ();
            #endif
        }

        private void OnEnable() {
            CTime.OnTimePaused += this.OnTimePaused;

            #if FMOD
            StudioEventEmitter.OnInstancePlay += this.StudioEventOnInstancePlay;
            StudioEventEmitter.OnInstanceStop += this.StudioEventOnInstanceStop;
            #endif
        }

        private void OnDisable() {
            CTime.OnTimePaused -= this.OnTimePaused;

            #if FMOD
            StudioEventEmitter.OnInstancePlay -= this.StudioEventOnInstancePlay;
            StudioEventEmitter.OnInstanceStop -= this.StudioEventOnInstanceStop;
            #endif
        }

#if FMOD
        private void Update() {
            this.ComputeOcclusion();            
        }

        private void LateUpdate(){
            foreach (var playingSound in this._playingUniqueSounds) {
                if (playingSound.Value.connectedTransform == null || !playingSound.Value.instance.isValid()) {
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

#if UNITY_EDITOR
        private Dictionary<StudioEventEmitter, RaycastHit[]> debugRayHits = new Dictionary<StudioEventEmitter,  RaycastHit[]>();
        private void OnDrawGizmosSelected() {
            foreach (var hits in this.debugRayHits.Values) {
                for (int i = 0; i < hits.Length; i ++) {
                    if (i + 1 >= hits.Length) break;
                    
                    Debug.DrawLine(hits[i].point, hits[i + 1].point, new Color(i * 0.01f,i * 0.25f,i * 0.01f), 1f, true);
                }
            }
        }
#endif
#endif

        #endregion <<---------- Mono Behaviour ---------->>




        #region <<---------- General ---------->>

        private void OnTimePaused(bool paused) {
            foreach (var playingSound in this._playingUniqueSounds.Where(playingSound => playingSound.Value.autoPauseManagment)) {
                this.RequestPauseState(playingSound.Key, paused);
            }
        }

#if FMOD
        /// <summary>
        /// Play an event that will be replaced if requested to play again.
        /// </summary>
        public EventInstance PlaySingletonEvent(EventReference soundRef, Transform connectedTransform = null, bool autoPauseManagment = true) {
            try{
                if (this._playingUniqueSounds.ContainsKey(soundRef)) {
#if UNITY_EDITOR
                    if (this._debug) {
                        Debug.LogWarning($"Replacing already created sound '{soundRef}'");
                    }
#endif
                    if(this._playingUniqueSounds[soundRef].instance.isValid()) {
                        this._playingUniqueSounds[soundRef].instance.stop(STOP_MODE.IMMEDIATE);
                        this._playingUniqueSounds[soundRef] = default;
                    }
                }
                
                var soundInstance = RuntimeManager.CreateInstance(soundRef);

                var getDescriptionResult = soundInstance.getDescription(out var description);
                if (getDescriptionResult != RESULT.OK) {
                    if(_debug) Debug.LogWarning($"Issue getting sound '{soundRef}' description: {getDescriptionResult}");
                }

                var getIs3dResult = description.is3D(out var is3d);
                if (getIs3dResult != RESULT.OK) {
                    if(_debug) Debug.LogWarning($"Issue getting if sound '{soundRef}' is 3D: {getDescriptionResult}");
                }
                
                is3d = (is3d && connectedTransform != null);

                this._playingUniqueSounds[soundRef] = new PlayingSoundData (soundInstance, is3d, connectedTransform, autoPauseManagment);

                Vector3 soundPosition = default;
                if (is3d) {
                    soundPosition = connectedTransform.position;
                    var r = soundInstance.set3DAttributes(new FMOD.ATTRIBUTES_3D {
                         forward = connectedTransform.forward.ToFMODVector(),
                         position = soundPosition.ToFMODVector(),
                         up = connectedTransform.up.ToFMODVector()
                    });
                    if (r != RESULT.OK) {
#if UNITY_EDITOR
                        Debug.LogError($"Issue settings 3d attributes on sound '{soundRef.Path}': {r}");
#endif
                    }
                    this.CStartCoroutine(this.PlaySoundByDistanceRoutine(soundInstance, soundPosition));
                }
                else {
                    this.StartEventInstance(soundInstance);   
                }

                return soundInstance;
            }
            catch (Exception e) {
                Debug.LogError(e);
            }

            return default;
        }

        public void StopPlaying(EventReference soundRef, STOP_MODE stopMode = STOP_MODE.IMMEDIATE) {
            try {
                if (!this._playingUniqueSounds.TryGetValue(soundRef, out var sound)) {
                    if(_debug) Debug.LogWarning($"Tried to get a sound that is not playing anymore.");
                    return;
                }

                if (!this._playingUniqueSounds.Remove(soundRef)) {
                    Debug.LogError($"Issue trying to remove sound ref from '{nameof(this._playingUniqueSounds)}'");
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
        /// Requests are Retained and Released.
        /// </summary>
        public void RequestPauseState(EventReference eventRef, bool shouldPause) {
            try {
                var data = this.GetUniqueSoundDataFromEventReference(eventRef);

                // analyse pause
                if (!this._pausedSounds.ContainsKey(data)) this._pausedSounds[data] = new CRetainable();

                var retainable = this._pausedSounds[data];
                if (shouldPause) {
                    retainable.Retain(eventRef);
                }
                else {
                    retainable.Release(eventRef);
                }
                
                var r = data.instance.setPaused(retainable.IsRetained);
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
                if (!this._playingUniqueSounds.TryGetValue(soundRef, out var sound)) {
                    if(_debug) Debug.LogWarning($"Tried to get a sound that is not playing anymore.");
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
                if (!this._playingUniqueSounds.TryGetValue(soundRef, out var sound)) {
                    if(_debug) Debug.LogWarning($"Tried to get a sound that is not playing anymore.");
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

        public bool IsUniquePlaying(EventReference soundRef) {
            try {
                if (!this._playingUniqueSounds.TryGetValue(soundRef, out var sound)) {
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

        private PlayingSoundData GetUniqueSoundDataFromEventReference(EventReference eventReference) {
            if (!this._playingUniqueSounds.TryGetValue(eventReference, out var sound)) {
                Debug.LogError($"Tried to get a sound that is not playing anymore.");
                return default;
            }

            if (!sound.instance.isValid()) {
                Debug.LogError($"Sound instance '{eventReference}' is not valid");
                return default;
            }

            return sound;

        }
#endif

        #endregion <<---------- General ---------->>



        
        #region <<---------- Unity Audio ---------->>

        public void PlayOneShot(AudioClip audioClip, float volume = 1f) {
            this.OneShotAudioSource.Stop();
            if (audioClip == null) return;
            this.OneShotAudioSource.PlayOneShot(audioClip, volume);
        }

        #endregion <<---------- Unity Audio ---------->>

        


        #region <<---------- Distance Based ---------->>

#if FMOD
        private IEnumerator PlaySoundByDistanceRoutine(EventInstance eventInstance, Vector3 point) {
            if (point == default || MainListener == null) {
                this.StartEventInstance(eventInstance);
                yield break;
            }
            
            var distance = Vector3.Distance(point, MainListener.transform.position);
            var factor = (float)(SoundSpeedInKm * 0.000001d);

            var delaySeconds = (distance * factor).CImprecise();

            if (delaySeconds < 0.01f) {
                this.StartEventInstance(eventInstance);
                yield break;
            }
            
            if(this._debug) Debug.Log($"Distance from listener: '{distance}', delay will be '{delaySeconds}' seconds", this);
            
            yield return new WaitForSeconds(delaySeconds);

            this.StartEventInstance(eventInstance);
        }

        void StartEventInstance(EventInstance eventInstance) {
            if (!eventInstance.isValid()) return;
            var r = eventInstance.start();
#if UNITY_EDITOR
            if (r != RESULT.OK) {
                Debug.LogError($"Issue starting to start (play) sound: {r}");
            }
#endif
        }
#endif

        #endregion <<---------- Distance Based ---------->>




        #region <<---------- Occlusion ---------->>

        private void ComputeOcclusion() {
#if FMOD
            if (this._timeLastOcclusionCalculation + OcclusionComputingInterval > Time.time) {
                return;
            }
            this._timeLastOcclusionCalculation = Time.time;
            
            if (MainListener == null) return;

            var originalQueriesHitBackfaces = Physics.queriesHitBackfaces;
            Physics.queriesHitBackfaces = true;

            var listenerTransform = mainListener.transform;

            foreach (var activeEmitter in StudioEventEmitter.activeEmitters) {
                if (!activeEmitter.EventInstance.isValid()) continue;
                if (activeEmitter.EventInstance.getParameterByName(OcclusionParameter, out var currentValue, out var maxValue) != RESULT.OK) continue;
                if (activeEmitter.EventInstance.getDescription(out EventDescription description) != RESULT.OK) continue;
                if (description.is3D(out var is3d) != RESULT.OK || !is3d) continue;
                if (description.getMinMaxDistance(out var minDistance, out var maxDistance) != RESULT.OK) continue;
                var emitterPos = activeEmitter.transform.position;

                var hitsAmount = Physics.RaycastNonAlloc(
                    emitterPos,
                    listenerTransform.position - emitterPos,
                    this._raycastHitResults,
                    maxDistance,
                    this._occlusionLayerMask,
                    QueryTriggerInteraction.Ignore
                );

#if UNITY_EDITOR
                debugRayHits[activeEmitter] = this._raycastHitResults.Take(hitsAmount).ToArray();
#endif
                
                var newOcclusionValue = GetOcclusionValue(hitsAmount);
                activeEmitter.EventInstance.setParameterByName(OcclusionParameter, newOcclusionValue);
            }
            
            Physics.queriesHitBackfaces = originalQueriesHitBackfaces;
#endif
        }

        private float GetOcclusionValue(int hitsAmount) {
            float occlusion = 0.0f;

            if (this._raycastHitResults.Length <= 0) {
                return occlusion;
            }
            
            for (int i = 0; i < hitsAmount; i += 2) {
                if (i >= hitsAmount || i + 1 >= hitsAmount) break;
                occlusion += Vector3.Distance(this._raycastHitResults[i].point, this._raycastHitResults[i + 1].point) * this._occlusionPowerByDistance;
            }

            return occlusion;
        }

        #endregion <<---------- Occlusion ---------->>



        #region <<---------- FMOD Studio Event Emitter Monitoring ---------->>

        #if FMOD

        private void StudioEventOnInstancePlay(EventInstance eventInstance, bool is3d, Transform connectedTransform) {
            var data = new PlayingSoundData (eventInstance, is3d, connectedTransform, true);
            this._playingSounds.Add(data);
        }

        private void StudioEventOnInstanceStop(EventInstance eventInstance) {
            if (CApplication.IsQuitting) return;
            if (eventInstance.handle == default) {
                Debug.LogError($"Notified a sound stopped but the handle is default.");
                return;
            }
            var data = this._playingSounds.FirstOrDefault(d => d.instance.handle == eventInstance.handle);
            this._playingSounds.Remove(data);
        }

        #endif

        #endregion <<---------- FMOD Studio Event Emitter Monitoring ---------->>
    }
}