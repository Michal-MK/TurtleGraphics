using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TurtleGraphics.InteliCmmands;

namespace TurtleGraphics.Language.Definition {
	public class FunctionDefinition : ILanguageElement {
		public string Name { get; set; }

		private string completionText = "";
		public string CompletionText => completionText;

		public List<Parameter> Parameters { get; } = new List<Parameter>();

		public int CaretIndex { get; set; }

		public Func<Task<ILanguageElement>> PreInsertEvent => null;

		public InteliCommandData Process(string wordArtifact) {
			if (Name.StartsWith(wordArtifact) && Name.Length != wordArtifact.Length) {
				string suffix = Name.Remove(0, wordArtifact.Length);
				completionText = suffix + "();";
				CaretIndex = suffix.Length + 1;
				return new InteliCommandData(this, wordArtifact, suffix);
			}
			return null;
		}
	}
}