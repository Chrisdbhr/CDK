using System;
using System.Collections.Generic;
using System.Linq;

namespace CDK {
	public static class CEnumExtensions {

		public static IEnumerable<T> GetValues<T>() {
			return Enum.GetValues(typeof(T)).Cast<T>();
		}

	}
}