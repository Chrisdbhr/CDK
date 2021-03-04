using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK {
	public class CSceneEntryPoint : MonoBehaviour {
		
		public int Number {
			get { return this._number; }
		}
		[SerializeField] private int _number;
		
		
		
		#if UNITY_EDITOR
		private void OnDrawGizmos() {
			Handles.color = Color.white;
			var pos = this.transform.position;
			//DebugExtension.DrawArrow(pos, this.transform.forward, Color.white);
			Handles.Label(pos, $"Entry point {this._number}");
		}
		#endif
	}
}
