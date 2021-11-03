﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using Flee.PublicTypes;

namespace TurtleGraphics {
	public class MoveData : ParsedData {

		private readonly IGenericExpression<double> x;
		private readonly IGenericExpression<double> y;

		public MoveData(ParameterValuation[] args, Dictionary<string, object> variables, string line) : base(variables, line, args) {
			ExpressionContext expression = FleeHelper.GetExpression(variables);
			string exceptionMessage = "";
			if(args.Length > 2) {
				throw new ParsingException("Extra arguments supplied, maximum of 2 allowed for this function.", line);
			}
			try {
				exceptionMessage = "Invalid expression for X coordinate!";
				x = expression.CompileGeneric<double>(args[0].Value.ToString());
				exceptionMessage = "Invalid expression for Y coordinate!";
				y = expression.CompileGeneric<double>(args[1].Value.ToString());
			}
			catch (Exception e) {
				throw new ParsingException(exceptionMessage, line, e);
			}
		}

		public override bool IsBlock => false;

		public override ParsedAction Action => ParsedAction.MoveTo;

		public override string Line { get; set; }

		public override TurtleData Compile(CancellationToken token) {
			token.ThrowIfCancellationRequested();
			UpdateVars(x);
			UpdateVars(y);

			return new TurtleData {
				MoveTo = new Point(x.Evaluate(), y.Evaluate()),
				Action = Action,
			};
		}

		public override IList<TurtleData> CompileBlock(CancellationToken token) {
			throw new NotImplementedException();
		}
	}
}