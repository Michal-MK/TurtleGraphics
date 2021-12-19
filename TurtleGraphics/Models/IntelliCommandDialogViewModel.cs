using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TurtleGraphics.Helpers;
using TurtleGraphics.IntelliCommands;
using TurtleGraphics.Language;
using TurtleGraphics.Language.Definition;
using TurtleGraphics.Language.Keywords;

namespace TurtleGraphics.Models {
	public class IntelliCommandDialogViewModel : BaseViewModel {
		[Notify]
		public int SelectedIndex { get; set; }

		[Notify]
		public Visibility Visible { get; set; }

		[Notify]
		public ObservableCollection<IntelliCommandData> Source { get; set; }

		public static readonly ILanguageElement[] INTELLI_COMMANDS = {
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
			new ForLoop(),
			new FunctionDefinition {Name = "SetColor", Parameters = {new Parameter { Description = "Changes the brush color", Name = "color", Type = typeof(string)} }},
			new FunctionDefinition {Name = "SetColor", Parameters = {
					new Parameter { Description = "Sets the RED component of the RGB color", Name = "colorR", Type = typeof(byte)},
					new Parameter { Description = "Sets the GREEN component of the RGB color", Name = "colorG", Type = typeof(byte)},
					new Parameter { Description = "Sets the BLUE component of the RGB color", Name = "colorB", Type = typeof(byte)}
			}},
		};

		public IntelliCommandDialogViewModel() {
			
			Source = new ObservableCollection<IntelliCommandData>();
		}

		public int Handle(string fullText, int caretIndex) {
			if (fullText.Length > 0 && caretIndex > 0 && caretIndex <= fullText.Length) {
				char c = fullText[caretIndex - 1];
				if (char.IsWhiteSpace(c)) {
					Source.Clear();
					Visible = Visibility.Collapsed;
					return 0;
				}
				if (caretIndex < fullText.Length && !char.IsWhiteSpace(fullText[caretIndex])) {
					Source.Clear();
					Visible = Visibility.Collapsed;
					return 0;
				}

				string wordArtifact = GetFullWord(fullText, caretIndex - 1);
				Source.Clear();

				INTELLI_COMMANDS.ForEach(elem => { IntelliCommandData data = elem.Process(wordArtifact); if (data != null) Source.Add(data); });

				if (Source.Count > 0) {
					Visible = Visibility.Visible;
					SelectedIndex = 0;
					Source[SelectedIndex].Selected = true;
				}
			}
			else {
				Source.Clear();
				Visible = Visibility.Collapsed;
			}
			return Source.Count;
		}

		private string GetFullWord(string value, int v) {
			StringBuilder sb = new StringBuilder();
			while (v >= 0 && !char.IsWhiteSpace(value[v])) {
				sb.Insert(0, value[v]);
				v--;
			}
			return sb.ToString();
		}

		public async void TextEvents(object _, KeyEventArgs e) {
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
					await InsertIntelliCommand();
					e.Handled = true;
				}
			}
			if (CaretHelper.HasAllParameters(MainWindow.Instance.CommandsTextInput) && (e.Key == Key.Enter || e.Key == Key.Return)) {
				e.Handled = true;
				TextBox text = MainWindow.Instance.CommandsTextInput;
				int index = CaretHelper.NextNewLineIndex(text);
				HandleNewlineIndent = true;
				intermediateIndex = index + Environment.NewLine.Length;
				text.Text = text.Text.Insert(index, Environment.NewLine);
				// text.CaretIndex = index + Environment.NewLine.Length;
				HandleNewlineIndent = false;
			}
		}

		public bool HandleNewlineIndent;
		public int intermediateIndex;

		public async Task InsertIntelliCommand() {
			TextBox textBox = MainWindow.Instance.CommandsTextInput;
			int caret = textBox.CaretIndex;
			IntelliCommandData data = Source[SelectedIndex];
			if (data.Def.PreInsertEvent != null) {
				data.Def = await data.Def.PreInsertEvent.Invoke();
			}
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