using System.Collections.Generic;
using System.Threading;
using Flee.PublicTypes;
using TurtleGraphics.Language.Logic;
using TurtleGraphics.Parsers;

namespace TurtleGraphics.ParsedData.Base {

	public abstract class BaseParsedData {

		public BaseParsedData(Dictionary<string, object> variables, string originalLine, params ParameterValuation[] parameters) {
			Parameters = parameters;
			Variables = variables;
			Line = originalLine;
		}

		public abstract bool IsBlock { get; }

		public abstract string Line { get; set; }

		public Dictionary<string, object> Variables { get; }

		public ParameterValuation[] Parameters { get; set; }

		public string Arg1 => Parameters.Length > 0 ? Parameters[0].Value.ToString() : null;
		public string Arg2 => Parameters.Length > 1 ? Parameters[1].Value.ToString() : null;
		public string Arg3 => Parameters.Length > 2 ? Parameters[2].Value.ToString() : null;
		public string Arg4 => Parameters.Length > 3 ? Parameters[3].Value.ToString() : null;

		public abstract ParsedAction Action { get; }

		public abstract TurtleData Compile(CancellationToken token);


		public abstract IList<TurtleData> CompileBlock(CancellationToken token);

		internal void UpdateVars(IDynamicExpression exp) {
			foreach (var item in Variables) {
				exp.Context.Variables[item.Key] = item.Value;
			}
		}

		internal void UpdateVars<T>(IGenericExpression<T> exp) {
			foreach (var item in Variables) {
				exp.Context.Variables[item.Key] = item.Value;
			}
		}
	}
}