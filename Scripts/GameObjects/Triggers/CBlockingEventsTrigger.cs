﻿using R3;
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

        CBlockingEventsManager b;

        void Awake() {
            b = this.gameObject.scene.GetSceneContainer().Resolve<CBlockingEventsManager>();
            OnBlockingEvent(b.IsOnMenu, b.IsPlayingCutscene);

            Observable.CombineLatest(
                b.OnMenuRetainable.IsRetainedAsObservable(),
                b.PlayingCutsceneRetainable.IsRetainedAsObservable()
            )
            .Subscribe(x => OnBlockingEvent(x[0], x[1]))
            .AddTo(this);
        }

        void OnBlockingEvent(bool isOnMenu, bool isPlayingCutscene) {
            AnyBlockingEvent.Invoke(isOnMenu || isPlayingCutscene);
            OnMenuOrPlayingCutsceneEvent.Invoke(isOnMenu || isPlayingCutscene);
            OnMenuEvent.Invoke(isOnMenu);
            PlayingCutsceneEvent.Invoke(isPlayingCutscene);

            // inverted
            NotOnMenuEvent.Invoke(!isOnMenu);
            NotPlayingCutsceneEvent.Invoke(!isPlayingCutscene);
            NotOnMenuAndNotPlayingCutsceneEvent.Invoke(!isOnMenu && !isPlayingCutscene);
        }

    }
}