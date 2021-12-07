using System;
using System.Threading.Tasks;

namespace TurtleGraphics.Language.Definition {
	public class Parameter : ILanguageElement {
		public Type Type { get; set; }

		public string Name { get; set; }

		public string CompletionText => Name; // There is nothing new to add other than the name

		public string Description { get; set; }

		public int CaretIndex => throw new InvalidOperationException("This is unexpected!");

		public Func<Task<ILanguageElement>> PreInsertEvent => null;
	}
}