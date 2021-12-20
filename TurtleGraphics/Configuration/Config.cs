using System;
using Igor.Configuration;

namespace TurtleGraphics.Configuration {
	public class Config : IConfiguration {
		public string ConfigurationHeader() => "Turtle Graphics configuration file. Do not edit directly!";

		[Comment("Selected UI language")]
		public string ActiveLanguage { get; set; } = "en-us";

		[Comment("Location of the produced screenshots")]
		public string ScreenshotSaveLocation { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
	}
}