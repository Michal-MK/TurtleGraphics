using System.Collections.Generic;
using TurtleGraphics.Definition;

namespace TurtleGraphics {
	public static class FunctionNames {
		public static List<string> Functions { get; } = new List<string> {
			"Forward",
			"Rotate",
			"MoveTo",
			"SetBrushSize",
			"SetColor",
			"StoreTurtlePosition",
			"RestoreTurtlePosition",
			"SetLineCapping",
			"PenUp",
			"PenDown",
			"CaptureScreenshot"
		};

		public static FunctionDefinition Parse(string name) {
			if (!Functions.Contains(name)) throw new ParsingException("Unknown sequence of characters encountered", name);

			return new FunctionDefinition{Name = name};
		}
	}
}
