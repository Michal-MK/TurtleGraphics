﻿using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Flee.PublicTypes;

namespace TurtleGraphics {

	public abstract class ParsedData {

		public ParsedData(params string[] parameters) {
			Parameters = parameters;
		}

		public abstract bool IsBlock { get; }

		public Dictionary<string, object> Variables { get; set; }

		public string[] Parameters { get; set; }

		public string Arg1 => Parameters[0];
		public string Arg2 => Parameters[1];

		public abstract ParsedAction Action { get; }

		public abstract TurtleData Compile(TurtleData previous, CancellationToken token);


		public abstract IList<TurtleData> CompileBlock(TurtleData previous, CancellationToken token);

		protected void UpdateVars(IDynamicExpression exp) {
			foreach (var item in Variables) {
				exp.Context.Variables[item.Key] = item.Value;
			}
		}

		protected void UpdateVars<T>(IGenericExpression<T> exp) {
			foreach (var item in Variables) {
				exp.Context.Variables[item.Key] = item.Value;
			}
		}
	}
}
