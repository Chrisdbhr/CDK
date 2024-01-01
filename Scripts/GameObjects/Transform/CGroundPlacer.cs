using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace CDK {
    [ExecuteInEditMode]
    public class CGroundPlacer : MonoBehaviour {

        [SerializeField] private LayerMask _groundLayer = 1;
        [SerializeField] private float _raycastDistance = 50f;
        [SerializeField] private Vector3 _groundOffset = new Vector3(0f,-0.5f,0f);

        private void OnValidate() {
            if (!this.enabled) return;
            this.PlaceOnGround();
        }

        private void Awake() {
            this.PlaceOnGround();
            #if UNITY_EDITOR
            EditorSceneManager.sceneSaving += EditorSceneManagerOnSceneSaving;
            #endif
        }

        private void OnDestroy() {
            #if UNITY_EDITOR
            EditorSceneManager.sceneSaving -= EditorSceneManagerOnSceneSaving;
            #endif
        }

        #if UNITY_EDITOR
        private void EditorSceneManagerOnSceneSaving(Scene scene, string path) {
            if (this == null) return;
            PlaceOnGround();
        }

        private void OnDrawGizmos() {
            if (!this.enabled) return;

            var ray = GetGroundCheckRay();
            Gizmos.color = Color.white;
            Gizmos.DrawRay(ray.origin, ray.direction * this._raycastDistance);

            if (GetRaycastResult(ray, out var hitPoint)) {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(ray.origin, hitPoint - ray.origin);
                if (TryGetComponent<Renderer>(out var rend)) {
                    // project transform rotation on ground:
                    var dir = transform.forward;
                    dir.y = 0f;

                    Gizmos.matrix = Matrix4x4.TRS(hitPoint,
                        Quaternion.LookRotation(dir),
                        this.transform.localScale * 0.5f);
                    var size = rend.bounds.size;
                    size.y = 0f;
                    if (TryGetComponent<MeshFilter>(out var meshFilter)) {
                        Gizmos.DrawWireMesh(meshFilter.sharedMesh, Vector3.zero, Quaternion.identity, size);
                    }
                    else {
                        Gizmos.DrawWireCube(Vector3.zero, size);
                    }
                }
            }
        }
        #endif

        #if EASY_BUTTONS
        [EasyButtons.Button]
        #endif
        private void PlaceOnGround() {
            if (!this.enabled) return;
            if (this._groundLayer == 0) {
                Debug.LogError("Ground layer is not set!");
                return;
            }

            var ray = GetGroundCheckRay();
            if (GetRaycastResult(ray, out var hitPoint)) {
                var newPos = hitPoint + this._groundOffset;
                #if UNITY_EDITOR
                Undo.RecordObject(this.transform, $"{this.name} new position {newPos}");
                #endif
                this.transform.position = newPos;
            }
        }

        Ray GetGroundCheckRay() {
            return new Ray(this.transform.position + (Vector3.up * (_raycastDistance * 0.5f)),
                Vector3.down);
        }

        private bool GetRaycastResult(Ray ray, out Vector3 hitPoint) {
            hitPoint = default;
            var hits = Physics.RaycastAll(ray, this._raycastDistance, this._groundLayer);
            if (hits.CIsNullOrEmpty()) return false;

            var allChild = this.GetComponentsInChildren<Transform>();

            hits = hits.Where(h => !allChild.Contains(h.transform)).ToArray();

            if (hits.CIsNullOrEmpty()) return false;

            hitPoint = hits.First().point;
            return true;
        }

    }
}