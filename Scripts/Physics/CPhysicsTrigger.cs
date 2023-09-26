using System;
using UnityEngine;

namespace CDK {
	public class CPhysicsTrigger : CBasePhysicsTriggers {
        
		#region <<---------- Triggers ---------->>
		
		protected virtual void OnTriggerEnter(Collider other) {
			if (this.WillIgnoreTrigger(other.transform)) return;
			this.StartedCollisionOrTrigger(other.transform);
		}

		protected virtual void OnTriggerEnter2D(Collider2D other) {
			if (this.WillIgnoreTrigger(other.transform)) return;
			this.StartedCollisionOrTrigger(other.transform);
		}

		protected virtual void OnTriggerExit(Collider other) {
			if (this.WillIgnoreTrigger(other.transform)) return;
			this.ExitedCollisionOrTrigger(other.transform);
		}

		protected virtual void OnTriggerExit2D(Collider2D other) {
			if (this.WillIgnoreTrigger(other.transform)) return;
			this.ExitedCollisionOrTrigger(other.transform);
		}

        private void OnTriggerStay(Collider other) {
            if (this.WillIgnoreTrigger(other.transform)) return;
            this.StayingOnCollisionOrTrigger(other.transform);
        }

        private void OnTriggerStay2D(Collider2D other) {
            if (this.WillIgnoreTrigger(other.transform)) return;
            this.StayingOnCollisionOrTrigger(other.transform);
        }

        #endregion <<---------- Triggers ---------->>

		
		
		
		#region <<---------- Collision ---------->>
		
		protected virtual void OnCollisionEnter(Collision other) {
			if (this.WillIgnoreTrigger(other.transform)) return;
			this.StartedCollisionOrTrigger(other.transform);
		}

		protected virtual void OnCollisionExit(Collision other) {
			if (this.WillIgnoreTrigger(other.transform)) return;
			this.StartedCollisionOrTrigger(other.transform);
		}

		protected virtual void OnCollisionEnter2D(Collision2D other) {
			if (this.WillIgnoreTrigger(other.transform)) return;
			this.ExitedCollisionOrTrigger(other.transform);
		}

		protected virtual void OnCollisionExit2D(Collision2D other) {
			if (this.WillIgnoreTrigger(other.transform)) return;
			this.ExitedCollisionOrTrigger(other.transform);
		}

        private void OnCollisionStay(Collision other) {
            if (this.WillIgnoreTrigger(other.transform)) return;
            this.StayingOnCollisionOrTrigger(other.transform);
        }

        private void OnCollisionStay2D(Collision2D other) {
            if (this.WillIgnoreTrigger(other.transform)) return;
            this.StayingOnCollisionOrTrigger(other.transform);
        }

        #endregion <<---------- Collision ---------->>
		



		#region <<---------- Collision and Trigger Registry ---------->>
		
		protected virtual void StartedCollisionOrTrigger(Transform other) {
            if (TriggerOnce && _triggered) return;
            bool exited = false;
            this.Enter?.Invoke(other);
            this.Entered?.Invoke(!exited);
            this.Exited?.Invoke(exited);
            _triggered = true;
        }

		protected virtual void ExitedCollisionOrTrigger(Transform other) {
            if (TriggerOnce && _triggered) return;
            bool exited = true;
			this.Exit?.Invoke(other);
			this.Entered?.Invoke(!exited);
			this.Exited?.Invoke(exited);
		}

        protected virtual void StayingOnCollisionOrTrigger(Transform other) {
            if (this.CannotTrigger) return;
        }
		
		#endregion <<---------- Collision and Trigger Registry ---------->>
		
	}
}