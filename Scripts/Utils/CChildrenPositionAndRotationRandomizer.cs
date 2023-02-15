using System;
using System.Linq;
using UnityEngine;
using Unity.Linq;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK {
	public class CChildrenPositionAndRotationRandomizer : MonoBehaviour {
		
		#region <<---------- Properties and Fields ---------->>

        [SerializeField, Range(0, 1000)] private int _safeOverflowCount = 1000;
        [SerializeField] private Vector3 _positionRange = Vector3.one * 10;
		[SerializeField] private Vector2 _scaleRange = new Vector2(1f, 2f);
		[SerializeField] private Vector3 _angleRange = Vector3.one * 30;
        [NonSerialized] private GameObject[] _childObjs;
        [SerializeField] private LayerMask _checkLayers = -1;
        [SerializeField, TagSelector] private string[] _ignoreTag = new []{ "NoCameraCollision" };

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
				var pos = this.GetValidPosition(target, this._safeOverflowCount);
				Undo.RecordObject(target, $"undo-position-{target.name}");
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
				Undo.RecordObject(target, $"undo-rotation-{target.name}");
				target.rotation = Quaternion.Euler(angle);
			});
		}
		public void RandomizeScalesEditor() {
			this.DoForEachGameObject(target => {
                float scale = Random.Range(_scaleRange.x, _scaleRange.y);
                Undo.RecordObject(target, $"undo-scale-{target.name}");
				target.localScale = scale * Vector3.one;
			});
		}

        public void RandomizeAll() {
            this.RandomizeScalesEditor();
            this.RandomizeRotationsEditor();
            this.RandomizePositionsEditor();
        }
        
		#endif

		private void DoForEachGameObject(Action<Transform> actionOnTransform) {
			#if UNITY_EDITOR
			string progressBarTitle = "Randomizing Objects";
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

		private Vector3 GetValidPosition(Transform t, int safeOverflowCount) {
			var pos = new Vector3(
				Random.Range(-this._positionRange.x, this._positionRange.x),
				Random.Range(-this._positionRange.y, this._positionRange.y),
				Random.Range(-this._positionRange.z, this._positionRange.z)
			);
			pos += this.transform.position;
            t.transform.position = pos;
            Physics.SyncTransforms();
            
			if (safeOverflowCount <= 0) {
				Debug.Log($"{nameof(CChildrenPositionAndRotationRandomizer)}: Safe overflow reached on {t.name}. Stopping loop to prevent stack overflow.", t);
				return pos;
			}

            var tChildCol = t.GetComponentsInChildren<Collider>();
            var allOtherColliders = FindObjectsOfType<Collider>(false).Where(c => !tChildCol.Contains(c) && this._checkLayers.CContains(c.gameObject.layer)).ToArray();
            
			bool needToRecalculate = false;
            MeshCollider startedNonConvexMesh = null;
            foreach (var myCol in tChildCol) {
                foreach (var otherCol in allOtherColliders) {
                    startedNonConvexMesh = null;
                    if (otherCol is MeshCollider m && !m.convex) {
                        startedNonConvexMesh = m;
                        startedNonConvexMesh.convex = true;
                    }

                    if (Physics.ComputePenetration(myCol,
                            myCol.transform.position,
                            myCol.transform.rotation,
                            otherCol, 
                            otherCol.transform.position,
                            otherCol.transform.rotation,
                            out _, out _)
                        ) {
                        foreach (var tagToIgnore in _ignoreTag) {
                            if (!otherCol.CompareTag(tagToIgnore)) {
                                needToRecalculate = true;
                            }
                        }
                    }
                    
                    if (startedNonConvexMesh != null) {
                        startedNonConvexMesh.convex = false;
                    }

                    if (needToRecalculate) break;
                }
                if (needToRecalculate) break;
            }

			return needToRecalculate ? this.GetValidPosition(t, safeOverflowCount - 1) : pos;
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
            if(GUILayout.Button(nameof(CChildrenPositionAndRotationRandomizer.RandomizeScalesEditor))) {
                myScript.RandomizeScalesEditor();
            }
            if(GUILayout.Button(nameof(CChildrenPositionAndRotationRandomizer.RandomizeRotationsEditor))) {
                myScript.RandomizeRotationsEditor();
            }
			if(GUILayout.Button(nameof(CChildrenPositionAndRotationRandomizer.RandomizePositionsEditor))) {
				myScript.RandomizePositionsEditor();
			}
            if(GUILayout.Button(nameof(CChildrenPositionAndRotationRandomizer.RandomizeAll))) {
                myScript.RandomizeAll();
            }
		}
	}
	#endif
	
	#endregion <<---------- Editor ---------->>

}