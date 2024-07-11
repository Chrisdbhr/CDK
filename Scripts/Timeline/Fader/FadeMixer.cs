using UnityEngine;
using UnityEngine.Playables;

namespace CDK.Timeline {
    public class FadeMixer : PlayableBehaviour {

        FadeController _fadeController;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
            base.ProcessFrame(playable, info, playerData);

            _fadeController = playerData as FadeController;
            if (_fadeController == null) return;

            var count = playable.GetInputCount();

            var targetColor = Color.clear;
            float targetAlpha = 0f;
            for (var i = 0; i < count; i++) {
                var input = playable.GetInput(i);
                var weight = playable.GetInputWeight(i);

                if (!input.IsValid() || input.GetPlayState() != PlayState.Playing) continue;

                var data = ((ScriptPlayable<FadeDataPlayable>) input).GetBehaviour();
                if (data == null) continue;

                targetAlpha = Mathf.Max(targetAlpha, (data.MaxAlpha * weight));
                targetColor = targetColor == Color.clear ? data.Color : Color.Lerp(targetColor, data.Color, weight);
            }

            _fadeController.SetColor(targetColor == Color.clear ? Color.black : targetColor, targetAlpha);
        }

        public override void OnGraphStop (Playable playable) {
            if (_fadeController == null) return;
            _fadeController.SetColor(Color.black, 0f);
        }
    }
}