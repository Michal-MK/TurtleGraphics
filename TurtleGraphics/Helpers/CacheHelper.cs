using System.Collections.Generic;

namespace TurtleGraphics {
	public static class CacheHelper {

		private static readonly string[] UNCACHEABLE_STRINGS = new[] { "random", "Rand" };

		public static bool IsCacheable(ParsedData data) {
			foreach (string pm in data.Parameters) {
				if (ContainsVariable(pm, data.Variables.Keys)) {
					return false;
				}
				if (ContainsUncacheable(pm)) {
					return false;
				}
			}
			return true;
		}

		private static bool ContainsUncacheable(string pm) {
			foreach (string uncache in UNCACHEABLE_STRINGS) {
				if (pm.Contains(uncache)) return true;
			}
			return false;
		}

		private static bool ContainsVariable(string pm, IEnumerable<string> variables) {
			foreach (string str in variables) {
				if (ContainsAsWord(str, pm)) return true;
			}
			return false;
		}

		private static bool ContainsAsWord(string word, string arg) {
			int startIndex = 0;
			while (true) {
				int ind = arg.IndexOf(word, startIndex);

				if (ind == -1) {
					return false;
				}

				int end = ind + word.Length;

				if (ind == 0 && end == arg.Length) {
					return true;
				}

				if (ind == 0 && end < arg.Length && !char.IsLetterOrDigit(arg[end])) {
					return true;
				}

				if ((ind - 1 > -1 && !char.IsLetterOrDigit(arg[ind - 1])) || (end < arg.Length && !char.IsLetterOrDigit(arg[end]))) {
					return true;
				}

				startIndex = ind;
			}
		}
	}
}
