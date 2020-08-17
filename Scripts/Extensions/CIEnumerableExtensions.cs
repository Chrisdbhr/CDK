using System.Collections.Generic;
using System.Linq;

namespace CDK {
	public static class CIEnumerableExtensions {
		public static T RandomElement<T>(this IEnumerable<T> enumerable) {
			var array = enumerable as T[] ?? enumerable.ToArray();
			int index = CRandom.system.Next(0, array.Length);
			return array.ElementAt(index);
		}
	}
}
