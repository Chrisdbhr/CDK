using UnityEngine;

namespace CDK {
    public class CWaterSurface : CPhysicsTrigger {
        protected override void StartedCollisionOrTrigger(Transform transf) {
            base.StartedCollisionOrTrigger(transf);
            if (transf == null) return;
            var w = transf.GetComponent<ICWaterInteractor>();
            if (w == null) return;
            w.OnEnterWater(transform);
        }

        protected override void ExitedCollisionOrTrigger(Transform transf) {
            base.ExitedCollisionOrTrigger(transf);
            if (transf == null) return;
            var w = transf.GetComponent<ICWaterInteractor>();
            if (w == null) return;
            w.OnExitWater(transform);
        }
    }
}