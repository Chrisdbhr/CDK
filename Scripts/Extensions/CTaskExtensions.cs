using System.Threading.Tasks;

namespace CDK {
	public static class CTaskExtensions {

		public static async Task CAwait(this Task task, bool continueOnCapturedContext = true) {
			await task.ConfigureAwait(continueOnCapturedContext);
		}
		
	}
}
