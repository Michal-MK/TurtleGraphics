using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TurtleGraphics {
	public class LineCacheData {

		public ParsedData ParsingInfo { get; }
		public TurtleData CompiledData { get; }

		public bool ContainsVariable { get; }

		public LineCacheData(ParsedData parsed, TurtleData compiled) {
			ParsingInfo = parsed;
			CompiledData = compiled;
			ContainsVariable = ContainsAny(ParsingInfo.Variables.Keys);
		}

		private bool ContainsAny(IEnumerable<string> vals) {
			foreach (string str in vals) {
				if (ParsingInfo.Arg1 != null && ContainsAsWord(str, ParsingInfo.Arg1)) return true;
				if (ParsingInfo.Arg2 != null && ContainsAsWord(str, ParsingInfo.Arg2)) return true;
				if (ParsingInfo.Arg3 != null && ContainsAsWord(str, ParsingInfo.Arg3)) return true;
				if (ParsingInfo.Arg4 != null && ContainsAsWord(str, ParsingInfo.Arg4)) return true;
			}
			return false;
		}

		private bool ContainsAsWord(string word, string arg) {
			int startIndex = 0;
			while (true) {
				int ind = arg.IndexOf(word, startIndex);

				if (ind == -1) {
					return false;
				}

				int end = ind + word.Length;

				if (ind == 0 && end < arg.Length && !char.IsLetterOrDigit(arg[end])) {
					return true;
				}

				if (!char.IsLetterOrDigit(arg[ind - 1]) || (end < arg.Length && !char.IsLetterOrDigit(arg[end]))) {
					return true;
				}

				startIndex = ind;
			}
		}
	}
}
