using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TurtleGraphics.Models {
	public class InteliCommandDialogViewModel : BaseViewModel {
		[Notify] public int SelectedIndex { get; set; }
		[Notify] public Visibility Visible { get; set; }
		[Notify] public ObservableCollection<InteliCommandData> Source { get; set; }

		private readonly string[] _inteliCommands = new string[] {
			"Rotate({0:double});",
			"MoveTo({0:double}, {1:double});",
			"Forward({0:double});",
			"PenUp();",
			"PenDown();",
			"SetBrushSize({0:double});",
			"SetColor({0:double});",
			"SetLineCapping({0:LineCapping});",
			"StoreTurtlePosition();",
			"RestoreTurtlePosition({0:bool});",
			"CaptureScreenshot();",
		};

		public InteliCommandDialogViewModel() {
			Source = new ObservableCollection<InteliCommandData>();
		}

		public void Handle(string value, int caretPos) {
			if (value.Length > 0 && caretPos > 0 && caretPos <= value.Length) {
				char c = value[caretPos - 1];
				if (char.IsWhiteSpace(c)) {
					Source.Clear();
					Visible = Visibility.Collapsed;
					return;
				}
				if (caretPos < value.Length && !char.IsWhiteSpace(value[caretPos])) {
					Source.Clear();
					Visible = Visibility.Collapsed;
					return;
				}
				string fullWord = GetFullWord(value, caretPos - 1);
				Source.Clear();
				foreach (string cmd in _inteliCommands) {
					if (cmd.StartsWith(fullWord) && cmd.Length != fullWord.Length) {
						string suffix = cmd.Remove(0, fullWord.Length);
						Source.Add(new InteliCommandData(cmd, fullWord + suffix, suffix));
					}
				}
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
			textBox.Text = textBox.Text.Insert(caret, data.ToComplete);
			textBox.CaretIndex = caret + data.CaretPosition;
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