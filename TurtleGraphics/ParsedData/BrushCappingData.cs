using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Media;
using TurtleGraphics.Exceptions;
using TurtleGraphics.Language.Logic;
using TurtleGraphics.ParsedData.Base;
using TurtleGraphics.Parsers;

namespace TurtleGraphics.ParsedData {
	public class BrushCappingData : BaseParsedData {
		public BrushCappingData(ParameterValuation[] args, Dictionary<string, object> variables, string line) : base(variables, line, args) { }

		public override bool IsBlock => false;

		public override string Line { get; set; }

		public override ParsedAction Action => ParsedAction.Capping;

		public override TurtleData Compile(CancellationToken token) {
			token.ThrowIfCancellationRequested();

			if (!new List<string>(Enum.GetNames(typeof(PenLineCap))).Contains(Arg1)) {
				throw new ParsingException("Invalid Brush cap, Expected one of: " + string.Join(", ", Enum.GetNames(typeof(PenLineCap))), Line);
			}

			return new TurtleData {
				Action = Action,
				LineCap = (PenLineCap)Enum.Parse(typeof(PenLineCap), Arg1),
			};
		}

		public override IList<TurtleData> CompileBlock(CancellationToken token) {
			throw new NotImplementedException();
		}
	}
}