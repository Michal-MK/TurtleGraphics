﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Media;

namespace TurtleGraphics {
	public class BrushCappingData : ParsedData {
		public BrushCappingData(string[] args, Dictionary<string, object> variables, string line) : base(variables, line, args) { }

		public override bool IsBlock => false;

		public override string Line { get; set; }

		public override ParsedAction Action => ParsedAction.Capping;

		public override bool Cacheable => true;

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

		public override IList<TurtleData> CompileBlock(CancellationToken token, Dictionary<int, LineCacheData> cache) {
			throw new NotImplementedException();
		}
	}
}
