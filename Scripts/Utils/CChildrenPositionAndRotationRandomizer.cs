using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.Linq;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK {
	public class CChildrenPositionAndRotationRandomizer : MonoBehaviour {
		
		[SerializeField] private Vector3 _range = Vector3.one * 10;
		[SerializeField] private Vector3 _angle = Vector3.one * 30;
		[SerializeField] private float _minimumSpaceBetweenObjs = 1f;
		[NonSerialized] private GameObject[] _childObjs;
		
		
		
		
		private void OnDrawGizmosSelected() {
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireCube(this.transform.position, this._range*2f);
		}

		public void RandomizeObjs() {
			#if UNITY_EDITOR
			string progressBarTitle = "Randomizing positions";
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

				var pos = this.GetValidPosition(200);
				var angle = new Vector3(
					Random.Range(-this._angle.x, this._angle.x),
					Random.Range(-this._angle.y, this._angle.y),
					Random.Range(-this._angle.z, this._angle.z)
				);
				child.transform.position = pos;
				child.transform.rotation = Quaternion.Euler(angle);
			}
			#if UNITY_EDITOR
			EditorUtility.ClearProgressBar();
			#endif
		}

		private Vector3 GetValidPosition(int safeOverflowCount) {
			var pos = new Vector3(
				Random.Range(-this._range.x, this._range.x),
				Random.Range(-this._range.y, this._range.y),
				Random.Range(-this._range.z, this._range.z)
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

	#if UNITY_EDITOR
	[CustomEditor(typeof(CChildrenPositionAndRotationRandomizer))]
	public class RandomPositionAndRotationInstantiatorEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
        
			CChildrenPositionAndRotationRandomizer myScript = (CChildrenPositionAndRotationRandomizer)target;
			if(GUILayout.Button(nameof(CTransformExtensions.DeleteAllChildren)))
			{
				myScript.transform.DeleteAllChildren();
			}
			if(GUILayout.Button(nameof(myScript.RandomizeObjs)))
			{
				myScript.RandomizeObjs();
			}
		}
	}
	#endif
}