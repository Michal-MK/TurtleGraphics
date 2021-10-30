using System.ComponentModel;

namespace TurtleGraphics.Models {
	public class BaseViewModel : INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;

		public void Notify(string name) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
