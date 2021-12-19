using System;
using System.Threading.Tasks;
using TurtleGraphics.IntelliCommands;

namespace TurtleGraphics.Language {
	public interface ILanguageElement { 
		string Name { get; }

		int CaretIndex { get; }

		string CompletionText { get; }

		Func<Task<ILanguageElement>> PreInsertEvent { get; }

		IntelliCommandData Process(string wordArtifact);
	}
}