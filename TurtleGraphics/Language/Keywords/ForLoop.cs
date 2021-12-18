using System;
using System.Threading.Tasks;
using TurtleGraphics.InteliCmmands;

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

		public Func<Task<ILanguageElement>> PreInsertEvent => async () => {
			ForLoopVarNameDialog dialog = new ForLoopVarNameDialog();
			bool? result = dialog.ShowDialog();
			if (result.HasValue && result.Value) {
				Expand(dialog.VarName);
			}
			return await Task.FromResult(this);
		};

		public InteliCommandData Process(string wordArtifact) {
			if (Name.StartsWith(wordArtifact) && Name.Length != wordArtifact.Length) {
				return new InteliCommandData(this);
			}
			return null;
		}
	}
}
