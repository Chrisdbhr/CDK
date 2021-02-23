using System;
using System.Collections.Generic;
using System.Linq;

namespace CDK {
	public static class CIEnumerableExtensions {
		public static T RandomElement<T>(this IEnumerable<T> enumerable) {
			var array = enumerable as T[] ?? enumerable.ToArray();
			if (array.Length <= 0) return default;
			int index = CRandom.system.Next(0, array.Length);
			return array.ElementAt(index);
		}
		
		public static bool ContainsIndex<T>(this IEnumerable<T> enumerable, int index) {
			return index >= 0 && enumerable != null && index < enumerable.Count();
		}

		public static T GetAtIndexSafe<T>(this IEnumerable<T> enumerable, int index) {
			var array = enumerable as T[] ?? enumerable.ToArray();
			return array.ContainsIndex(index) ? array.ElementAt(index) : default;
		}
		
	}
}
