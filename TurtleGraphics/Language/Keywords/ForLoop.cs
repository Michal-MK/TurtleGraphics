using System;
using System.Threading.Tasks;

namespace TurtleGraphics.Language.Keywords {
	public class ForLoop : ILanguageElement {
		public string Format { get; } = "for(int {0} = 0; {0} < _|; i++) {" + Environment.NewLine + Environment.NewLine + "}";

		public string CompletionText { get; private set; } = null;

		public string Expand(string paramName) {
			CompletionText = string.Format(Format, paramName);
			CaretIndex = CompletionText.IndexOf("_|");
			CompletionText = CompletionText.Remove(CaretIndex, 2);
			return CompletionText;
		}

		public string Name => "for loop";

		public int CaretIndex { get; private set; }

		public Func<Task<ILanguageElement>> PreInsertEvent => null; // TODO param name
	}
}
