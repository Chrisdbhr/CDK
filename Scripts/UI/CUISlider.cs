using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CDK.UI {
	[RequireComponent(typeof(Slider))]
	public class CUISlider : CUIInteractable, IMoveHandler {

		public Slider Slider;
		[SerializeField] private TextMeshProUGUI _valueTmp;
		[SerializeField] private string _toStringParams = "0";

		private void OnEnable() { 
			this.Slider.onValueChanged.AddListener(this.SliderValueChanged);
		}
		
		private void OnDisable() { 
			this.Slider.onValueChanged.RemoveListener(this.SliderValueChanged);
		}

		private void SliderValueChanged(float newValue) {
			if (this._valueTmp) this._valueTmp.text = newValue.ToString(this._toStringParams);
		}

		public void OnMove(AxisEventData eventData) {
			this.Slider.value += eventData.moveVector.x * Time.deltaTime;
		}
	}
}
