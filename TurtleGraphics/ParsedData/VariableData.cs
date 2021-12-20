﻿using System.Collections.Generic;
using System.Threading;
using Flee.PublicTypes;
using TurtleGraphics.ParsedData.Base;
using TurtleGraphics.Parsers;

namespace TurtleGraphics.ParsedData {
	public class VariableData : BaseParsedData {
		public override bool IsBlock => false;

		public override string Line { get; set; }

		public override ParsedAction Action => ParsedAction.VariableChange;

		public string VariableName { get; }
		public IDynamicExpression Value { get; }

		public VariableData(string varName, IDynamicExpression value, Dictionary<string, object> variables, string line) : base(variables, line) {
			VariableName = varName;
			Value = value;
		}

		public override TurtleData Compile(CancellationToken token) {
			return TurtleData.NoAction;
		}

		public override IList<TurtleData> CompileBlock(CancellationToken token) {
			throw new System.NotImplementedException();
		}
	}
}