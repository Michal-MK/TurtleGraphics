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
	}
}