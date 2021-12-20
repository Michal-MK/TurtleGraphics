﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using TurtleGraphics.Language.Logic;
using TurtleGraphics.ParsedData.Base;
using TurtleGraphics.Parsers;

namespace TurtleGraphics.ParsedData {
	public class StoredPositionData : BaseParsedData {

		public StoredPositionData(ParameterValuation[] args, Dictionary<string, object> variables, string line) : base(variables, line, args) { }

		public override bool IsBlock => false;

		public override string Line { get; set; }

		public override ParsedAction Action => ParsedAction.StorePos;

		public Point Point { get; set; }
		public double Angle { get; set; }

		public override TurtleData Compile(CancellationToken token) {
			return new TurtleData {
				Action = Action
			};
		}

		public override IList<TurtleData> CompileBlock(CancellationToken token) {
			throw new NotImplementedException();
		}
	}
}