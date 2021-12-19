using System;
using System.Linq;
using System.Windows.Controls;
using TurtleGraphics.Language.Definition;
using TurtleGraphics.Models;

namespace TurtleGraphics.Helpers {
	public static class CaretHelper {
		public static bool HasAllParameters(TextBox input) {
			bool inFuncParams = IsInFunctionParams(input, out FunctionDefinition func);

			if (inFuncParams) {
				int nParams = func.Parameters.Count;
				string rTrimmed = input.Text.Substring(0, input.CaretIndex);
				string paramContents = rTrimmed.Substring(rTrimmed.LastIndexOf('(') + 1);
				int commaIndices = paramContents.AllIndicesOf(',').Length;
				if (nParams == 1) {
					return paramContents.Length != 0;
				}
				return nParams == commaIndices - 1 && !paramContents.EndsWith(",");
			}
			return false;
		}

		private static bool IsInFunctionParams(TextBox input, out FunctionDefinition functionDefinition) {
			int openBracketIndex = input.Text.Substring(0, input.CaretIndex).LastIndexOf('(');
			functionDefinition = null;
			if (openBracketIndex == -1) {
				return false;
			}
			int closedBracketIndex = input.Text.Substring(openBracketIndex, input.CaretIndex - openBracketIndex).IndexOf(')');
			if (closedBracketIndex != -1) {
				return false;
			}
			string preBracketContent = GetWordBefore(input, openBracketIndex);
			foreach (FunctionDefinition languageElement in IntelliCommandDialogViewModel.INTELLI_COMMANDS.OfType<FunctionDefinition>()) {
				if (languageElement.Name == preBracketContent) {
					functionDefinition = languageElement;
					return true;
				}
			}
			return false;
		}

		private static string GetWordBefore(TextBox input, int charIndex) {
			string text = input.Text;
			int start = charIndex;
			while (charIndex >= 0) {
				if (charIndex == 0) {
					return text.Substring(0, start);
				}
				if (char.IsWhiteSpace(text[charIndex])) {
					return text.Substring(charIndex + 1, start - charIndex - 1);
				}
				charIndex--;
			}
			return "";
		}

		public static int NextNewLineIndex(TextBox text) {
			int pos = text.CaretIndex;
			while (pos < text.Text.Length) {
				if (text.Text[pos] == Environment.NewLine[0]) {
					return pos;
				}
				pos++;
			}
			return text.Text.Length;
		}
	}
}