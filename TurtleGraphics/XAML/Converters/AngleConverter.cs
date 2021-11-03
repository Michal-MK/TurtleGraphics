using System;
using System.Globalization;
using System.Windows.Data;

namespace TurtleGraphics.XAML.Converters {
	public class AngleConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is double d) {
				return $"\t{Math.Floor(ContextExtensions.AsDeg(d))}°";
			}
			throw new ArgumentException("Can only convert from doubles!");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new InvalidOperationException("This converter does not support conversion backwards");
		}
	}
}