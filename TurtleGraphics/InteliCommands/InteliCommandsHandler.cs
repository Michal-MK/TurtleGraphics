using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using TurtleGraphics.Validation;

namespace TurtleGraphics {
	public class InteliCommandsHandler {

		#region InteliCommands
		private readonly Dictionary<string, string> _inteliCommands = new Dictionary<string, string> {
			{ "for", " (int _ = 0; _ < ; _++) {" + Environment.NewLine + "{0}" + Environment.NewLine + "{1}" + "}" },
			{ "if", " () {" + Environment.NewLine + "{0}" +  Environment.NewLine + "{1}" + "}" },
			{ "R", "otate();" },
			{ "M", "oveTo();" },
			{ "F", "orward();" },
			{ "PenU", "p();" },
			{ "PenD", "own();" },
			{ "SetB", "rushSize();" },
			{ "SetC", "olor();" },
			{ "SetL", "ineCapping();" },
			{ "St", "oreTurtlePosition();" },
			{ "Re", "storeTurtlePosition();" },
			{ "Ca", "ptureScreenshot();" },
		};

		private readonly Dictionary<string, int> _inteliCommandsIndexes = new Dictionary<string, int> {
			{ "for", 17  },
			{ "if", 2 },
			{ "R", 6 },
			{ "M", 6 },
			{ "F", 7 },
			{ "PenU", 2 },
			{ "PenD", 4 },
			{ "SetB", 9 },
			{ "SetC", 5 },
			{ "SetL", 11 },
			{ "St", 20 },
			{ "Re", 20 },
			{ "Ca", 18 }
		};

		private readonly Dictionary<string, Func<string, int, int, ModificationData>> _inteliCommandModifications;


		public InteliCommandsHandler() {
			_inteliCommandModifications = new Dictionary<string, Func<string, int, int, ModificationData>> {
				{ "for", ForLoopVariableName }
			};
		}

		public string GetInteliCommand(string value) {
			if (_inteliCommands.ContainsKey(value)) {
				return value + _inteliCommands[value];
			}
			return value;
		}

		public int GetIndexForCaret(string value) {
			if (_inteliCommands.ContainsKey(value)) {
				return value.Length + _inteliCommandsIndexes[value];
			}
			throw new NotImplementedException();
		}

		#endregion

		public InteliCommandsState State { get; set; } = InteliCommandsState.Normal;
		private string _triggerCommand;
		private int _addedLinesCount;
		private int _addedLinesIndex;
		private bool _ignoreEvent = false;
		private int _textLength;
		private int _currentIndentLevel;

