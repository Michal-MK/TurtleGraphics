using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using Igor.Localization;
using Igor.Models;

namespace TurtleGraphics {
	/// <summary>
	/// Interaction logic for SaveDialog.xaml
	/// </summary>
	public partial class SaveDialog : UserControl, INotifyPropertyChanged {

		#region Notifications

		public event PropertyChangedEventHandler PropertyChanged;

		private void Notify(string prop) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
		}

		#endregion

		public string Save_Name => LocaleProvider.Instance.Get(Locale.Saves.NAME);
		public string GenericSave => LocaleProvider.Instance.Get(Locale.Base.GENERIC_SAVE);
		public string GenericCancel => LocaleProvider.Instance.Get(Locale.Base.GENERIC_CANCEL);

		public SaveDialog() {
			InitializeComponent();
			DataContext = this;
			SaveCommand = new Command(() => {
				if (string.IsNullOrWhiteSpace(SaveFileName)) {
					return;
				}
				MainWindow.Instance.FSManager.Save(SaveFileName, MainWindow.Instance.CommandsText);
				Common();
			});
			CancelCommand = new Command(Common);

			_focus.Loaded += (s,e) => _focus.Focus();
		}

		private void Common() {
			MainWindow.Instance.Paths.Children.Remove(this);
			MainWindow.Instance.SaveDialogActive = false;
		}

		private ICommand _saveCommand;
		private string _saveFileName;
		private ICommand _cancelCommand;

		public ICommand CancelCommand { get => _cancelCommand; set { _cancelCommand = value; Notify(nameof(CancelCommand)); } }
		public string SaveFileName { get => _saveFileName; set { _saveFileName = value; Notify(nameof(SaveFileName)); } }
		public ICommand SaveCommand { get => _saveCommand; set { _saveCommand = value; Notify(nameof(SaveCommand)); } }
	}
}
