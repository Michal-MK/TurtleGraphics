using Igor.Localization;
using Igor.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Windows.Forms;
using Igor.Configuration;

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
		private ICommand _closeCommand;
		private ICommand _selectScreenshotLocationCommand;

		public ICommand SelectScreenshotLocationCommand { get => _selectScreenshotLocationCommand; set { _selectScreenshotLocationCommand = value; Notify(nameof(SelectScreenshotLocationCommand)); } }
		public ICommand CloseCommand { get => _closeCommand; set { _closeCommand = value; Notify(nameof(CloseCommand)); } }
		public GridLength LanguageColumnWidth { get => _languageColumnWidth; set { _languageColumnWidth = value; Notify(nameof(LanguageColumnWidth)); } }
		public ObservableCollection<LanguageButtonModel> Languages { get => _languages; set { _languages = value; Notify(nameof(Languages)); } }

		#endregion

		#region Language

		public class Lang {
			public string AvailableLanguages => LocaleProvider.Instance.Get(Locale.Settings.AVAILABLE_LANGUAGES);
			public string GenericBack => LocaleProvider.Instance.Get(Locale.Base.GENERIC_BACK);
			public string GenericSelect => LocaleProvider.Instance.Get(Locale.Base.GENERIC_SELECT);
			public string TipsLabel => LocaleProvider.Instance.Get(Locale.Settings.TIPS_LABEL);
			public string ScreenshotNote => LocaleProvider.Instance.Get(Locale.Settings.SCREENSHOT_NOTE);
			public string ScreenshotSaveLocation => LocaleProvider.Instance.Get(Locale.Settings.SCREENSHOT_SAVE_LOCATION);
		};

		public Lang L { get; } = new Lang();

		#endregion

		public Settings() {
			DataContext = this;
			InitializeComponent();

			CloseCommand = new Command(Close);
			SelectScreenshotLocationCommand = new Command(SelectScreenshotLocationAction);

			LocaleProvider.Instance.OnLanguageChanged += (s, e) => { Notify(nameof(L)); };

			double largestWidth = 0;

			foreach (string code in LocaleProvider.Instance.LoadedLanguages) {
				LanguageButtonModel m = new LanguageButtonModel(code);
				_languages.Add(m);
				FormattedText t = new FormattedText(m.LanguageText, CultureInfo.InvariantCulture,
					System.Windows.FlowDirection.LeftToRight,
					new Typeface(FontFamily, FontStyle, FontWeight, FontStretch, FontFamily),
					12, Brushes.Black, new NumberSubstitution(), 1);
				largestWidth = Math.Max(largestWidth, t.Width);
			}
			LanguageColumnWidth = new GridLength(largestWidth + 40);
		}


		public string ScreenshotSaveLocation {
			get => ConfigurationManager<Config>.Instance.CurrentSettings.ScreenshotSaveLocation;
			set {
				ConfigurationManager<Config>.Instance.CurrentSettings.ScreenshotSaveLocation = value;
				Notify(nameof(ScreenshotSaveLocation));
			}
		}


		#region Actions

		private void SelectScreenshotLocationAction() {
			if (CommonFileDialog.IsPlatformSupported) {
				using (CommonOpenFileDialog dialog = new CommonOpenFileDialog {
					IsFolderPicker = true
				}) {
					CommonFileDialogResult result = dialog.ShowDialog();
					Focus();
					if (result == CommonFileDialogResult.Ok) {
						FileAttributes attr = File.GetAttributes(dialog.FileName);
						if (attr.HasFlag(FileAttributes.Directory)) {
							ScreenshotSaveLocation = dialog.FileName;
						}
						else {
							ScreenshotSaveLocation = new FileInfo(dialog.FileName).DirectoryName;
						}
						ConfigurationManager<Config>.Instance.CurrentSettings.ScreenshotSaveLocation = ScreenshotSaveLocation;
						ConfigurationManager<Config>.Instance.Save();
					}
				}
			}
			else {
				using (var dialog = new FolderBrowserDialog()) {
					DialogResult result = dialog.ShowDialog();
					if (result == System.Windows.Forms.DialogResult.OK) {
						ScreenshotSaveLocation = dialog.SelectedPath;
						ConfigurationManager<Config>.Instance.CurrentSettings.ScreenshotSaveLocation = ScreenshotSaveLocation;
						ConfigurationManager<Config>.Instance.Save();
					}
				}
			}
		}

		#endregion
	}
}
