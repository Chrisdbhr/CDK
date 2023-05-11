using System;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK {
	public class CSceneEntryPoint : MonoBehaviour {
		
		[SerializeField] private int _number;
        [SerializeField] private CUnityEventBool _chosenEntryPoint;
		#if UNITY_EDITOR
		private float _editorRadius = 0.2f;
		#endif
        private bool _isSelectedEntryPoint;


 

        private void OnEnable() {
            this._chosenEntryPoint?.Invoke(this._isSelectedEntryPoint);
        }
        
        public void SetIsSelected() {
            this._isSelectedEntryPoint = true;
            this._chosenEntryPoint?.Invoke(this._isSelectedEntryPoint);
        }
        
        public static CSceneEntryPoint GetSceneEntryPointByNumber(int entryPointNumber) {
			var sceneEntryPoints = GameObject.FindObjectsOfType<CSceneEntryPoint>();
			if (!sceneEntryPoints.Any() || entryPointNumber >= sceneEntryPoints.Length) {
				Debug.LogWarning($"Cant find any level entry point {entryPointNumber} OR it is invalid. Spawning in 0,0,0");
			}
			else {
				var selectedEntryPoint = sceneEntryPoints.Where(ep => ep._number == entryPointNumber).ToList();
				if (selectedEntryPoint.CIsNullOrEmpty()) return null;
				var selected = selectedEntryPoint.CRandomElement();
				if (selectedEntryPoint.Count > 1) {
					Debug.LogWarning($"There was more than one scene entry point with the number '{entryPointNumber}', will select a random one. (selected '{selected.name}'", selected);
				}
				return selected;
			}
			return null;
		}

        public static (Vector3 pos, Quaternion rot) GetEntryPointPositionAndRotation(int entryPointNumber) {
            var entry = GetSceneEntryPointByNumber(entryPointNumber);
            if (entry == null) {
                return default;
            }
            return (entry.transform.position, entry.transform.rotation);
        }
		
		
		#if UNITY_EDITOR
		private void OnDrawGizmos() {
			var transf = this.transform;
			var fwd = transf.forward;
			var pos = transf.position;
			pos += Vector3.up * this._editorRadius;
			
			// gizmo
			Gizmos.color = new Color(0f,1f,1f,0.33f);
			Gizmos.DrawSphere(pos, this._editorRadius * 2f);
			Gizmos.DrawLine(pos, pos + (fwd * _editorRadius * 2f));

			Handles.color = new Color(0.9f, 0.1f,0.6f);
			Handles.Slider(pos + (fwd * _editorRadius * 2f), fwd, (_editorRadius * .5f), Handles.ConeHandleCap, 0f);
			
			// text
			Handles.Label(pos - (Vector3.up*0.5f), $"Entry point {this._number}");
            
            // Ground Check Distance
            pos = this.transform.position;
            var maxDistance = 11.20f;
            var hit = Physics.Raycast(pos, Vector3.down, out var hitInfo,maxDistance, 1);
            Debug.DrawLine(pos, pos + (Vector3.down * maxDistance), Color.red);
            if(hit) Debug.DrawLine(pos, hitInfo.point, Color.cyan);
		}

		private void Reset() {
			this.name = nameof(CSceneEntryPoint);
		}
		#endif
	}
}
