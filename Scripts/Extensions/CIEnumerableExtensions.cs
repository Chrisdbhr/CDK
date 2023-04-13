using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CDK {
	public static class CIEnumerableExtensions {
		public static T CRandomElement<T>(this IEnumerable<T> enumerable) {
			var array = enumerable as T[] ?? enumerable.ToArray();
			if (array.Length <= 0) return default;
			int index = CRandom.system.Next(0, array.Length);
			return array.ElementAt(index);
		}
		
		public static bool CContainsIndex<T>(this IEnumerable<T> enumerable, int index) {
			return index >= 0 && enumerable != null && index < enumerable.Count();
		}

		public static T CGetAtIndexSafe<T>(this IEnumerable<T> enumerable, int index) {
			var array = enumerable as T[] ?? enumerable.ToArray();
			return array.CContainsIndex(index) ? array.ElementAt(index) : default;
		}

		public static bool CHasAnyAndNotNull<T>(this IEnumerable<T> enumerable) {
			return enumerable != null && enumerable.Any();
		}
		
		public static bool CIsNullOrEmpty<T>(this IEnumerable<T> enumerable) {
			return !CHasAnyAndNotNull(enumerable);
		}

        public static void CDoForEachNotNull<T>(this IEnumerable<T> enumerable, Action<T> a) where T : Object {
            if (enumerable == null || a == null) return;
            foreach (var o in enumerable) {
                if (o == null) continue;
                a.Invoke(o);
            }
        }
    }
}
