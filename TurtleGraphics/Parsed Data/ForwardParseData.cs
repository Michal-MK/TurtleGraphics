﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using Flee.PublicTypes;

namespace TurtleGraphics {
	public class ForwardParseData : ParsedData {

		private readonly IGenericExpression<double> _expression;

		public ForwardParseData(IGenericExpression<double> expression, string[] args, Dictionary<string, object> variables, string line) : base(variables, line, args) {
			_expression = expression;
		}

		public double Distance { get; set; }

		public override bool IsBlock => false;

		public override ParsedAction Action => ParsedAction.Forward;

		public override string Line { get; set; }

		public override TurtleData Compile(CancellationToken token) {
			token.ThrowIfCancellationRequested();
			UpdateVars(_expression);
			Distance = _expression.Evaluate();
			return new TurtleData {
				Distance = Distance,
				Action = Action,
			};
		}

		public override IList<TurtleData> CompileBlock(CancellationToken token, Dictionary<int, LineCacheData> cache) {
			throw new NotImplementedException();
		}
	}
}
