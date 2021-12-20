using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Flee.PublicTypes;
using TurtleGraphics.ParsedData.Base;
using TurtleGraphics.Parsers;

namespace TurtleGraphics.ParsedData {
	public class ConditionalData : BaseParsedData {

		public IGenericExpression<bool> IfCondition { get; set; }
		public Queue<BaseParsedData> IfData { get; set; }
		public Queue<BaseParsedData> ElseData { get; set; }
		public IList<(IGenericExpression<bool>, Queue<BaseParsedData>)> ElseIfs { get; set; }
		public bool IsModifiable { get; set; } = true;
		public int LineIndex { get; set; }

		public override bool IsBlock => true;

		public override ParsedAction Action => ParsedAction.NONE;

		public override string Line { get; set; }

		public ConditionalData(string line, IGenericExpression<bool> ifCondition, Queue<BaseParsedData> data, Dictionary<string, object> variables) : base(variables, line/*,line TODO*/) {
			IfCondition = ifCondition;
			IfData = data;
			Line = line;
		}

		public void AddElse(StringReader reader, string line) {
			if (!line.EndsWith("{")) {
				BlockParser.ReadToBlock(reader, line);
			}
			List<string> lines = BlockParser.ParseBlock(reader);

			Queue<BaseParsedData> data = CommandParser.Parse(string.Join(Environment.NewLine, lines), CommandParser.Window, Variables);
			ElseData = data;
		}

		public override IList<TurtleData> CompileBlock(CancellationToken token) {
			List<TurtleData> ret = new List<TurtleData>(128);
			UpdateVars(IfCondition);

			if (IfCondition.Evaluate()) {
				ret.AddRange(CompileQueue(IfData, token));
			}
			else {
				if (ElseData != null) {
					ret.AddRange(CompileQueue(ElseData, token));
				}
			}
			return ret;
		}

		private IEnumerable<TurtleData> CompileQueue(Queue<BaseParsedData> data, CancellationToken token) {
			List<TurtleData> interData = new List<TurtleData>();
			BaseParsedData current;
			int counter = 0;
			if (data.Count == 0) {
				return interData;
			}

			while (data.Count > 0) {
				token.ThrowIfCancellationRequested();
				current = data.Dequeue();
				counter++;
				if (current.IsBlock) {
					interData.AddRange(current.CompileBlock(token));
				}
				else {
					interData.Add(current.Compile(token));
				}
				data.Enqueue(current);
				if (counter == data.Count) {
					return interData;
				}
			}
			throw new Exception();
		}

		public override TurtleData Compile(CancellationToken token) {
			throw new NotImplementedException();
		}
	}
}