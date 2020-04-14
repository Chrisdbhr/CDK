namespace CDK {
	public enum CMovState {
		Idle, Walking, Running, Sliding
	}
	
	public static class CMovStateExtensions {
		public static bool IsMoving(this CMovState movState) {
			return movState == CMovState.Running 
					|| movState == CMovState.Walking
					|| movState == CMovState.Sliding;
		}
	}
}