using Igor.Configuration;

namespace TurtleGraphics {
	public class Config : IConfiguration {
		public string ConfigurationHeader() => "Turtle Graphics configuration file. Do not edit directly!";

		[Comment("Selected UI language")]
		public string ActiveLanguage { get; set; } = "en-us";
	}
}
