using UnityEngine;

namespace CDK {
	public static class CStringExtensions {
		public static Vector3 ToVector3(this string sVector)
		{
			// Remove the parentheses
			if (sVector.StartsWith ("(") && sVector.EndsWith (")")) {
				sVector = sVector.Substring(1, sVector.Length-2);
			}
 
			// split the items
			string[] sArray = sVector.Split(',');
 
			// store as a Vector3
			Vector3 result = new Vector3(
				float.Parse(sArray[0]),
				float.Parse(sArray[1]),
				float.Parse(sArray[2]));
 
			return result;
		}

		public static bool CIsNullOrEmpty(this string str) {
			return string.IsNullOrEmpty(str);
		}
		
		public static bool CIsNullOrWhitespace(this string str) {
			return string.IsNullOrWhiteSpace(str);
		}

		public static string CSubstring(this string input, int startIndex, int length) {
			if (string.IsNullOrEmpty(input)) return null;
			if (length >= input.Length) {
				return input;
			}
			return input.Substring(startIndex, length);
		}
	}
}