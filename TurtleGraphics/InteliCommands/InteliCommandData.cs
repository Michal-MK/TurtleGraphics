using TurtleGraphics.Definition;
using TurtleGraphics.Models;

namespace TurtleGraphics.InteliCmmands {
	public class InteliCommandData : BaseViewModel {
		[Notify]
		public string ToComplete { get; }

		[Notify]
		public bool Selected { get; set; }

		[Notify]
		public string HintText { get; }

		public string FullInsertText => ToComplete + "();";

		public FunctionDefinition Def { get; }
		public string FullCommand { get; }
		public int CaretPosition => Def.CaretIndex;


		public InteliCommandData(FunctionDefinition def, string hintText, string toComplete) {
			Def = def;
			FullCommand = def.Name;
			HintText = hintText;
			ToComplete = toComplete;
		}

		public int GetCaretPos(int caret) {
			return caret + CaretPosition - (FullCommand.Length - ToComplete.Length);
		}
	}
}