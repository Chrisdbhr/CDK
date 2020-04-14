namespace CDK {
	public class CDontDestroyOnLoad : CAutoTriggerCompBase
	{
		protected override void TriggerEvent() {
			DontDestroyOnLoad(this.gameObject);
		}
	}
}