using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TurtleGraphics.IO {
	public class FileSystemManager {

		public const string EXTENSION = ".tgs";
		public const string CRASH_BCK = ".crash_bck";

		public string SavedDataPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SavedData");
		private LoadSaveDataDialog loadDialog;

		public FileSystemManager() {
			if (!Directory.Exists(SavedDataPath)) {
				Directory.CreateDirectory(SavedDataPath);
			}
		}

		public void Save(string saveFileName, string code) {
			if (saveFileName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1) {
				foreach (char c in Path.GetInvalidFileNameChars()) {
					saveFileName = saveFileName.Replace(c, '_');
				}
			}
			File.WriteAllText(Path.Combine(SavedDataPath, saveFileName + EXTENSION), string.Join(Environment.NewLine, code));
		}

		public void AbortLoad() {
			if (loadDialog != null) {
				loadDialog.CancelCommand.Execute(null);
			}
		}

		public async Task<SavedData> Load() {
			loadDialog = new LoadSaveDataDialog {
				Path = SavedDataPath
			};
			Grid.SetColumn(loadDialog, MainWindow.PAGES_COLUMN_INDEX);
			Panel.SetZIndex(loadDialog, 2);
			MainWindow.Instance.Paths.Children.Add(loadDialog);
			SavedData selected = await loadDialog.Select();
			loadDialog = null;
			return selected;
		}

		public void CreateCodeBackup(string commandsText) {
			File.WriteAllText(Path.Combine(SavedDataPath, CRASH_BCK), commandsText);
		}

		public string RestoreCodeIfExists() {
			string fullPath = Path.Combine(SavedDataPath, CRASH_BCK);
			if (File.Exists(fullPath)) {
				string content = File.ReadAllText(fullPath);
				File.Delete(fullPath);
				return content;
			}
			return "";
		}
	}
}