using TurtleGraphics.Language;
using TurtleGraphics.Models.Base;

namespace TurtleGraphics.IntelliCommands {
	public class IntelliCommandData : BaseViewModel {

		[Notify]
		public bool Selected { get; set; }

		[Notify]
		public string HintText { get; }

		public string FullInsertText => Def.CompletionText;

		public ILanguageElement Def { get; set; }
		public string FullCommand { get; }
		public int CaretPosition => Def.CaretIndex;


		public IntelliCommandData(ILanguageElement def, string wordArtifact, string suffix) {
			Def = def;
			FullCommand = def.Name;
			HintText = wordArtifact + suffix;
		}

		public int GetCaretPos(int caret) {
			return caret + CaretPosition;
		}
	}
}