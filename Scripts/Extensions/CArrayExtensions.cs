using System;

namespace CDK {
	public static class CArrayExtensions {
		
		public static bool ContainsIndex(this Array array, int index, int dimension) {
			if (index < 0) return false;
			return index < array.GetLength(dimension);
		}
	}
}
