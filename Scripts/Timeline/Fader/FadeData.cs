using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CDK.Timeline {

    [System.Serializable]
    public class FadeDataPlayable : PlayableBehaviour {
        [Range(0f,1f)] public float MaxAlpha = 1f;
        [Tooltip("Alpha is ignored, not blended")] public Color Color = Color.black;
    }

    [System.Serializable]
    public class FadeData : PlayableAsset, ITimelineClipAsset {

        public FadeDataPlayable data = new();

        public override Playable CreatePlayable (PlayableGraph graph, GameObject owner) {
            return ScriptPlayable<FadeDataPlayable>.Create(graph, data);
        }

        public ClipCaps clipCaps => ClipCaps.Blending;

    }
}