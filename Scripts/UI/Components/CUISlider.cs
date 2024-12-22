using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CDK.UI {
	[RequireComponent(typeof(Slider))]
	public class CUISlider : CUIInteractable, IMoveHandler {

		public Slider Slider;
        [SerializeField, Min(0.001f)] float _inputMovementMultiplier = 0.5f;
		[SerializeField] TextMeshProUGUI _valueTmp;
		[SerializeField] string _toStringParams = "0";

        protected override void OnEnable() { 
            base.OnEnable();
			Slider.onValueChanged.AddListener(SliderValueChanged);
		}

        protected void OnDisable() {
			Slider.onValueChanged.RemoveListener(SliderValueChanged);
		}

		void SliderValueChanged(float newValue) {
			if (_valueTmp) _valueTmp.text = newValue.ToString(_toStringParams);
		}

		public void OnMove(AxisEventData eventData) {
			Slider.value += (eventData.moveVector.x * _inputMovementMultiplier) * Time.deltaTime;
			Selected();
		}

		void Reset() {
			if(!Slider) Slider = GetComponent<Slider>();
		}

	}
}