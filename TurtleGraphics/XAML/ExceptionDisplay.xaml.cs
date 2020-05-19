using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using Igor.Localization;
using Igor.Models;

namespace TurtleGraphics {
	/// <summary>
	/// Interaction logic for ExceptionDisplay.xaml
	/// </summary>
	public partial class ExceptionDisplay : UserControl, INotifyPropertyChanged {

		#region Notifications

		public event PropertyChangedEventHandler PropertyChanged;

		private void Notify(string prop) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
		}

		#endregion

		public ExceptionDisplay() {
			InitializeComponent();
			DataContext = this;
			Grid.SetColumn(this, 1);

			_dismissCommand = new Command(() => {
				MainWindow.Instance.Paths.Children.Remove(this);
				MainWindow.Instance.ShowTurtleCheckBox = _turtleVisibilityBck;
				MainWindow.Instance.ExceptionDialogActive = false;
			});
			Loaded += ExceptionDisplay_Loaded;
		}

		private void ExceptionDisplay_Loaded(object sender, System.Windows.RoutedEventArgs e) {
			MainWindow.Instance.ExceptionDialogActive = true;
			_turtleVisibilityBck = MainWindow.Instance.ShowTurtleCheckBox;
			MainWindow.Instance.ShowTurtleCheckBox = false;
			LocaleProvider.Instance.OnLanguageChanged += (s, ee) => { Notify(nameof(L)); };
			FocusMe.Focus();
		}

		private bool _turtleVisibilityBck;
		private ParsingException _exception;

		private string _exceptionMessage;
		private ICommand _dismissCommand;
		private string _stackTrace;

		public string StackTrace { get => _stackTrace; set { _stackTrace = value; Notify(nameof(StackTrace)); } }
		public ICommand DismissCommand { get => _dismissCommand; set { _dismissCommand = value; Notify(nameof(DismissCommand)); } }
		public string ExceptionMessage {
			get => _exceptionMessage;
			set {
				bool success = CommandParser.LineIndexes.TryGetValue(Exception.LineText, out int val);
				_exceptionMessage = value + $"{Environment.NewLine}  at line ({(success ? val.ToString() : "unable to determine")}): {Exception.LineText}";
				Notify(nameof(ExceptionMessage));
			}
		}

		public class Lang {
			public string Except_Header => LocaleProvider.Instance.Get(Locale.ExceptionDisplay.HEADER);
			public string Except_MessageLabel => LocaleProvider.Instance.Get(Locale.ExceptionDisplay.MESSAGE_LABEL);
			public string Except_StackTraceLabel => LocaleProvider.Instance.Get(Locale.ExceptionDisplay.STACK_TRACE_LABEL);
			public string Except_ButtonLabel => LocaleProvider.Instance.Get(Locale.ExceptionDisplay.BUTTON_LABEL);
		}

		public Lang L { get; } = new Lang();

		public ParsingException Exception {
			get => _exception;
			set {
				_exception = value;
				StackTrace = value.StackTrace;
			}
		}

		public void Show() {
			Grid.SetColumn(this, 2);
			MainWindow.Instance.Paths.Children.Add(this);
		}
	}
}
