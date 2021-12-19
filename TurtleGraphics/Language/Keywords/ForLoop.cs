using System;
using System.Threading.Tasks;
using System.Windows;
using TurtleGraphics.IntelliCommands;

namespace TurtleGraphics.Language.Keywords {
	public class ForLoop : ILanguageElement {
		public string Format { get; } = "for(int {0} = 0; {0} < _|; {0}++) {{" + Environment.NewLine + Environment.NewLine + "}}";

		public string CompletionText { get; private set; }

		public string Expand(string paramName) {
			CompletionText = string.Format(Format, paramName);
			CompletionText = CompletionText.Replace(artifact, "");
			CaretIndex = CompletionText.IndexOf("_|");
			CompletionText = CompletionText.Remove(CaretIndex, 2);
			return CompletionText;
		}

		public string Name => "for...";

		private string artifact;

		public int CaretIndex { get; private set; }

		public Func<Task<ILanguageElement>> PreInsertEvent => async () => {
			ForLoopVarNameDialog dialog = new ForLoopVarNameDialog();
			dialog.Owner = MainWindow.Instance;
			dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			bool? result = dialog.ShowDialog();
			if (result.HasValue && result.Value) {
				Expand(dialog.VarName);
			}
			return await Task.FromResult(this);
		};

		public IntelliCommandData Process(string wordArtifact) {
			if (Name.StartsWith(wordArtifact) && Name.Length != wordArtifact.Length) {
				artifact = wordArtifact;
				string suffix = Name.Remove(0, wordArtifact.Length);
				return new IntelliCommandData(this, wordArtifact, suffix);
			}
			return null;
		}
	}
}