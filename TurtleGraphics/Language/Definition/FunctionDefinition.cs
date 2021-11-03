using System.Collections.Generic;

namespace TurtleGraphics.Definition {
	public class FunctionDefinition {
		public string Name { get; set; }

		public List<Parameter> Parameters { get; set; } = new List<Parameter>();
	}
}