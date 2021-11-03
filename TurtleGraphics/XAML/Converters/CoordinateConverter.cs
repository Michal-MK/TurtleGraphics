using System;
using System.Globalization;
using System.Windows.Data;

namespace TurtleGraphics.XAML.Converters {
	public class CoordinateConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is double d) {
				return $"\t{Math.Round(d, 2)}";
			}
			throw new ArgumentException("Can only convert from doubles!");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new InvalidOperationException("This converter does not support conversion backwards");
		}
	}
}