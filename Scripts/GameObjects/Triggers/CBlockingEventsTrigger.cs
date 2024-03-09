using R3;
using UnityEngine;

namespace CDK {
    public class CBlockingEventsTrigger : MonoBehaviour {

        [SerializeField] private CUnityEventBool AnyBlockingEvent;
        [SerializeField] private CUnityEventBool OnMenuOrPlayingCutsceneEvent;
        [Header("Individual")]
        [SerializeField] private CUnityEventBool OnMenuEvent;
        [SerializeField] private CUnityEventBool PlayingCutsceneEvent;
        [SerializeField] private CUnityEventBool LimitingPlayerActionsEvent;
        [Header("Inverted")]
        [SerializeField] private CUnityEventBool NotOnMenuEvent;
        [SerializeField] private CUnityEventBool NotPlayingCutsceneEvent;
        [SerializeField] private CUnityEventBool NotLimitingPlayerActionsEvent;

        CompositeDisposable _disposables;

        private void Awake() {
            _disposables = new CompositeDisposable();

            var b = CBlockingEventsManager.get;

            OnBlockingEvent(b.IsOnMenu, b.IsPlayingCutscene, b.IsLimitingPlayerActions);

            Observable.CombineLatest(
                b.OnMenuRetainable.IsRetainedAsObservable(),
                b.PlayingCutsceneRetainable.IsRetainedAsObservable(),
                b.LimitPlayerActionsRetainable.IsRetainedAsObservable()
            )
            .Subscribe(x => OnBlockingEvent(x[0], x[1], x[2]))
            .AddTo(_disposables);

        }

        private void OnBlockingEvent(bool isOnMenu, bool isPlayingCutscene, bool limitPlayerActions) {
            AnyBlockingEvent.Invoke(isOnMenu || isPlayingCutscene || limitPlayerActions);
            OnMenuOrPlayingCutsceneEvent.Invoke(isOnMenu || isPlayingCutscene);
            OnMenuEvent.Invoke(isOnMenu);
            PlayingCutsceneEvent.Invoke(isPlayingCutscene);
            LimitingPlayerActionsEvent.Invoke(limitPlayerActions);

            // inverted
            NotOnMenuEvent.Invoke(!isOnMenu);
            NotPlayingCutsceneEvent.Invoke(!isPlayingCutscene);
            NotLimitingPlayerActionsEvent.Invoke(!limitPlayerActions);
        }

        private void OnDestroy() {
            _disposables?.Dispose();
        }
    }
}