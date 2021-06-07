using System.Threading.Tasks;

namespace CDK {
	public static class CTaskExtensions {

		public static async void CAwait(this Task task, bool continueOnCapturedContext = true) {
			await task.ConfigureAwait(continueOnCapturedContext);
		}
		
	}
}
