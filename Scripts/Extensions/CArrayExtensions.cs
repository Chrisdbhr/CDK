using System.Collections.Generic;

namespace CDK {
	public static class CArrayExtensions {
        
		public static T GetRandomElement<T>(this T[] items)
		{
			// Return a random item.
			return items[UnityEngine.Random.Range(0, items.Length)];
		}
        
		public static T GetRandomElement<T>(this List<T> items)
		{
			// Return a random item.
			return items[UnityEngine.Random.Range(0, items.Count)];
		}
	}
}