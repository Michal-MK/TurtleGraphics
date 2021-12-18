using System;
using System.Threading.Tasks;
using TurtleGraphics.InteliCmmands;

namespace TurtleGraphics.Language {
	public interface ILanguageElement { 
		string Name { get; }

		int CaretIndex { get; }

		string CompletionText { get; }

		Func<Task<ILanguageElement>> PreInsertEvent { get; }

		InteliCommandData Process(string wordArtifact);
	}
}
