using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TurtleGraphics {
	public class ScreenCaptureData : ParsedData {

		public ScreenCaptureData(ParameterValuation[] args, string originalLine, Dictionary<string, object> variables) : base(variables, originalLine, args) {

		}

		public override bool IsBlock => false;

		public override string Line { get; set; }

		public override ParsedAction Action => ParsedAction.ScreenCapture;

		public override TurtleData Compile(CancellationToken token) {
			return new TurtleData() { Action = Action };
		}

		public override IList<TurtleData> CompileBlock(CancellationToken token) {
			throw new NotImplementedException();
		}
	}
}
