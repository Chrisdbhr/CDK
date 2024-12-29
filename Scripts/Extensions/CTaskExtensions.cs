using System;
using System.Threading.Tasks;
using UnityEngine;

namespace CDK {
	public static class CTaskExtensions {

		public static async void CAwait(this Task task, bool continueOnCapturedContext = true)
		{
			try {
				await task.ConfigureAwait(continueOnCapturedContext);
			}
			catch (Exception e) {
				Debug.LogException(e);
			}
		}

		public static bool CIsRunning(this Task task) {
			return task != null && !task.IsCompleted;
		}
	}
}
