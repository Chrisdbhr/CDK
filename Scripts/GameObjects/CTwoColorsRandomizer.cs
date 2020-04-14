using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK {
	public class CTwoColorsRandomizer : MonoBehaviour {

		[SerializeField] private Color _colorOne = Color.magenta;
		[SerializeField] private Color _colorTwo = Color.blue;

		[SerializeField] private CUnityEventColor _colorEvent;

		public void RandomizeColorsAndTriggerEvent() {
			this._colorEvent?.Invoke(new Color(
				Random.Range(this._colorOne.r, this._colorTwo.r), 
				Random.Range(this._colorOne.g, this._colorTwo.g), 
				Random.Range(this._colorOne.b, this._colorTwo.b)
			));
		}
	}
	
	#if UNITY_EDITOR
	[CustomEditor(typeof(CTwoColorsRandomizer))]
	public class CTwoColorsRandomizerEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			this.DrawDefaultInspector();
        
			var myScript = (CTwoColorsRandomizer)this.target;
			if(GUILayout.Button(nameof(CTwoColorsRandomizer.RandomizeColorsAndTriggerEvent)))
			{
				myScript.RandomizeColorsAndTriggerEvent();
			}
		}
	}
	#endif
}