namespace CDK {
	public class CDontDestroyOnLoadTrigger : CAutoTriggerCompBase {
		protected override void TriggerEvent() {
			gameObject.CDontDestroyOnLoad();
		}
	}
}