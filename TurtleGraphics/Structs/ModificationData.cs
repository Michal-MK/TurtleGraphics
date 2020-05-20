namespace TurtleGraphics {
	public struct ModificationData {
		public ModificationData(string text, int inputLength) {
			Text = text;
			InputLength = inputLength;
		}

		public string Text { get; set; }
		public int InputLength { get; set; }
	}
}
