using System.Windows.Input;
using Igor.Configuration;
using Igor.Localization;
using Igor.Models;
using TurtleGraphics.Configuration;

namespace TurtleGraphics.Models {
	public class LanguageButtonModel : BaseViewModel {

		private readonly string _languageCode;

		public LanguageButtonModel(string language) {
			_languageCode = language;
			LanguageText = LocaleProvider.Instance.GetFullName(language) + " - " + language;
			ChangeLanguage = new Command(() => {
				LocaleProvider.Instance.SetActiveLanguage(_languageCode);
				ConfigurationManager<Config>.Instance.CurrentSettings.ActiveLanguage = _languageCode;
				ConfigurationManager<Config>.Instance.Save();
			});
		}

		public string LanguageText { get; set; }
		public ICommand ChangeLanguage { get; set; }
	}
}