using System;
using UnityEngine;
using Unity.Linq;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK {
	public class CChildrenPositionAndRotationRandomizer : MonoBehaviour {
		
		#region <<---------- Properties and Fields ---------->>

		[SerializeField] private Vector3 _scaleRange = Vector3.one * 0.10f;
		[SerializeField] private Vector3 _angleRange = Vector3.one * 30;
		[SerializeField] private Vector3 _positionRange = Vector3.one * 10;
		[SerializeField] private float _minimumSpaceBetweenObjs = 1f;
		[NonSerialized] private GameObject[] _childObjs;

		#endregion <<---------- Properties and Fields ---------->>

		
		
		
		#region <<---------- MonoBehaviour ---------->>

		#if UNITY_EDITOR
		private void OnDrawGizmosSelected() {
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireCube(this.transform.position, this._positionRange*2f);
		}
		#endif
		
		#endregion <<---------- MonoBehaviour ---------->>

		#if UNITY_EDITOR
		public void RandomizePositionsEditor() {
			this.DoForEachGameObject(target => {
				var pos = this.GetValidPosition(200);
				Undo.RecordObject(target, $"undo-{target.name}");
				target.position = pos;
			});
		}
		public void RandomizeRotationsEditor() {
			this.DoForEachGameObject(target => {
				var angle = new Vector3(
					Random.Range(-this._angleRange.x, this._angleRange.x),
					Random.Range(-this._angleRange.y, this._angleRange.y),
					Random.Range(-this._angleRange.z, this._angleRange.z)
				);
				Undo.RecordObject(target, $"undo-{target.name}");
				target.rotation = Quaternion.Euler(angle);
			});
		}
		public void RandomizeScalesEditor() {
			this.DoForEachGameObject(target => {
				var scale = target.localScale;
				scale.x += scale.x * this._scaleRange.x;
				scale.y += scale.y * this._scaleRange.y;
				scale.z += scale.z * this._scaleRange.z;
				Undo.RecordObject(target, $"undo-{target.name}");
				target.localScale = scale;
			});
		}
		#endif

		private void DoForEachGameObject(Action<Transform> actionOnTransform) {
			#if UNITY_EDITOR
			string progressBarTitle = "Randomizing Rotations";
			string progressBarInfo = "";
			EditorUtility.DisplayProgressBar(progressBarTitle, progressBarInfo, 0f); 
			#endif


			this._childObjs = this.gameObject.Children().ToArray();
			if (this._childObjs.Length <= 0) return;
			int count = 0;
			foreach (var child in this._childObjs) {
				#if UNITY_EDITOR
				if (EditorUtility.DisplayCancelableProgressBar(progressBarTitle, progressBarInfo, (float)count / this._childObjs.Length)) {
					break;
				}
				#endif
				count++;
				
				actionOnTransform?.Invoke(child.transform);
			}
			#if UNITY_EDITOR
			EditorUtility.ClearProgressBar();
			#endif
		}

		private Vector3 GetValidPosition(int safeOverflowCount) {
			var pos = new Vector3(
				Random.Range(-this._positionRange.x, this._positionRange.x),
				Random.Range(-this._positionRange.y, this._positionRange.y),
				Random.Range(-this._positionRange.z, this._positionRange.z)
			);
			pos += this.transform.position;
			
			if (safeOverflowCount <= 0) {
				Debug.Log($"[CChildrenPositionAndRotationRandomizer] Safe overflow reached. Stopping loop to prevent stack overflow.");
				return pos;
			}

			bool needToRecalculate = false;
			foreach (var createdObj in this._childObjs) {
				if (Vector3.Distance(createdObj.transform.position, pos) <= this._minimumSpaceBetweenObjs) {
					needToRecalculate = true;
					break;
				}
			}

			return needToRecalculate ? this.GetValidPosition(safeOverflowCount - 1) : pos;
		}

	}

	#region <<---------- Editor ---------->>
	
	#if UNITY_EDITOR
	[CustomEditor(typeof(CChildrenPositionAndRotationRandomizer))]
	public class RandomPositionAndRotationInstantiatorEditor : Editor {
		public override void OnInspectorGUI() {
			this.DrawDefaultInspector();

			if (!(target is CChildrenPositionAndRotationRandomizer myScript)) return;
			if(GUILayout.Button(nameof(CTransformExtensions.DeleteAllChildren))) {
				myScript.transform.DeleteAllChildren();
			}
			if(GUILayout.Button(nameof(CChildrenPositionAndRotationRandomizer.RandomizePositionsEditor))) {
				myScript.RandomizePositionsEditor();
			}
			if(GUILayout.Button(nameof(CChildrenPositionAndRotationRandomizer.RandomizeRotationsEditor))) {
				myScript.RandomizeRotationsEditor();
			}
			if(GUILayout.Button(nameof(CChildrenPositionAndRotationRandomizer.RandomizeScalesEditor))) {
				myScript.RandomizeScalesEditor();
			}
		}
	}
	#endif
	
	#endregion <<---------- Editor ---------->>

}