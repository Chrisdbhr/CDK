using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CDK.UI {
	[RequireComponent(typeof(Slider))]
	public class CUISlider : CUIInteractable, IMoveHandler {

		public Slider Slider;
        [SerializeField, Min(0.001f)] private float _inputMovementMultiplier = 0.5f;
		[SerializeField] private TextMeshProUGUI _valueTmp;
		[SerializeField] private string _toStringParams = "0";

        protected override void OnEnable() { 
            base.OnEnable();
			this.Slider.onValueChanged.AddListener(this.SliderValueChanged);
		}

        protected override void OnDisable() { 
            base.OnDisable();
			this.Slider.onValueChanged.RemoveListener(this.SliderValueChanged);
		}

		private void SliderValueChanged(float newValue) {
			if (this._valueTmp) this._valueTmp.text = newValue.ToString(this._toStringParams);
		}

		public void OnMove(AxisEventData eventData) {
			this.Slider.value += (eventData.moveVector.x * this._inputMovementMultiplier) * Time.deltaTime;
			this.Selected();
		}

		private void Reset() {
			if(!Slider) this.Slider = this.GetComponent<Slider>();
		}

	}
}