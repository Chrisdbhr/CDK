using System;
using UnityEngine;

namespace CDK {
    public class CSnapToRaycastPointOnAwake : CAutoTriggerCompBase {

        [SerializeField] private Vector3 _globalDirection = Vector3.down;
        [SerializeField] private float _distance = 1000f;
        [SerializeField] private LayerMask _collisionLayers = 1;
        
        

        protected override void TriggerEvent() {
            if (!CheckForGround(out var hitInfo)) return;
            this.transform.position = hitInfo.point;
        }

        private void OnDrawGizmosSelected() {
            var ray = this.GetRay();
            var color = Color.white;
            if (!this.CheckForGround(out var hitInfo)) {
                color = Color.red;
                hitInfo.point = ray.origin + (ray.direction * this._distance);
            }
            Debug.DrawLine(ray.origin, hitInfo.point, color, 1f, true);
        }

        private bool CheckForGround(out RaycastHit hitInfo) {
            var ray = GetRay();
            return Physics.Raycast(ray.origin, ray.direction, out hitInfo, this._distance, this._collisionLayers, QueryTriggerInteraction.Ignore);
        }
        
        private Ray GetRay() {
            return new Ray(this.transform.position, this._globalDirection.normalized * this._distance);
        }
        
    }
}