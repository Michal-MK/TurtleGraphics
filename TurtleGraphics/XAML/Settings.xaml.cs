using Igor.Localization;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace TurtleGraphics {
	/// <summary>
	/// Interaction logic for Settings.xaml
	/// </summary>
	public partial class Settings : Window, INotifyPropertyChanged {

		#region Notifications

		public event PropertyChangedEventHandler PropertyChanged;

		private void Notify(string prop) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
		}

		#endregion


		#region Binding

		private ObservableCollection<LanguageButtonModel> _languages = new ObservableCollection<LanguageButtonModel>();
		private GridLength _languageColumnWidth;

		public GridLength LanguageColumnWidth { get => _languageColumnWidth; set { _languageColumnWidth = value; Notify(nameof(LanguageColumnWidth)); } }
		public ObservableCollection<LanguageButtonModel> Languages { get => _languages; set { _languages = value; Notify(nameof(Languages)); } }

		#endregion

		#region Language

		public class Lang {
			public string Sett_AvailableLanguages => LocaleProvider.Instance.Get(Locale.SETT__AVAILABLE_LANGUAGES);
		};

		public Lang L { get; } = new Lang();

		#endregion

		public Settings() {
			DataContext = this;
			InitializeComponent();
			LocaleProvider.Instance.OnLanguageChanged += (s, e) => { Notify(nameof(L)); };

			double largestWidth = 0;

			foreach (string code in LocaleProvider.Instance.LoadedLanguages) {
				LanguageButtonModel m = new LanguageButtonModel(code);
				_languages.Add(m);
				FormattedText t = new FormattedText(m.LanguageText, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface(FontFamily, FontStyle, FontWeight, FontStretch, FontFamily), 12, Brushes.Black, new NumberSubstitution(), 1);
				largestWidth = Math.Max(largestWidth, t.Width);
			}
			LanguageColumnWidth = new GridLength(largestWidth + 40);
		}
	}
}
