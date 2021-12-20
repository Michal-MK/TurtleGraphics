namespace TurtleGraphics.ParsedData {
	public enum ParsedAction {
		NONE,
		Forward,
		Rotate,
		MoveTo,
		Color,
		Thickness,
		PenState,
		Capping,
		StorePos,
		RestorePos,
		VariableChange,
		ScreenCapture,
	}
}