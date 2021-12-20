﻿using System.Collections.Generic;
using System.Threading;
using TurtleGraphics.Language.Logic;
using TurtleGraphics.ParsedData.Base;
using TurtleGraphics.Parsers;

namespace TurtleGraphics.ParsedData {
	public class RestorePositionData : BaseParsedData {

		public RestorePositionData(ParameterValuation[] args, Dictionary<string, object> variables, string line) : base(variables, line, args) {
			if (Arg1 != null) {
				IsPop = bool.Parse(Arg1);
			}
		}

		public bool IsPop { get; set; }

		public override bool IsBlock => false;

		public override string Line { get; set; }

		public override ParsedAction Action => ParsedAction.RestorePos;

		public override TurtleData Compile(CancellationToken token) {
			return new TurtleData {
				Action = Action,
				PopPosition = IsPop
			};
		}

		public override IList<TurtleData> CompileBlock(CancellationToken token) {
			throw new System.NotImplementedException();
		}
	}
}