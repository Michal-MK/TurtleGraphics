using System;
using System.Collections.Generic;

namespace TurtleGraphics {
	public class LineCacheData {

		public ParsedData ParsingInfo { get; }
		public TurtleData CompiledData { get; }

		public bool ContainsVariable => ContainsAny(ParsingInfo.Line, ParsingInfo.Variables.Keys);

		public LineCacheData(ParsedData parsed, TurtleData compiled) {
			ParsingInfo = parsed;
			CompiledData = compiled;
		}

		private bool ContainsAny(string line, IEnumerable<string> vals) {
			foreach (string str in vals) {
				if (line.Contains(str)) {
					return true;
				}
			}
			return false;
		}
	}
}
