namespace CDK {
	public enum CMovState {
		Idle, Walking, Running, Sprint, Sliding
	}
	
	public static class CMovStateExtensions {
		public static bool IsMoving(this CMovState movState) {
			return movState > CMovState.Idle;
		}

	}
}