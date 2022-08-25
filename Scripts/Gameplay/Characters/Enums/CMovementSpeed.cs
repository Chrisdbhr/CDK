namespace CDK {
	public enum CMovementSpeed {
		Idle, 
        Walking,
        Running,
        Sprint
	}
	
	public static class CMovStateExtensions {
		public static bool IsMoving(this CMovementSpeed movementSpeed) {
			return movementSpeed > CMovementSpeed.Idle;
		}
        
        public static bool IsMovingFast(this CMovementSpeed movementSpeed) {
            return movementSpeed > CMovementSpeed.Walking;
        }

	}
}