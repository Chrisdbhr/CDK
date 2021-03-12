using System;
using TMPro;
using UnityEngine;

namespace CDK {
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class CSetGameObjectNameFromTMProInputTextEditor : MonoBehaviour {

		#if UNITY_EDITOR
		
		[SerializeField] private GameObject _goToSetName;
		[NonSerialized] private TextMeshProUGUI _tmproText;

		
		
		public void OnValidate() {
			this.Trigger();
		}

		private void Trigger() {
			if (this._tmproText == null) this._tmproText = this.GetComponent<TextMeshProUGUI>();
			if (this._tmproText.text.CIsNullOrEmpty()) return;
			if (this._goToSetName != null && this._goToSetName.name != this._tmproText.text) {
				this._goToSetName.name = this._tmproText.text;
				Debug.Log($"{nameof(CSetGameObjectNameFromTMProInputTextEditor)} set name '{this._tmproText.text}'", this._goToSetName);
			}
		}
		
		#endif
	}
}
