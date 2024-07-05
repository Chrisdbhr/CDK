using System;
using CDK.Damage;
using UnityEngine;

namespace CDK {
    public class CPreciseBullet : CDamageDealerTrigger {

        [Range(1, 20), SerializeField] float autoDestroyAfter = 3f;
        [SerializeField] float moveSpeed = 22f;
        [SerializeField] float bulletSize = 0.05f;
        [SerializeField] LayerMask hitLayers;
        Vector3 previousPosition;

        void Start() {
            this.CDestroyGameObject(autoDestroyAfter);
        }

        void Update() {
            this.previousPosition = this.transform.position;
            this.transform.position += this.transform.forward * (moveSpeed * CTime.DeltaTimeScaled);
        }

        void LateUpdate() {
            if (!Physics.SphereCast(previousPosition,
                    bulletSize,
                    transform.forward,
                    out var hit,
                    Vector3.Distance(transform.position, previousPosition),
                    hitLayers,
                    QueryTriggerInteraction.Ignore
                )) return;
            var damageable = hit.collider.GetComponent<ICDamageable>();
            damageable?.TakeHit(AttackData.data, AttackData.AttackerTransform);
            this.DestroyBullet(hit.point);
        }

        void DestroyBullet(Vector3 hitPoint) {
            this.CDestroyGameObject();
        }

        #if UNITY_EDITOR
        void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this.transform.position, bulletSize);
        }
        #endif
    }
}