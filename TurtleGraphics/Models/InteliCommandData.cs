namespace TurtleGraphics.Models {
	public class InteliCommandData : BaseViewModel {
		private string _hintText;
		private bool _selected;
		private string _toComplete;

		public string ToComplete { get => _toComplete; set { _toComplete = value; Notify(nameof(ToComplete)); } }
		public bool Selected { get => _selected; set { _selected = value; Notify(nameof(Selected)); } }
		public string HintText { get => _hintText; set { _hintText = value; Notify(nameof(HintText)); } }

		public InteliCommandData(string hintText, string toComplete) {
			HintText = hintText;
			ToComplete = toComplete;
		}
	}
}
