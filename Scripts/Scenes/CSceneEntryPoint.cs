using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK {
	public class CSceneEntryPoint : MonoBehaviour {
		
		[SerializeField] private int _number;
		private float _radius = 0.2f;



		public static Transform GetSceneEntryPointTransform(int entryPointNumber) {
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
			var color = Color.cyan;
			var tranf = this.transform;
			var fwd = tranf.forward;
			var pos = tranf.position;
			
			// gizmo
			Gizmos.color = color;
			Gizmos.DrawWireSphere(pos + Vector3.up * _radius, _radius);
			Gizmos.DrawLine(pos, pos + fwd);
			
			// text
			Handles.color = color;
			Handles.Label(pos, $"Entry point {this._number}");
		}
		#endif
	}
}
