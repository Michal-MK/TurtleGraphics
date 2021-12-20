using System;
using System.Collections.Generic;
using System.Threading;
using TurtleGraphics.Language.Logic;
using TurtleGraphics.ParsedData.Base;
using TurtleGraphics.Parsers;

namespace TurtleGraphics.ParsedData {
	public class ScreenCaptureData : BaseParsedData {

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