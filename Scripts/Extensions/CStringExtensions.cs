using System.Text;
using UnityEngine;

namespace CDK {
	public static class CStringExtensions {

		private static StringBuilder sb = new StringBuilder();
		
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

        public static bool CIsNotNullOrEmpty(this string str) {
            return !CIsNullOrEmpty(str);
        }

        public static bool CIsNullOrEmpty(this string str) {
			return string.IsNullOrEmpty(str);
		}
		
        public static bool CIsNotNullOrWhitespace(this string str) {
            return !CIsNullOrWhitespace(str);
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
		
		/// <summary>
		/// Returns the input string with the first character converted to uppercase, or mutates any nulls passed into string.Empty
		/// </summary>
		public static string CFirstLetterToUpperCase(this string s) {
			// from https://stackoverflow.com/questions/4135317/make-first-letter-of-a-string-upper-case-with-maximum-performance/27073919#27073919
			if (string.IsNullOrEmpty(s)) return string.Empty;

			char[] a = s.ToCharArray();
			a[0] = char.ToUpper(a[0]);
			return new string(a);
		}
		
		/// <summary>
		/// Remove special characters from string.
		/// </summary>
		/// <param name="text"></param>
		/// <returns>Return normalized string.</returns>
		public static string CRemoveDiacritics(this string s) {
			var normalizedString = s.Normalize(NormalizationForm.FormD);
			sb.Clear();

			foreach (var c in normalizedString) {
				var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
				if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark) {
					sb.Append(c);
				}
			}

			return sb.ToString().Normalize(NormalizationForm.FormC);
		}
	}
}