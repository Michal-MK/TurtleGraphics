using System;
using System.Collections.Generic;
using System.Drawing;

namespace TurtleGraphics.Parsers {
	public static class SupportedTypes {
		public static readonly Dictionary<string, Type> TYPE_DICT = new Dictionary<string, Type> {
			{ "int", typeof(int) },
			{ "long", typeof(long) },
			{ "Point", typeof(Point) },
		};
	}
}