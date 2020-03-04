using System;
using System.Windows;
using System.Windows.Media;

namespace TurtleGraphics.XAML {
	// Create a host visual derived from the FrameworkElement class.
	// This class provides layout, event handling, and container support for
	// the child visual objects.
	public class MyVisualHost : FrameworkElement {
		// Create a collection of child visual objects.
		private VisualCollection _children;

		public MyVisualHost() {
			_children = new VisualCollection(this);

			this.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(MyVisualHost_MouseLeftButtonUp);
		}

		public void AddV(DrawingVisual v) {
			_children.Add(v);
		}

		// Provide a required override for the VisualChildrenCount property.
		protected override int VisualChildrenCount {
			get { return _children.Count; }
		}

		// Provide a required override for the GetVisualChild method.
		protected override Visual GetVisualChild(int index) {
			if (index < 0 || index >= _children.Count) {
				throw new ArgumentOutOfRangeException();
			}

			return _children[index];
		}

		// Capture the mouse event and hit test the coordinate point value against
		// the child visual objects.
		void MyVisualHost_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			// Retrieve the coordinates of the mouse button event.
			System.Windows.Point pt = e.GetPosition((UIElement)sender);

			// Initiate the hit test by setting up a hit test result callback method.
			VisualTreeHelper.HitTest(this, null, new HitTestResultCallback(myCallback), new PointHitTestParameters(pt));
		}

		// If a child visual object is hit, toggle its opacity to visually indicate a hit.
		public HitTestResultBehavior myCallback(HitTestResult result) {
			if (result.VisualHit.GetType() == typeof(DrawingVisual)) {
				if (((DrawingVisual)result.VisualHit).Opacity == 1.0) {
					((DrawingVisual)result.VisualHit).Opacity = 0.4;
				}
				else {
					((DrawingVisual)result.VisualHit).Opacity = 1.0;
				}
			}

			// Stop the hit test enumeration of objects in the visual tree.
			return HitTestResultBehavior.Stop;
		}
	}
}
