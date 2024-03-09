namespace CDK {
	public class CDontDestroyOnLoadTrigger : CAutoTriggerCompBase {
		protected override void TriggerEvent() {
			DontDestroyOnLoad(this.gameObject);
		}
	}
}