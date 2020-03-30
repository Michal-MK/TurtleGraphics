using Igor.Configuration;
using Igor.Localization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;

namespace TurtleGraphics {
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application {

		public bool LaunchFullScreen { get; private set; } = false;

		public bool LaunchHiddenControlPanel { get; private set; } = false;

		public TurtleGraphicsCodeData Deserialized { get; private set; }

		public static App Instance { get; private set; }

		public ConfigurationManager<Config> Cfg => ConfigurationManager<Config>.Instance;

		public App() {
			ConfigurationManager<Config>.Initialize("settings.cfg", true);

			LocaleProvider.Initialize("locale").SetActiveLanguage(Cfg.CurrentSettings.ActiveLanguage);
			Instance = this;
		}

		protected override void OnStartup(StartupEventArgs e) {
			foreach (string arg in e.Args) {
				if (arg == "-f") {
					LaunchFullScreen = true;
				}
				if (arg == "-thc") {
					LaunchHiddenControlPanel = true;
				}

				if (File.Exists(arg)) {
					BinaryFormatter bf = new BinaryFormatter();
					using (FileStream fs = File.OpenRead(arg)) {
						Deserialized = (TurtleGraphicsCodeData)bf.Deserialize(fs);
					}
				}
			}
			base.OnStartup(e);
		}
	}
}
