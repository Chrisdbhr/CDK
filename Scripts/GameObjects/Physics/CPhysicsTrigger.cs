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

		#endregion <<---------- Collision ---------->>
		



		#region <<---------- Collision and Trigger Registry ---------->>
		
		protected virtual void StartedCollisionOrTrigger(Transform other) {
            bool exited = false;
            this.Enter?.Invoke(other);
            this.Entered?.Invoke(!exited);
            this.Exited?.Invoke(exited);
		}

		protected virtual void ExitedCollisionOrTrigger(Transform other) {
            bool exited = true;
			this.Exit?.Invoke(other);
			this.Entered?.Invoke(!exited);
			this.Exited?.Invoke(exited);
		}
		
		#endregion <<---------- Collision and Trigger Registry ---------->>
		
	}
}