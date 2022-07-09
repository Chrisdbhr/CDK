using UnityEngine;

namespace CDK {
    public class CWaterTrigger : CPhysicsTrigger {
        protected override void StartedCollisionOrTrigger(Transform transf) {
            base.StartedCollisionOrTrigger(transf);
            if (transf == null) return;
            var w = transf.GetComponent<ICWaterInteraction>();
            if (w == null) return;
            w.OnEnterWater(this.transform);
        }

        protected override void ExitedCollisionOrTrigger(Transform transf) {
            base.ExitedCollisionOrTrigger(transf);
            if (transf == null) return;
            var w = transf.GetComponent<ICWaterInteraction>();
            if (w == null) return;
            w.OnExitWater(this.transform);
        }
    }
}