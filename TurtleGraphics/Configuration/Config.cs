using Igor.Configuration;
using System;

namespace TurtleGraphics {
	public class Config : IConfiguration {
		public string ConfigurationHeader() => "Turtle Graphics configuration file. Do not edit directly!";

		[Comment("Selected UI language")]
		public string ActiveLanguage { get; set; } = "en-us";

		public string ScreenshotSaveLocation { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
	}
}
