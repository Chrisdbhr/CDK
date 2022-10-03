using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK {
	public class CSceneEntryPoint : MonoBehaviour {
		
		[SerializeField] private int _number;
		#if UNITY_EDITOR
		private float _editorRadius = 0.2f;
		#endif
		



		public static Transform GetSceneEntryPointTransformByNumber(int entryPointNumber) {
			var sceneEntryPoints = GameObject.FindObjectsOfType<CSceneEntryPoint>();
			if (!sceneEntryPoints.Any() || entryPointNumber >= sceneEntryPoints.Length) {
				Debug.LogWarning($"Cant find any level entry point {entryPointNumber} OR it is invalid.");
			}
			else {
				var selectedEntryPoint = sceneEntryPoints.Where(ep => ep._number == entryPointNumber).ToList();
				if (selectedEntryPoint.CIsNullOrEmpty()) return null;
				var selected = selectedEntryPoint.CRandomElement();
				if (selectedEntryPoint.Count > 1) {
					Debug.LogWarning($"There was more than one scene entry point with the number '{entryPointNumber}', will select a random one. (selected '{selected.name}'", selected);
				}
				return selected.transform;
			}
			return null;
		}
		
		
		#if UNITY_EDITOR
		private void OnDrawGizmos() {
			var tranf = this.transform;
			var fwd = tranf.forward;
			var pos = tranf.position;
			pos += Vector3.up * this._editorRadius;
			
			// gizmo
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(pos, this._editorRadius * 2f);
			Gizmos.DrawLine(pos, pos + (fwd * _editorRadius * 2f));

			Handles.color = new Color(0.9f, 0.1f,0.6f);
			Handles.Slider(pos + (fwd * _editorRadius * 2f), fwd, (_editorRadius * .5f), Handles.ConeHandleCap, 0f);
			
			// text
			Handles.Label(pos - (Vector3.up*0.5f), $"Entry point {this._number}");
		}

		private void Reset() {
			this.name = nameof(CSceneEntryPoint);
		}
		#endif
	}
}