		public void Handle(MainWindow window, TextBox inputControl) {
			if (_ignoreEvent) {
				_ignoreEvent = false;
				return;
			}
			string newText = inputControl.Text;

			string region = newText.Substring(0, inputControl.CaretIndex);
			_currentIndentLevel = region.Count(s => s == '{') - region.Count(s => s == '}');

			if (State == InteliCommandsState.Normal) {
				int carret = inputControl.CaretIndex;
				if (IsValidCommand(newText, carret, out (int start, int length) values)) {
					_triggerCommand = newText.Substring(values.start, values.length);

					string commandFull = GetInteliCommand(_triggerCommand);
					commandFull = commandFull.Replace("{0}", new string(' ', 3 * (_currentIndentLevel + 1)));
					commandFull = commandFull.Replace("{1}", new string(' ', 3 * _currentIndentLevel));

					_addedLinesCount = (commandFull.Split('\n').Length - 1) * Environment.NewLine.Length;
					_addedLinesIndex = inputControl.CaretIndex;
					State = InteliCommandsState.IsSuggesting;

					window.InteliCommandsText = newText.Remove(values.start, values.length).Insert(values.start, commandFull);
					_ignoreEvent = true;
					window.CommandsText = newText.Insert(_addedLinesIndex, string.Concat(Enumerable.Repeat(Environment.NewLine, _addedLinesCount / Environment.NewLine.Length)));
					_ignoreEvent = true;
					inputControl.CaretIndex = carret;
				}
				else {
					if (carret > 0 && inputControl.Text[carret - 1] == '}') {
						int indent = CountIndent(inputControl, carret - 2);
						if (indent > 3 * _currentIndentLevel) {
							int difference = Math.Abs(indent - 3 * _currentIndentLevel);
							if (_currentIndentLevel < 0) {
								window.InteliCommandsText = newText;
								window.CommandsText = newText;
							}
							else {
								_ignoreEvent = true;
								window.CommandsText = inputControl.Text.Remove(carret - 1 - indent, difference);
								_ignoreEvent = true;
								inputControl.CaretIndex = carret - difference;
							}
						}
						else {
							window.InteliCommandsText = newText;
							window.CommandsText = newText;
						}
					}
					else {
						window.InteliCommandsText = newText;
						window.CommandsText = newText;
					}
				}
				_textLength = window.CommandsText.Length;
			}
			else {
				int carret = inputControl.CaretIndex;
				int lastChar = inputControl.CaretIndex - 1;

				if (lastChar > 0 && inputControl.SelectionLength == 0 && newText[lastChar] == '\t' && carret == _addedLinesIndex + 1) {
					_ignoreEvent = true;

					if (_inteliCommandModifications.ContainsKey(_triggerCommand)) {
						ModificationData data = _inteliCommandModifications[_triggerCommand](window.InteliCommandsText, carret, window.InteliCommandsText.Length - window.CommandsText.Length);
						window.CommandsText = data.Text;
						_ignoreEvent = true;
						int carretIndexOffset = GetIndexForCaret(_triggerCommand) + (-1 + data.InputLength);
						inputControl.CaretIndex = lastChar - _triggerCommand.Length + carretIndexOffset;
					}
					else {
						window.CommandsText = window.InteliCommandsText;
						_ignoreEvent = true;
						int carretIndexOffset = GetIndexForCaret(_triggerCommand);
						inputControl.CaretIndex = lastChar - _triggerCommand.Length + carretIndexOffset;
					}
				}
				else {
					if (lastChar < 0) {
						State = InteliCommandsState.Normal;
						window.InteliCommandsText = newText;
						window.CommandsText = newText;
						return;
					}

					int selectionLen = inputControl.SelectionLength;
					int selectionIndex = inputControl.SelectionStart;
					_ignoreEvent = true;
					if (_textLength > newText.Length) {
						window.InteliCommandsText = newText.Remove(_addedLinesIndex - 1, _addedLinesCount);
						window.CommandsText = newText.Remove(_addedLinesIndex - 1, _addedLinesCount);
					}
					else if (_textLength < newText.Length) {
						window.InteliCommandsText = newText.Remove(_addedLinesIndex + 1, _addedLinesCount);
						window.CommandsText = newText.Remove(_addedLinesIndex + 1, _addedLinesCount);
					}
					else {
						window.InteliCommandsText = newText.Remove(_addedLinesIndex, _addedLinesCount);
						window.CommandsText = newText.Remove(_addedLinesIndex, _addedLinesCount);
					}
					if (selectionLen != 0) {
						_ignoreEvent = true;
						inputControl.SelectionStart = selectionIndex;
						_ignoreEvent = true;
						inputControl.SelectionLength = selectionLen;
					}
					else {
						_ignoreEvent = true;
						inputControl.CaretIndex = carret;
					}
				}
				State = InteliCommandsState.Normal;
				Handle(window, inputControl);
			}
		}

		private int CountIndent(TextBox inputControl, int index) {
			int counter = 0;
			while (index >= 0) {
				if (inputControl.Text[index] == ' ') {
					counter++;
					index--;
				}
				else {
					break;
				}
			}
			return counter;
		}

		private bool IsValidCommand(string value, int carret, out (int, int) substringInfo) {
			substringInfo = (0, 0);

			int lastChar = carret - 1;


			if (lastChar < 0) {
				return false;
			}

			while (lastChar >= 0 && !char.IsWhiteSpace(value[lastChar]) && !Environment.NewLine.Contains(value[lastChar])) {
				lastChar--;
			}

			int whiteSpaceCount = 0;
			while (lastChar >= 0 && value[lastChar] == ' ') {
				lastChar--;
				whiteSpaceCount++;
			}


			if (lastChar >= 0 && value[lastChar] != '\n') {
				return false;
			}

			lastChar++;

			string possibleCommand = value.Substring(lastChar + whiteSpaceCount, carret - (lastChar + whiteSpaceCount));

			if (carret < value.Length) {
				if (!LineValidators.IsEmptyLine(value, carret)) {
					return false;
				}
			}

			if (_inteliCommands.ContainsKey(possibleCommand)) {
				substringInfo = (lastChar + whiteSpaceCount, carret - (lastChar + whiteSpaceCount));
				return true;
			}
			return false;
		}

		public ModificationData ForLoopVariableName(string str, int start, int count) {
			ForLoopVarNameDialog d = new ForLoopVarNameDialog();
			d.Owner = MainWindow.Instance;
			d.VarName = "i";
			bool? res = d.ShowDialog();

			string s = str.Substring(0, start);
			string m = str.Substring(start, count);
			string l = str.Substring(start + count);

			if (!res.HasValue || !res.Value) {
				return new ModificationData(s + m.Replace("_", "i") + l, 1);
			}
			return new ModificationData(s + m.Replace("_", d.VarName) + l, d.VarName.Length == 1 ? 1 : (d.VarName.Length * 2) - 1);
		}
	}
}