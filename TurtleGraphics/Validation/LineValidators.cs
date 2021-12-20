using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TurtleGraphics.Exceptions;
using TurtleGraphics.Language;
using TurtleGraphics.Language.Logic;
using TurtleGraphics.Parsers;

namespace TurtleGraphics.Validation {
	public static class LineValidators {
		public static bool IsAssignment(string line, out AssignmentInfo assignment) {
			assignment = null;

			string[] split = line.Split(new[] { ' ' }, 2);
			string[] typeName = split[0].Trim().Split();

			AssignmentInfo i = new AssignmentInfo();
			if (!IsType(typeName[0], out Type t)) {
				return false;
			}
			i.Type = t;
			i.VariableName = typeName[1];
			i.Value = split[1].TrimEnd(';');
			assignment = i;
			return true;
		}

		public static bool IsType(string line, out Type t) {
			t = null;
			if (SupportedTypes.TYPE_DICT.ContainsKey(line)) {
				t = SupportedTypes.TYPE_DICT[line];
				return true;
			}
			return false;
		}

		public static bool IsFunctionCall(string line, out FunctionCall info) {

			//test (20);
			//teset(ToDeg(Sin(PI))); 
			//Func(RandX(),Rand(20,50));
			//for (int i = 0; i < 20, i++)
			//if (True)
			//int x = Rand(0,100);
			info = null;
			string[] split = line.Split(new[] { '(' }, 2, StringSplitOptions.RemoveEmptyEntries);

			//test  20);
			//teset ToDeg(Sin(PI))); 
			//Func RandX(),Rand(20,50));
			//for int i = 0; i < 20, i++)
			//if True)
			//int x = Rand 0,100);

			if (split.Length < 2)
				return false;

			split[0] = split[0].TrimEnd();

			if (FunctionNames.Functions.Contains(split[0]) && !split[1].EndsWith(";")) {
				throw new ParsingException("Missing semicolon after a function call!", line);
			}

			split[1] = split[1].TrimEnd(';', ' ');

			//test 20)
			//teset ToDeg(Sin(PI)))
			//Func RandX(),Rand(20,50))
			//for int i = 0; i < 20, i++)
			//if True)
			//int x = Rand 0,100)

			split[1] = split[1].Remove(split[1].Length - 1, 1);

			//test 20
			//teset ToDeg(Sin(PI))
			//Func RandX(),Rand(20,50)
			//for int i = 0; i < 20, i++
			//if True
			//int x = Rand 0,100

			if (IsType(split[1].Split()[0], out _))
				return false;

			if (split[0].TrimEnd() == "for" || split[0].TrimEnd() == "if")
				return false;

			if (split[0].Contains("=")) {
				return false;
			}

			FunctionCall i = new FunctionCall();
			i.FunctionDef = FunctionNames.Parse(split[0].TrimEnd());

			i.Arguments = ArgParser.Parse(split[1].TrimEnd());

			info = i;
			return true;
		}

		public static bool IsForLoop(string line) {
			string[] split = line.Split('(', ')');
			bool startsWithFor = line.StartsWith("for");

			if (!startsWithFor) return false;

			bool isValidType = IsType(split[1].Split()[0], out _);

			return split.Length >= 3 && isValidType;
		}

		internal static bool IsConditional(string line) {
			string[] split = line.Split('(', ')');
			bool isIf = split.Length == 3 && (split[0].Trim() == "if");
			bool isElseIf = split.Length == 3 && (split[0].Trim() == "else if");
			bool isElse = split.Length == 1 && split[0].TrimEnd(' ', '{') == "else";
			return isIf || isElseIf || isElse;
		}

		internal static bool IsVariableDeclaration(string line, Dictionary<string, object> variables, out (string, string, string) data) {
			data = (null, null, null);
			string[] values = line.Split('=');
			if (values.Length != 2) {
				return false;
			}
			values[0] = values[0].Trim();
			values[1] = values[1].Trim();
			string[] typeName = values[0].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			if (typeName.Length == 2) {
				typeName[0] = typeName[0].TrimEnd();
				typeName[1] = typeName[1].TrimStart();
				if (!SupportedTypes.TYPE_DICT.Keys.ToList().Contains(typeName[0])) {
					return false;
				}
				if (typeName[1] == "Width" || typeName[1] == "Height" || variables.ContainsKey(typeName[1])) {
					throw new ParsingException($"'{typeName[1]}' is already defined in an outer scope!", line);
				}
				if (!values[1].EndsWith(";")) {
					return false;
				}
				values[1] = values[1].TrimStart().TrimEnd(' ', ';');
				data = (typeName[0], typeName[1], values[1]);
				return true;
			}

			//Variable assignment
			if (IsVariableAssignment(new[] { typeName[0].TrimEnd(), values[1] }, line, variables, out (string, string) assignData)) {
				data = (null, assignData.Item1, assignData.Item2);
				return true;
			}
			return false;
		}

		private static bool IsVariableAssignment(string[] info, string line, Dictionary<string, object> variables, out (string, string) data) {
			if (!variables.ContainsKey(info[0])) {
				throw new ParsingException("Unable to assign value to an undefined variable!", line);
			}
			if (info[0] == "Width" || info[0] == "Height") {
				throw new ParsingException($"'{info[0]}' is a read-only variable!", line);
			}
			if (!info[1].EndsWith(";")) {
				throw new ParsingException($"Missing a semicolon at the end of variable assignment!", line);
			}
			info[1] = info[1].TrimEnd(' ', ';');
			data = (info[0].Trim(), info[1].TrimStart());
			return true;
		}

		public static bool IsEmptyLine(string value, int caret) {
			while (char.IsWhiteSpace(value[caret])) {
				if (value[caret] == Environment.NewLine[0]) {
					return true;
				}
				caret++;
			}
			return false;
		}
	}
}