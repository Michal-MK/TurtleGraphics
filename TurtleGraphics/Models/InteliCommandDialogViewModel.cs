using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TurtleGraphics.Definition;
using TurtleGraphics.InteliCmmands;
using TurtleGraphics.XAML;

namespace TurtleGraphics.Models {
	public class InteliCommandDialogViewModel : BaseViewModel {
		[Notify]
		public int SelectedIndex { get; set; }

		[Notify]
		public Visibility Visible { get; set; }

		[Notify]
		public ObservableCollection<InteliCommandData> Source { get; set; }

		private readonly FunctionDefinition[] _intelliCommands = {
			new FunctionDefinition {Name = "Rotate", Parameters = {new Parameter{Description = "Angle in degrees", Name = "angle", Type = typeof(double)}}},
			new FunctionDefinition {Name = "Forward", Parameters = {new Parameter{Description = "Number of pixels", Name = "distance", Type = typeof(double)}}},
			new FunctionDefinition {Name = "MoveTo", Parameters = {
				new Parameter{Description = "Absolute position on screen in pixels X coordinate", Name = "x_coord", Type = typeof(double)},
				new Parameter{Description = "Absolute position on screen in pixels Y coordinate", Name = "y_coord", Type = typeof(double)}
			}},
			new FunctionDefinition {Name = "CaptureScreenshot", Parameters = {}},
			new FunctionDefinition {Name = "PenUp", Parameters = {}},
			new FunctionDefinition {Name = "PenDown", Parameters = {}},
			new FunctionDefinition {Name = "StoreTurtlePosition", Parameters = {}},
			new FunctionDefinition {Name = "RestoreTurtlePosition", Parameters = {new Parameter{Description = "Keep position in memory, able to return repeatedly", Name = "keepInMem", Type = typeof(bool)}}},
			new FunctionDefinition {Name = "SetBrushSize", Parameters = {new Parameter{Description = "Pen radius in pixels", Name = "radius", Type = typeof(double)}}},
			new FunctionDefinition {Name = "SetLineCapping", Parameters = {new Parameter{Description = "Pen capping style", Name = "capStyle", Type = typeof(string)}}},
		};

		public InteliCommandDialogViewModel() {
			Source = new ObservableCollection<InteliCommandData>();
		}

		public void Handle(string value, TextBox input, InteliCommandsDialog commandsView, double controlAreaWidth) {
			if (value.Length > 0 && input.CaretIndex > 0 && input.CaretIndex <= value.Length) {
				char c = value[input.CaretIndex - 1];
				if (char.IsWhiteSpace(c)) {
					Source.Clear();
					Visible = Visibility.Collapsed;
					return;
				}
				if (input.CaretIndex < value.Length && !char.IsWhiteSpace(value[input.CaretIndex])) {
					Source.Clear();
					Visible = Visibility.Collapsed;
					return;
				}
				string fullWord = GetFullWord(value, input.CaretIndex - 1);
				Source.Clear();
				foreach (FunctionDefinition cmd in _intelliCommands) {
					if (cmd.Name.StartsWith(fullWord) && cmd.Name.Length != fullWord.Length) {
						string suffix = cmd.Name.Remove(0, fullWord.Length);
						Source.Add(new InteliCommandData(cmd, fullWord + suffix, suffix));
					}
				}
				if (Source.Count > 0) {
					Visible = Visibility.Visible;
					Rect r = input.GetRectFromCharacterIndex(input.CaretIndex - 1);
					Canvas.SetTop(commandsView, r.Top + r.Height);
					Canvas.SetLeft(commandsView, 0);

					commandsView.Width = controlAreaWidth;
					commandsView.Height = 60;
					SelectedIndex = 0;
					Source[SelectedIndex].Selected = true;
				}
			}
			else {
				Source.Clear();
				Visible = Visibility.Collapsed;
			}
		}

		private string GetFullWord(string value, int v) {
			StringBuilder sb = new StringBuilder();
			while (v >= 0 && !char.IsWhiteSpace(value[v])) {
				sb.Insert(0, value[v]);
				v--;
			}
			return sb.ToString();
		}

		public void TextEvents(object _, KeyEventArgs e) {
			if (Visible == Visibility.Visible) {
				if (e.Key == Key.Escape) {
					e.Handled = true;
					Visible = Visibility.Collapsed;
				}
				if (e.Key == Key.Up) {
					SetSelected(SelectedIndex - 1);
					e.Handled = true;
				}
				if (e.Key == Key.Down) {
					SetSelected(SelectedIndex + 1);
					e.Handled = true;
				}
				if (e.Key == Key.Enter || e.Key == Key.Return || e.Key == Key.Tab) {
					InsertInteliCommand();
					e.Handled = true;
				}
			}
		}

		public void InsertInteliCommand() {
			TextBox textBox = MainWindow.Instance.CommandsTextInput;
			int caret = textBox.CaretIndex;
			InteliCommandData data = Source[SelectedIndex];
			textBox.Text = textBox.Text.Insert(caret, data.FullInsertText);
			textBox.CaretIndex = data.GetCaretPos(caret);
		}

		public void SetSelected(int index) {
			Source[SelectedIndex].Selected = false;
			SelectedIndex = index;
			SelectedIndex = Mod(SelectedIndex, Source.Count);
			Source[SelectedIndex].Selected = true;
		}

		private int Mod(int k, int n) {
			return ((k %= n) < 0) ? k + n : k;
		}
	}
}