using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Flee.PublicTypes;

namespace TurtleGraphics {
	public class ForLoopData : ParsedData {
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

		public override IList<TurtleData> CompileBlock(CancellationToken token, Dictionary<int, LineCacheData> cache) {
			List<TurtleData> ret = new List<TurtleData>();
			List<ParsedData> loopContents = CompileLoop().Result;

			ret.AddRange(CompileQueue(loopContents, token, cache));
			return ret;
		}

		private IEnumerable<TurtleData> CompileQueue(List<ParsedData> data, CancellationToken token, Dictionary<int, LineCacheData> cache) {
			List<TurtleData> interData = new List<TurtleData>();
			ParsedData current;

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
						interData.AddRange(current.CompileBlock(token, cache));
					}
					else if (current is VariableData variableChange) {
						current.UpdateVars(variableChange.Value);
						Variables[variableChange.VariableName] = variableChange.Value.Evaluate();
					}
					else {
						bool cached = cache.ContainsKey(current.LineHash);
						if (cached && !cache[current.LineHash].ContainsVariable && cache[current.LineHash].ParsingInfo.Parameters[0].ToLower() != "random") {
							interData.Add(cache[current.LineHash].CompiledData);
						}
						else {
							TurtleData compiled = current.Compile(token);
							if (!cached) {
								cache.Add(current.LineHash, new LineCacheData(current, compiled));
							}
							interData.Add(compiled);
						}
						//interData.Add(current.Compile(token));
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


		private List<ParsedData> parsedDataC;
		private async Task<List<ParsedData>> CompileLoop() {
			if (parsedDataC != null)
				return parsedDataC;

			parsedDataC = new List<ParsedData>();

			Queue<ParsedData> data = await CommandParser.ParseAsync(
				string.Join(Environment.NewLine, Lines),
				CommandParser.Window,
				Helpers.Join(Variables, new Dictionary<string, object> { { LoopVariable, 0 } }));

			parsedDataC.AddRange(data);

			return parsedDataC;
		}
	}
}
