using System;
using Reflex.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace CDK {
    public class CBlockingEventsTrigger : MonoBehaviour {

        #if ODIN_INSPECTOR
        [FoldoutGroup("Default")]
        #else
        [Header("Default")]
        #endif
        [SerializeField] CUnityEventBool AnyBlockingEvent;
        #if ODIN_INSPECTOR
        [FoldoutGroup("Default")]
        #endif
        [SerializeField] CUnityEventBool OnMenuOrPlayingCutsceneEvent;

        #if ODIN_INSPECTOR
        [FoldoutGroup("Individual")]
        #else
        [Header("Individual")]
        #endif
        [SerializeField] CUnityEventBool OnMenuEvent;
        #if ODIN_INSPECTOR
        [FoldoutGroup("Individual")]
        #endif
        [SerializeField] CUnityEventBool PlayingCutsceneEvent;

        #if ODIN_INSPECTOR
        [FoldoutGroup("Inverted")]
        #else
        [Header("Inverted")]
        #endif
        [SerializeField] CUnityEventBool NotOnMenuEvent;
        #if ODIN_INSPECTOR
        [FoldoutGroup("Inverted")]
        #endif
        [SerializeField] CUnityEventBool NotPlayingCutsceneEvent;
        #if ODIN_INSPECTOR
        [FoldoutGroup("Inverted")]
        #endif
        [FormerlySerializedAs("NotOnMenuOrNotPlayingCutsceneEvent")] [SerializeField] CUnityEventBool NotOnMenuAndNotPlayingCutsceneEvent;

        [NonSerialized] CBlockingEventsManager _blockingEventsManager;

        void Awake() {
            _blockingEventsManager = gameObject.scene.GetSceneContainer().Resolve<CBlockingEventsManager>();
            BlockingEvent(_blockingEventsManager.InMenuOrPlayingCutscene);
        }

        void OnEnable()
        {
            _blockingEventsManager.InMenuOrPlayingCutsceneEvent += BlockingEvent;
        }

        void OnDisable()
        {
            _blockingEventsManager.InMenuOrPlayingCutsceneEvent -= BlockingEvent;
        }

        void BlockingEvent(bool inMenuOrPlayingCutscene) {
            var isInMenu = _blockingEventsManager.IsInMenu;
            var isPlayingCutscene = _blockingEventsManager.IsPlayingCutscene;

            AnyBlockingEvent.Invoke(inMenuOrPlayingCutscene);
            OnMenuOrPlayingCutsceneEvent.Invoke(isInMenu || isPlayingCutscene);
            OnMenuEvent.Invoke(isInMenu);
            PlayingCutsceneEvent.Invoke(isPlayingCutscene);

            // inverted
            NotOnMenuEvent.Invoke(!isInMenu);
            NotPlayingCutsceneEvent.Invoke(!isPlayingCutscene);
            NotOnMenuAndNotPlayingCutsceneEvent.Invoke(!isInMenu && !isPlayingCutscene);
        }

    }
}