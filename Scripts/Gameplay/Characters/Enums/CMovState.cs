namespace CDK {
	public enum CMovState {
		Idle, 
        Walking,
        Running,
        Sprint
	}
	
	public static class CMovStateExtensions {
		public static bool IsMoving(this CMovState movState) {
			return movState > CMovState.Idle;
		}
        
        public static bool IsMovingFast(this CMovState movState) {
            return movState > CMovState.Walking;
        }

	}
}