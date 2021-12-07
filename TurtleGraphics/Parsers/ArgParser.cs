using System.Collections.Generic;
using TurtleGraphics.Language.Definition;

namespace TurtleGraphics.Parsers {
	public static class ArgParser {
		public static ParameterValuation[] Parse(string args) {
			List<ParameterValuation> parsed = new List<ParameterValuation>();

			bool isString = false;
			bool isChar = false;
			int innerLevel = 0;

			int start = 0;

			for (int i = 0; i < args.Length; i++) {
				if (args[i] == '(') {
					innerLevel++;
				}
				if (args[i] == ')') {
					innerLevel--;
				}
				if (args[i] == '\'') {
					isChar ^= true;
				}
				if (args[i] == '\"') {
					isString ^= true;
				}

				if (!isString && !isChar && innerLevel == 0 && args[i] == ',') {
					parsed.Add(new ParameterValuation<string>(new Parameter { Name = "Param TODO" }, args.Substring(start, i)));
					parsed.AddRange(Parse(args.Substring(i + 1)));
					break;
				}
			}
			if (parsed.Count == 0 && !string.IsNullOrWhiteSpace(args)) {
				parsed.Add(new ParameterValuation<string>(new Parameter { Name = "Param TODO" }, args));
			}
			return parsed.ToArray();
		}
	}
}