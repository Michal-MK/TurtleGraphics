using System;

namespace TurtleGraphics.Models {
	public class InteliCommandData : BaseViewModel {
		[Notify] public string ToComplete { get; }
		[Notify] public bool Selected { get; set; }
		[Notify] public string HintText { get; }

		public string FullCommand { get; }
		public int CaretPosition => TakesParameters ? ToComplete.IndexOf("(", StringComparison.Ordinal) + 1 : ToComplete.Length;
		public bool TakesParameters => FullCommand.IndexOf(")", StringComparison.Ordinal) - 1 > FullCommand.IndexOf("(", StringComparison.Ordinal);

		public InteliCommandData(string fullCommand, string hintText, string toComplete) {
			FullCommand = fullCommand;
			HintText = hintText;
			int start = toComplete.IndexOf("(", StringComparison.Ordinal);
			ToComplete = toComplete.Remove(start + 1, toComplete.IndexOf(")", StringComparison.Ordinal) - 1 - start);
		}
	}
}