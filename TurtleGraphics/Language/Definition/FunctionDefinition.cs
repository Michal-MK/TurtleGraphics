using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TurtleGraphics.Language.Definition {
	public class FunctionDefinition : ILanguageElement {
		public string Name { get; set; }

		public string CompletionText => Name; // There is nothing new to add other than the name

		public List<Parameter> Parameters { get; } = new List<Parameter>();

		public int CaretIndex => Name.Length + 1;

		public Func<Task<ILanguageElement>> PreInsertEvent => null;
	}
}