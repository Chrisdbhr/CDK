using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CDK.Timeline {
    [TrackClipType(typeof(FadeData))]
    [TrackBindingType(typeof(FadeController))]
    [TrackColor(0f,0f,0f)]
    public class FadeTrack : TrackAsset {

        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount) {
            return ScriptPlayable<FadeMixer>.Create(graph, inputCount);
        }

    }
}