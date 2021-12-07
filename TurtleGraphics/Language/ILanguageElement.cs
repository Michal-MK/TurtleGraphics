using System;
using System.Threading.Tasks;

namespace TurtleGraphics.Language {
	public interface ILanguageElement { 
		string Name { get; }

		int CaretIndex { get; }

		string CompletionText { get; }

		Func<Task<ILanguageElement>> PreInsertEvent { get; }
	}
}
