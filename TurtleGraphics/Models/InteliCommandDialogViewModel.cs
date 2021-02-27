using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TurtleGraphics.Models {
	public class InteliCommandDialogViewModel : BaseViewModel {

		private ObservableCollection<InteliCommandData> _source;
		private Visibility _visible;
		private int _selectedIndex;

		public int SelectedIndex { get => _selectedIndex; set { _selectedIndex = value; Notify(nameof(SelectedIndex)); } }
		public Visibility Visible { get => _visible; set { _visible = value; Notify(nameof(Visible)); } }
		public ObservableCollection<InteliCommandData> Source { get => _source; set { _source = value; Notify(nameof(Source)); } }


		private readonly string[] _inteliCommands = new string[] {
			"Rotate();" ,
			"MoveTo();" ,
			"Forward();" ,
			"PenUp();" ,
			"PenDown();" ,
			"SetBrushSize();" ,
			"SetColor();" ,
			"SetLineCapping();" ,
			"StoreTurtlePosition();" ,
			"RestoreTurtlePosition();" ,
			"CaptureScreenshot();" ,
		};

		public InteliCommandDialogViewModel() {
			Source = new ObservableCollection<InteliCommandData>();
		}

		Random tmp = new Random();

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
				for (int i = 0; i < _inteliCommands.Length; i++) {
					if (_inteliCommands[i].StartsWith(fullWord) && _inteliCommands[i].Length != fullWord.Length) {
						string suffix = _inteliCommands[i].Remove(0, fullWord.Length);
						Source.Add(new InteliCommandData(fullWord + suffix.ToString(), suffix.ToString()));
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

		public void TextEvents(object sender, KeyEventArgs e) {
			if (Visible == Visibility.Visible) {
				if (e.Key == Key.Escape) {
					e.Handled = true;
					Visible = Visibility.Collapsed;
				}
				if (e.Key == Key.Up) {
					e.Handled = true;
					Source[SelectedIndex].Selected = false;
					SelectedIndex--;
					SelectedIndex = Mod(SelectedIndex, Source.Count);
					Source[SelectedIndex].Selected = true;
				}
				if (e.Key == Key.Down) {
					e.Handled = true;
					Source[SelectedIndex].Selected = false;
					SelectedIndex++;
					SelectedIndex = Mod(SelectedIndex, Source.Count);
					Source[SelectedIndex].Selected = true;
				}
				if (e.Key == Key.Enter || e.Key == Key.Return || e.Key == Key.Tab) {
					e.Handled = true;
					TextBox t = (sender as TextBox);
					int carret = t.CaretIndex;
					InteliCommandData data = Source[SelectedIndex];
					t.Text = t.Text.Insert(carret, data.ToComplete);
					t.CaretIndex = carret + data.ToComplete.Length;
				}
			}
		}

		private int Mod(int k, int n) { return ((k %= n) < 0) ? k + n : k; }
	}
}
