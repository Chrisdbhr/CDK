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
			var color = Color.cyan;
			var tranf = this.transform;
			var fwd = tranf.forward;
			var pos = tranf.position;
			
			// gizmo
			Gizmos.color = color;
			Gizmos.DrawWireSphere(pos, 0.5f);
			Gizmos.DrawLine(pos, pos + fwd);
			
			// text
			Handles.color = color;
			Handles.Label(pos, $"Entry point {this._number}");
		}
		#endif
	}
}
