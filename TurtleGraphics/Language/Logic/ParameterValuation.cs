using System;
using TurtleGraphics.Language.Definition;

namespace TurtleGraphics {
	public abstract class ParameterValuation {
		public ParameterValuation(Parameter param, object value) {
			Param = param;
			Value = value;
		}

		public Parameter Param { get; }
		public object Value { get; }
	}

	public class ParameterValuation<T> : ParameterValuation {
		public ParameterValuation(Parameter param, T value) : base(param, value) {
			if (param.Type != null && typeof(T) != param.Type) throw new InvalidOperationException($"Parameter does not support values of type {typeof(T)}");
			Value = value;
		}

		public new T Value { get; }
	}
}