using TurtleGraphics.Language;
using TurtleGraphics.Models;

namespace TurtleGraphics.InteliCmmands {
	public class InteliCommandData : BaseViewModel {

		[Notify]
		public bool Selected { get; set; }

		[Notify]
		public string HintText { get; }

		public string FullInsertText => Def.CompletionText;

		public ILanguageElement Def { get; }
		public string FullCommand { get; }
		public int CaretPosition => Def.CaretIndex;


		public InteliCommandData(ILanguageElement def, string wordArtifact, string suffix) {
			Def = def;
			FullCommand = def.Name;
			HintText = wordArtifact + suffix;
		}

		public InteliCommandData(ILanguageElement def) {
			Def = def;
			FullCommand = def.Name;
		}

		public int GetCaretPos(int caret) {
			return caret + CaretPosition;
		}
	}
}