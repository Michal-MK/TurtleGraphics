using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TurtleGraphics.XAML {
	/// <summary>
	/// Interaction logic for MainWindowEmpty.xaml
	/// </summary>
	public partial class MainWindowEmpty : Window {

		public MainWindowEmpty() {
			InitializeComponent();
			//AddHandler(MouseDownEvent, new MouseButtonEventHandler(VisualParent_MouseDown), true);
			AddHandler(MouseMoveEvent, new MouseEventHandler(MouseMoveEventHandle), true);
		}

		Point? prev;

		private void MouseMoveEventHandle(object sender, MouseEventArgs e) {
			Point p = e.GetPosition(this);
			
			if(prev == null) {
				prev = p;
				return;
			}

			Vector d = p - prev.Value;

			if (e.LeftButton == MouseButtonState.Pressed) {
				DrawingVisual v = CreateDrawingVisualRectangle(p.X, p.Y, d.X, d.Y, w: 2); 
				visualHost.AddV(v);
			}

			prev = p;
		}

		private void VisualParent_MouseDown(object sender, MouseButtonEventArgs e) {
			Point p = e.GetPosition(this);
			DrawingVisual v = CreateDrawingVisualRectangle(p.X, p.Y, w:2); visualHost.AddV(v);
		}

		// Create a DrawingVisual that contains a rectangle.
		private DrawingVisual CreateDrawingVisualRectangle(double x = 160, double y = 100, double dX = 320, double dY = 80, int w = 10) {
			DrawingVisual drawingVisual = new DrawingVisual();

			// Retrieve the DrawingContext in order to create new drawing content.
			DrawingContext drawingContext = drawingVisual.RenderOpen();

			Pen p = new Pen(Brushes.Black, w);

			// Create a rectangle and draw it in the DrawingContext.
			drawingContext.DrawLine(p,new Point(x,y), new Point(x + dX, y + dY));

			// Persist the drawing content.
			drawingContext.Close();

			return drawingVisual;
		}

		private void visualHost_MouseDown(object sender, MouseButtonEventArgs e) {
			DrawingVisual v = CreateDrawingVisualRectangle(); visualHost.AddV(v);
		}

		private void visualHost_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
			DrawingVisual v = CreateDrawingVisualRectangle(); visualHost.AddV(v);
		}
	}
}
