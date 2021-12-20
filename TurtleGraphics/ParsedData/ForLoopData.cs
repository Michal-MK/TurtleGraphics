using System;
using System.Collections.Generic;
using System.Threading;
using Flee.PublicTypes;
using TurtleGraphics.Helpers;
using TurtleGraphics.Language.Logic;
using TurtleGraphics.ParsedData.Base;
using TurtleGraphics.Parsers;

namespace TurtleGraphics.ParsedData {
	public class ForLoopData : BaseParsedData {
		public IGenericExpression<int> From { get; set; }
		public IGenericExpression<int> To { get; set; }
		public string LoopVariable { get; set; }
		public IGenericExpression<int> Change { get; set; }
		public OperatorType Operator { get; set; }
		public ConditionType Condition { get; set; }

		public List<string> Lines { get; set; }

		public override bool IsBlock => true;

		public override ParsedAction Action => ParsedAction.NONE;

		public int LineIndex { get; set; }
		public override string Line { get; set; }

		public ForLoopData(Dictionary<string, object> dictionary, string line) : base(dictionary, line) { }

		public override TurtleData Compile(CancellationToken token) {
			throw new NotImplementedException();
		}

		public override IList<TurtleData> CompileBlock(CancellationToken token) {
			List<TurtleData> ret = new List<TurtleData>(4096);
			List<BaseParsedData> loopContents = CompileLoop();

			ret.AddRange(CompileQueue(loopContents, token));
			return ret;
		}

		private IEnumerable<TurtleData> CompileQueue(List<BaseParsedData> data, CancellationToken token) {
			List<TurtleData> interData = new List<TurtleData>();
			BaseParsedData current;

			UpdateVars(From);
			UpdateVars(To);

			int FromInt = From.Evaluate();
			int ToInt = To.Evaluate();
			int ChangeInt = 1;
			if (Operator == OperatorType.MinusEquals || Operator == OperatorType.PlusEquals) {
				UpdateVars(Change);
				ChangeInt = Change.Evaluate();
			}

			void Exec(int i) {
				token.ThrowIfCancellationRequested();
				for (int counter = 0; counter < data.Count; counter++) {
					current = data[counter];

					current.Variables[LoopVariable] = i;
					foreach (var item in Variables) {
						current.Variables[item.Key] = item.Value;
					}

					if (current.IsBlock) {
						interData.AddRange(current.CompileBlock(token));
					}
					else if (current is VariableData variableChange) {
						current.UpdateVars(variableChange.Value);
						Variables[variableChange.VariableName] = variableChange.Value.Evaluate();
					}
					else {
						interData.Add(current.Compile(token));
					}
				}
			}

			switch (Condition) {
				case ConditionType.Greater: {
					if (Operator == OperatorType.PlusPlus) {
						for (int i = FromInt; i > ToInt; i++) {
							Exec(i);
						}
					}
					if (Operator == OperatorType.PlusEquals) {
						for (int i = FromInt; i > ToInt; i += ChangeInt) {
							Exec(i);
						}
					}
					if (Operator == OperatorType.MinMin) {
						for (int i = FromInt; i > ToInt; i--) {
							Exec(i);
						}
					}
					if (Operator == OperatorType.MinusEquals) {
						for (int i = FromInt; i > ToInt; i -= ChangeInt) {
							Exec(i);
						}
					}
					break;
				}
				case ConditionType.Less: {
					if (Operator == OperatorType.PlusPlus) {
						for (int i = FromInt; i < ToInt; i++) {
							Exec(i);
						}
					}
					if (Operator == OperatorType.PlusEquals) {
						for (int i = FromInt; i < ToInt; i += ChangeInt) {
							Exec(i);
						}
					}
					if (Operator == OperatorType.MinMin) {
						for (int i = FromInt; i < ToInt; i--) {
							Exec(i);
						}
					}
					if (Operator == OperatorType.MinusEquals) {
						for (int i = FromInt; i < ToInt; i -= ChangeInt) {
							Exec(i);
						}
					}
					break;
				}
				case ConditionType.GreaterOrEqual: {
					if (Operator == OperatorType.PlusPlus) {
						for (int i = FromInt; i >= ToInt; i++) {
							Exec(i);
						}
					}
					if (Operator == OperatorType.PlusEquals) {
						for (int i = FromInt; i >= ToInt; i += ChangeInt) {
							Exec(i);
						}
					}
					if (Operator == OperatorType.MinMin) {
						for (int i = FromInt; i >= ToInt; i--) {
							Exec(i);
						}
					}
					if (Operator == OperatorType.MinusEquals) {
						for (int i = FromInt; i >= ToInt; i -= ChangeInt) {
							Exec(i);
						}
					}
					break;
				}
				case ConditionType.LessOrEqual: {
					if (Operator == OperatorType.PlusPlus) {
						for (int i = FromInt; i <= ToInt; i++) {
							Exec(i);
						}
					}
					if (Operator == OperatorType.PlusEquals) {
						for (int i = FromInt; i <= ToInt; i += ChangeInt) {
							Exec(i);
						}
					}
					if (Operator == OperatorType.MinMin) {
						for (int i = FromInt; i <= ToInt; i--) {
							Exec(i);
						}
					}
					if (Operator == OperatorType.MinusEquals) {
						for (int i = FromInt; i <= ToInt; i -= ChangeInt) {
							Exec(i);
						}
					}
					break;
				}
			}
			return interData;
		}

		private List<BaseParsedData> CompileLoop() {
			List<BaseParsedData> singleIteration = new List<BaseParsedData>();

			Queue<BaseParsedData> data = CommandParser.Parse(
				string.Join(Environment.NewLine, Lines),
				CommandParser.Window,
				HelpersFunctions.Join(Variables, new Dictionary<string, object> { { LoopVariable, 0 } }));

			singleIteration.AddRange(data);

			return singleIteration;
		}
	}
}