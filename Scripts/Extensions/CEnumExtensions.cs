using System;
using System.Collections.Generic;
using System.Linq;

namespace CDK {
	public static class CEnumExtensions {

		public static IEnumerable<T> CGetValues<T>() {
			return Enum.GetValues(typeof(T)).Cast<T>();
		}
		
		public static int CToInt(this Enum @enum) {
			return Convert.ToInt32(@enum);
		}

	}
}