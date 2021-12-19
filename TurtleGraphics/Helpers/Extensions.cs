using System;
using System.Collections.Generic;

namespace TurtleGraphics {
	public static class Extensions {
		public static Dictionary<T, K> Copy<T, K>(this Dictionary<T, K> dict) {
			return new Dictionary<T, K>(dict);
		}

		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) {
			foreach (T x in source) {
				action(x);
			}
		}

		public static int[] AllIndicesOf(this string source, char character) {
			int index = 0;
			List<int> ret = new List<int>();
			while (index < source.Length) {
				int charIndex = source.Substring(index).IndexOf(character);
				if (charIndex == -1) {
					return ret.ToArray();
				}
				index = charIndex;
				ret.Add(charIndex);
			}
			return ret.ToArray();
		}
	}
}