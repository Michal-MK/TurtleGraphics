using TurtleGraphics.Language.Definition;

namespace TurtleGraphics.Language {
	public class FunctionCall {
		public FunctionDefinition FunctionDef { get; set; }
		public ParameterValuation[] Arguments { get; set; }

		public string GetArg(int i, string line) => Arguments.Length > i ?
			Arguments[i].Value.ToString() : throw new ParsingException("Not enough arguments provided supplied to the function!", line);
	}
}