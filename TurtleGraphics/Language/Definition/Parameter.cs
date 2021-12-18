using System;

namespace TurtleGraphics.Language.Definition {
	public class Parameter {
		public Type Type { get; set; }

		public string Name { get; set; }

		public string CompletionText => Name; // There is nothing new to add other than the name

		public string Description { get; set; }
	}
}