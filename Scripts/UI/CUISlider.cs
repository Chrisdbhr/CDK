using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CDK.UI {
	[RequireComponent(typeof(Slider))]
	public class CUISlider : CUIInteractable {

		public Slider Slider;
		[SerializeField] private TextMeshProUGUI _valueTmp;
		[SerializeField] private string _toStringParams = "0";
		public Action<float> OnValueChanged;

		private void OnEnable() { 
			this.Slider.onValueChanged.AddListener(this.SliderValueChanged);
		}
		
		private void OnDisable() { 
			this.Slider.onValueChanged.RemoveListener(this.SliderValueChanged);
		}

		private void SliderValueChanged(float newValue) {
			if (newValue == this.Slider.value) return;
			if (this._valueTmp) this._valueTmp.text = newValue.ToString(this._toStringParams);
			this.OnValueChanged?.Invoke(newValue);
		}
	}
}
