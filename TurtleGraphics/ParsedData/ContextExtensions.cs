using System;

namespace TurtleGraphics.ParsedData {
	public static class ContextExtensions {

		private static readonly Random RANDOM = new Random();

		public static double AsRad(double degrees) {
			return Math.PI * degrees / 180.0;
		}

		public static double AsDeg(double rad) {
			return rad * (180.0 / Math.PI);
		}

		public static double RandX() {
			return RANDOM.NextDouble() * MainWindow.Instance.DrawWidth;
		}

		public static double RandY() {
			return RANDOM.NextDouble() * MainWindow.Instance.DrawHeight;
		}

		public static double Random(double from, double to) => Rand(from, to);
		public static double Random(int from, int to) => Rand(from, to);

		public static double Rand(double from, double to) {
			return RANDOM.Next((int)from, (int)to - 1) + RANDOM.NextDouble();
		}

		public static double Rand(int from, int to) {
			return RANDOM.Next(from, to - 1) + RANDOM.NextDouble();
		}

		public static int Map(double value, double min, double max, double newMin, double newMax) {
			double slope = (newMax - newMin) / (max - min);
			return (int)(newMin + slope * (value - min));
		}
	}
}