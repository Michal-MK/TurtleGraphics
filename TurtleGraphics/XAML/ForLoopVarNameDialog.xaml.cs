using Igor.Localization;
using Igor.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TurtleGraphics {
	/// <summary>
	/// Interaction logic for ForLoopVarNameDialog.xaml
	/// </summary>
	public partial class ForLoopVarNameDialog : Window, INotifyPropertyChanged {

		#region Notifications

		public event PropertyChangedEventHandler PropertyChanged;

		private void Notify(string prop) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
		}

		#endregion


		private string _varName;
		private ICommand _okCommand;

		public ICommand OkCommand { get => _okCommand; set { _okCommand = value; Notify(nameof(OkCommand)); } }
		public string VarName { get => _varName; set { _varName = value; Notify(nameof(VarName)); } }

		public ForLoopVarNameDialog() {
			InitializeComponent();
			DataContext = this;

			OkCommand = new Command(OkAction);
		}

		public class Lang {
			public string Header => LocaleProvider.Instance.Get(Locale.ForLoopVarName.HEADER);
			public string GenericBack => LocaleProvider.Instance.Get(Locale.Base.GENERIC_BACK);
		}


		public Lang L { get; } = new Lang();

		#region Actions

		private void OkAction() {
			DialogResult = true;
			Close();
		}

		#endregion
	}
}
