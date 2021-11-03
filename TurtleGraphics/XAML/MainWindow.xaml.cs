using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Igor.Models;
using System.IO;
using System.Windows.Media.Imaging;
using static TurtleGraphics.Helpers;
using Path = System.Windows.Shapes.Path;
using Igor.Localization;
using TurtleGraphics.Models;

namespace TurtleGraphics {
	public partial class MainWindow : Window, INotifyPropertyChanged {
		#region Notifications

		public event PropertyChangedEventHandler PropertyChanged;

		private void Notify(string prop) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
		}

		#endregion

		public const int PAGES_COLUMN_INDEX = 2;

		#region Bindings

		[Notify]
		public InteliCommandDialogViewModel InteliCommandModel { get; } = new InteliCommandDialogViewModel();

		[Notify]
		public ImageSource ImgSource { get; private set; }

		[Notify]
		public bool ControlsVisible { get; set; } = true;

		[Notify]
		public double Y { get; set; }

		[Notify]
		public double X { get; set; }

		[Notify]
		public double Angle { get; set; }

		[Notify]
		public PenLineCap LineCapping { get; set; }

		[Notify]
		public int CalculationFramesPreUIUpdate { get; set; } = 1;

		[Notify]
		public bool ShowTurtleCheckBox { get; set; } = true;

		[Notify]
		public bool ToggleFullscreenEnabled { get; set; }

		[Notify]
		public string ButtonText { get; set; }

		[Notify]
		public ICommand SettingsCommand { get; }

		[Notify]
		public ICommand ControlsVisibleCommand { get; }

		[Notify]
		public ICommand LoadCommand { get; }

		[Notify]
		public ICommand SaveCommand { get; }

		[Notify]
		public ICommand RunFullscreenCommand { get; }

		[Notify]
		public ICommand StopCommand { get; }

		[Notify]
		public ICommand RunCommand { get; }

		[Notify]
		public ICommand ButtonCommand { get; set; }

		[Notify]
		public int IterationCount { get; set; } = 1;

		private string _commandsText = "";

		public string CommandsText {
			get => _commandsText;
			set {
				_commandsText = value;
				Notify(nameof(CommandsText));
				InteliCommandModel.Handle(value, CommandsTextInput, CommandsView, ControlArea.ActualWidth);
			}
		}

		[NotifyWithCallback(nameof(ResetPathAnimationSlider))]
		public bool AnimatePath { get; set; }

		[NotifyWithCallback(nameof(NewPath))]
		public bool PenDown { get; set; }

		[NotifyWithCallback(nameof(NewPath))]
		public double BrushSize { get; set; }

		[NotifyWithCallback(nameof(NewPath))]
		public string Color { get; set; }

		#endregion

		#region Language

		public class Lang {
			public string WindowName => LocaleProvider.Instance.Get(Locale.Main.WINDOW_NAME);
			public string Angle => LocaleProvider.Instance.Get(Locale.Main.ANGLE);
			public string AnimatePath => LocaleProvider.Instance.Get(Locale.Main.ANIMATE_PATH);
			public string BackgroundCol => LocaleProvider.Instance.Get(Locale.Main.BACKGROUND_COL);
			public string PathAnimSpeed => LocaleProvider.Instance.Get(Locale.Main.PATH_ANIM_SPEED);
			public string ToggleControlPanel => LocaleProvider.Instance.Get(Locale.Main.TOGGLE_CONTROL_PANEL);
			public string TurtleSpeed => LocaleProvider.Instance.Get(Locale.Main.TURTLE_SPEED);
			public string ShowTurtle => LocaleProvider.Instance.Get(Locale.Main.SHOW_TURTLE);
			public string GenericLoad => LocaleProvider.Instance.Get(Locale.Base.GENERIC_LOAD);
			public string GenericSave => LocaleProvider.Instance.Get(Locale.Base.GENERIC_SAVE);
			public string RunFullscreen => LocaleProvider.Instance.Get(Locale.Main.RUN_FULLSCREEN) + " (Ctrl + F5)";
			public string Settings => LocaleProvider.Instance.Get(Locale.Main.SETTINGS);
			public string GenericRun => LocaleProvider.Instance.Get(Locale.Base.GENERIC_RUN) + " (F5)";
			public string GenericStop => LocaleProvider.Instance.Get(Locale.Base.GENERIC_STOP) + " (F5)";
		}

		public Lang L { get; } = new Lang();

		#endregion

		#region Standard Properties

		private Path _currentPath;
		private PathFigure _currentFigure;
		private PolyLineSegment _currentSegment;
		private CancellationTokenSource cancellationTokenSource;
		private readonly CompilationStatus _compilationStatus = new CompilationStatus();
		private readonly ExceptionDisplay _exceptionDisplay = new ExceptionDisplay();
		private readonly BrushConverter _brushConverter = new BrushConverter();

		public double DrawWidth { get; set; }
		public double DrawHeight { get; set; }
		public static MainWindow Instance { get; set; }
		public FileSystemManager FSManager { get; set; }
		public bool SaveDialogActive { get; set; }
		public bool LoadDialogActive { get; set; }
		public bool ExceptionDialogActive { get; set; }
		public bool IsFullscreen { get; set; }
		public bool ControlPanelHolderVisible { get; set; } = true;

		#endregion

		public MainWindow() {
			InitializeComponent();

			RunCommand = new AsyncCommand(RunCommandAction);
			StopCommand = new Command(StopCommandAction);
			SaveCommand = new Command(SaveCommandAction);
			LoadCommand = new AsyncCommand(LoadCommandAction);
			ControlsVisibleCommand = new Command(() => ControlsVisible ^= true);
			RunFullscreenCommand = new AsyncCommand(RunFullscreenCommandAction);
			SettingsCommand = new Command(OpenSettingsAction);
			LocaleProvider.Instance.OnLanguageChanged += (s, e) => { Notify(nameof(L)); };

			ButtonCommand = RunCommand;
			ButtonText = L.GenericRun;

			FSManager = new FileSystemManager();

			Loaded += MainWindow_Loaded;

			SizeChanged += MainWindow_SizeChanged;
			CommandsTextInput.PreviewKeyDown += InteliCommandModel.TextEvents;
			CommandsTextInput.TextChanged += CommandsTextInput_TextChanged;

			SetWindowState(App.Instance.LaunchFullScreen);
			if (App.Instance.LaunchHiddenControlPanel) {
				ToggleControlPanel();
			}

			DataContext = this;
			Instance = this;
		}

		private void RemoveAllPaths() {
			foreach (Path item in Paths.Children.OfType<Path>().ToArray()) {
				Paths.Children.Remove(item);
			}
		}

		public void Init() {
			RemoveAllPaths();

			_currentFigure = null;
			_currentPath = null;

			Color = "Blue";
			PenDown = true;
			X = DrawWidth / 2;
			Y = DrawHeight / 2;
			TurtleTranslation.X = X;
			TurtleTranslation.Y = Y;
			TurtleRotation.Angle = 90;
			TurtleScale.ScaleX = 1;
			TurtleScale.ScaleY = 1;
			Angle = 0;
			BrushSize = 4;
			LineCapping = PenLineCap.Round;

			NewPath();
		}

		#region Events

		private void MainWindow_Loaded(object sender, RoutedEventArgs e) {
			DrawWidth = DrawAreaX.ActualWidth;
			DrawHeight = DrawAreaY.ActualHeight;
			Init();
			CommandsText = FSManager.RestoreCodeIfExists();
			if (App.Instance.Deserialized != null) {
				cancellationTokenSource = new CancellationTokenSource();
				_ = ExecuteCode(App.Instance.Deserialized);
			}
			Loaded -= MainWindow_Loaded;
		}

		private async Task ExecuteCode(TurtleGraphicsCodeData deserialized) {
			ToggleControlPanel();
			Init();
			ShowTurtleCheckBox = deserialized.ShowTurtle;
			AnimatePath = deserialized.AnimatePath;
			CalculationFramesPreUIUpdate = deserialized.TurtleSpeed;
			BackgroundColor.Text = deserialized.BackgroundColor;
			ButtonCommand = StopCommand;
			await DrawData(deserialized.Data);
		}

		private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e) {
			DrawWidth = DrawAreaX.ActualWidth;
			DrawHeight = DrawAreaY.ActualHeight;
			TurtleTranslation.X = DrawWidth / 2;
			TurtleTranslation.Y = DrawHeight / 2;
		}

		private void Control_SizeChanged(object sender, SizeChangedEventArgs e) {
			if (InteliCommandModel.Visible == Visibility.Visible) {
				CommandsView.Width = ControlArea.ActualWidth;
			}
		}

		private void MainWindow_KeyDown(object sender, KeyEventArgs e) {
			if (e.Key == Key.Escape) {
				if (!ControlPanelHolderVisible) {
					ToggleControlPanel();
				}
				ImgSource = null;
				StopCommand.Execute(null);
				Init();
				UpdateLayout();
				ToggleFullScreenAction();
			}

			if (e.KeyboardDevice.Modifiers == (ModifierKeys.Control | ModifierKeys.Alt) && e.Key == Key.C) {
				if (IsFullscreen && !ControlPanelHolderVisible) {
					(int actualWidth, int actualHeight) = GetActualScreenSize();
					System.Drawing.Bitmap bitmap;
					using (MemoryStream ms = CaptureScreenshot(actualWidth, actualHeight)) {
						bitmap = (System.Drawing.Bitmap)System.Drawing.Image.FromStream(ms);
					}
					string fileName = $"ScreenCapture {DateTime.Now:dd-MM-yyyy hh_mm_ss}.png";
					string path = System.IO.Path.Combine(App.Instance.Cfg.CurrentSettings.ScreenshotSaveLocation, fileName);
					bitmap.Save(path, System.Drawing.Imaging.ImageFormat.Png);
				}
			}
		}

		private void CommandsTextInput_TextChanged(object sender, TextChangedEventArgs e) {
			foreach (TextChange change in e.Changes) {
				if (change.AddedLength == Environment.NewLine.Length) {
					string changedText = _commandsText.Substring(change.Offset, change.AddedLength);
					if (changedText == Environment.NewLine) {
						HandleNewLineIndent(change);
					}
				}
			}
		}

		private void HandleNewLineIndent(TextChange change) {
			string region = _commandsText.Substring(0, CommandsTextInput.CaretIndex);
			int indentLevel = (region.Count(s => s == '{') - region.Count(s => s == '}')) * 3;
			int carret = CommandsTextInput.CaretIndex;
			if (!(carret < CommandsTextInput.Text.Length && CommandsTextInput.Text[carret] == '}')) {
				CommandsTextInput.Text = CommandsTextInput.Text
														  .Insert(change.Offset + change.AddedLength, new string(' ', indentLevel <= 0 ? 0 : indentLevel));
				CommandsTextInput.CaretIndex = carret + indentLevel;
			}
		}

		protected override void OnKeyDown(KeyEventArgs e) {
			if (e.Key == Key.F11 && ControlPanelHolderVisible) {
				SetWindowState(!IsFullscreen);
			}
			if (e.Key == Key.F5) {
				if (!ExceptionDialogActive) {
					if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl)) {
						RunFullscreenCommand.Execute(null);
					}
					else {
						ButtonCommand.Execute(null);
					}
					e.Handled = true;
				}
			}
		}

		private void Grid_SizeChanged(object sender, SizeChangedEventArgs e) {
			MainWindow_SizeChanged(sender, e);
		}

		#endregion

		public void NewPath() {
			if (_currentPath != null) {
				_currentPath.Data.Freeze();
			}

			_currentPath = new Path();
			Grid.SetColumn(_currentPath, PAGES_COLUMN_INDEX);

			if (!PenDown) {
				_currentPath.Stroke = Brushes.Transparent;
			}
			else {
				_currentPath.Stroke = (Brush)_brushConverter.ConvertFromString(Color);
			}
			_currentPath.StrokeThickness = BrushSize;
			_currentPath.StrokeEndLineCap = LineCapping;
			_currentPath.StrokeStartLineCap = LineCapping;
			PathGeometry pGeometry = new PathGeometry {
				Figures = new PathFigureCollection()
			};
			_currentSegment = new PolyLineSegment();
			_currentFigure = new PathFigure {
				StartPoint = new Point(X, Y),
				Segments = new PathSegmentCollection { _currentSegment }
			};
			pGeometry.Figures.Add(_currentFigure);
			_currentPath.Data = pGeometry;
			Paths.Children.Add(_currentPath);
		}

		private void Capture() {
			(int actualWidth, int actualHeight) = GetActualScreenSize();
			using (MemoryStream ms = CaptureScreenshot(actualWidth, actualHeight)) {
				BitmapImage i = new BitmapImage();
				i.BeginInit();
				i.CacheOption = BitmapCacheOption.OnLoad;
				i.StreamSource = ms;
				i.EndInit();
				i.Freeze();
				ImgSource = i;
				Paths.Children.RemoveRange(4, Paths.Children.Count - 5);
			}
		}

		private MemoryStream CaptureScreenshot(int width, int height) {
			MemoryStream ms = new MemoryStream();
			int ix = 0;
			int iy = 0;
			System.Drawing.Bitmap image = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(image);
			g.CopyFromScreen(ix, iy, ix, iy, new System.Drawing.Size(width, height), System.Drawing.CopyPixelOperation.SourceCopy);
			image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
			return ms;
		}

		private (int, int) GetActualScreenSize() {
			PresentationSource presentationSource = PresentationSource.FromVisual(this);
			Matrix m = presentationSource.CompositionTarget.TransformToDevice;
			double dipWidth = m.M11;
			double dipHeight = m.M22;
			double actualHeight = SystemParameters.PrimaryScreenHeight * dipHeight;
			double actualWidth = SystemParameters.PrimaryScreenWidth * dipWidth;
			return ((int)actualWidth, (int)actualHeight);
		}

		public void Rotate(double angle, bool setRotation) {
			if (double.IsNaN(angle)) {
				Angle = 0;
				TurtleRotation.Angle = 90;
				return;
			}
			if (setRotation) {
				Angle = ContextExtensions.AsRad(angle);
				TurtleRotation.Angle = 90 + angle;
			}
			else {
				Angle += ContextExtensions.AsRad(angle);
				TurtleRotation.Angle += angle;
			}

			if (Angle > 2 * Math.PI) {
				Angle %= 2 * Math.PI;
			}
		}

		public async Task Forward(double length) {
			double targetX = X + Math.Cos(Angle) * length;
			double targetY = Y + Math.Sin(Angle) * length;

			await Draw(targetX, targetY);
		}

		#region Drawing lines

		private async Task DrawData(IList<TurtleData> compiledTasks) {
			Stack<(Point, double)> storedPositions = new Stack<(Point, double)>();

			for (int i = 0; i < compiledTasks.Count; i++) {
				if (cancellationTokenSource.Token.IsCancellationRequested) {
					return;
				}

				TurtleData data = compiledTasks[i];
				if (i % CalculationFramesPreUIUpdate == 0) {
					await Task.Delay(1);
				}
				switch (data.Action) {
					case ParsedAction.NONE: {
						break;
					}
					case ParsedAction.Forward: {
						await Forward(data.Distance);
						break;
					}
					case ParsedAction.Rotate: {
						Rotate(data.Angle, data.SetAngle);
						break;
					}
					case ParsedAction.MoveTo: {
						X = data.MoveTo.X;
						Y = data.MoveTo.Y;
						NewPath();
						break;
					}
					case ParsedAction.Color: {
						if (data.Brush == null) {
							Color = data.SerializedBrush;
							NewPath();
						}
						else {
							string newColor = ((SolidColorBrush)data.Brush).Color.ToString();
							if (newColor != Color) {
								Color = newColor;
								NewPath();
							}
						}
						break;
					}
					case ParsedAction.Thickness: {
						BrushSize = data.BrushThickness;
						double scale = BrushSize / 4;
						TurtleScale.ScaleX = TurtleScale.ScaleY = scale;
						NewPath();
						break;
					}
					case ParsedAction.PenState: {
						PenDown = data.PenDown;
						NewPath();
						break;
					}
					case ParsedAction.Capping: {
						LineCapping = data.LineCap;
						NewPath();
						break;
					}
					case ParsedAction.StorePos: {
						storedPositions.Push((new Point(X, Y), ContextExtensions.AsDeg(Angle)));
						break;
					}
					case ParsedAction.RestorePos: {
						(Point point, double angle) = storedPositions.Peek();
						if (data.PopPosition) {
							storedPositions.Pop();
						}
						X = point.X;
						Y = point.Y;
						Rotate(angle, true);
						NewPath();
						break;
					}
					case ParsedAction.ScreenCapture: {
						if (IsFullscreen && !ControlPanelHolderVisible) {
							await Task.Delay(10); // TODO Hardcoded delay
							Capture();
							await Task.Delay(10); // TODO Hardcoded delay
						}
						break;
					}
				}
			}
		}

		public async Task Draw(double x, double y) {
			_currentSegment.Points.Add(new Point(X, Y));
			X = x;
			Y = y;
			await Displace(x, y);
		}

		public async Task Displace(double x, double y) {
			int last = _currentSegment.Points.Count - 1;
			Point origin = _currentSegment.Points[last];
			double increment = 1d / IterationCount;
			double currentInterpolation = 0;

			for (int i = 0; i <= IterationCount; i++) {
				if (cancellationTokenSource.Token.IsCancellationRequested) {
					break;
				}
				_currentSegment.Points[last] = new Point(Lerp(origin.X, x, currentInterpolation), Lerp(origin.Y, y, currentInterpolation));
				TurtleTranslation.X = _currentSegment.Points[last].X + SplitterCol.ActualWidth;
				TurtleTranslation.Y = _currentSegment.Points[last].Y;
				currentInterpolation += increment;
				if (AnimatePath) {
					await Task.Delay(1);
				}
			}
		}

		public void ResetPathAnimationSlider() {
			CalculationFramesPreUIUpdate = 1;
		}

		#endregion

		#region Actions

		private async Task RunCommandAction() {
			if (!NoDialogsActive) return;
			_compilationStatus.Start();
			ToggleFullscreenEnabled = false;
			Init();
			cancellationTokenSource = new CancellationTokenSource();
			ButtonCommand = StopCommand;
			ButtonText = L.GenericStop;
			try {
				FSManager.CreateCodeBackup(CommandsText);
				Queue<ParsedData> tasks = CommandParser.ParseCommands(CommandsText, this);
				List<TurtleData> compiledTasks = await CompileTasks(tasks, cancellationTokenSource.Token);
				if (ShowTurtleCheckBox) {
					ShowTurtleCheckBox = compiledTasks.All(w => w.Action != ParsedAction.ScreenCapture);
				}
				_compilationStatus.Stop();
				Stopwatch s = new Stopwatch();
				s.Start();
				await DrawData(compiledTasks);
				s.Stop();
			}
			catch (OperationCanceledException) {
				_compilationStatus.Stop();
			}
			catch (ParsingException e) {
				_compilationStatus.Stop();
				_exceptionDisplay.Exception = e;
				_exceptionDisplay.ExceptionMessage = e.Message;
				_exceptionDisplay.Show();
			}
			finally {
				ButtonCommand = RunCommand;
				ButtonText = L.GenericRun;
				ToggleFullscreenEnabled = true;
			}
		}

		private Task<List<TurtleData>> CompileTasks(Queue<ParsedData> tasks, CancellationToken token) {
			return Task.Run(() => {
				List<TurtleData> ret = new List<TurtleData>(128) {
					new TurtleData { Angle = Angle, Brush = Brushes.Blue, BrushThickness = BrushSize, MoveTo = new Point(X, Y), PenDown = true }
				};

				while (tasks.Count > 0) {
					ParsedData current = tasks.Dequeue();
					if (current.IsBlock) {
						ret.AddRange(current.CompileBlock(token));
					}
					else {
						ret.Add(current.Compile(token));
					}
				}
				return ret;
			}, token);
		}

		public void ToggleFullScreenAction() {
			SetWindowState(!IsFullscreen);
			ImgSource = null;
		}

		private void ToggleControlPanel() {
			ControlPanelHolderVisible = !ControlPanelHolderVisible;
			if (ControlPanelHolderVisible) {
				SplitterCol.Width = new GridLength(5, GridUnitType.Pixel);
				ControlArea.Width = new GridLength(1, GridUnitType.Star);
				DrawAreaX.Width = new GridLength(3, GridUnitType.Star);
			}
			else {
				SplitterCol.Width = new GridLength(0, GridUnitType.Pixel);
				ControlArea.Width = new GridLength(0, GridUnitType.Star);
			}
			UpdateLayout();
			DrawWidth = DrawAreaX.ActualWidth;
			DrawHeight = DrawAreaY.ActualHeight;
			UpdateLayout();
		}

		private void SetWindowState(bool isFullscreen) {
			if (isFullscreen) {
				WindowStyle = WindowStyle.None;

				// When the window is maximized, fullscreen toggle fails to cover the taskbar, this hack fixes it
				if (WindowState == WindowState.Maximized) {
					WindowState = WindowState.Normal;
				}
				WindowState = WindowState.Maximized;
				PreviewKeyDown += MainWindow_KeyDown;
			}
			else {
				WindowStyle = WindowStyle.SingleBorderWindow;
				WindowState = WindowState.Normal;
				PreviewKeyDown -= MainWindow_KeyDown;
			}
			IsFullscreen = isFullscreen;
		}

		public async Task LoadCommandAction() {
			if (LoadDialogActive) {
				FSManager.AbortLoad();
				LoadDialogActive = false;
				return;
			}
			if (!NoDialogsActive)
				return;
			LoadDialogActive = true;
			SavedData data = await FSManager.Load();
			LoadDialogActive = false;
			if (data.Name != null) {
				CommandsText = data.Code;
			}
		}

		private SaveDialog saveDialog;

		public void SaveCommandAction() {
			if (SaveDialogActive) {
				saveDialog.CancelCommand.Execute(null);
				saveDialog = null;
				return;
			}
			if (!NoDialogsActive)
				return;
			SaveDialogActive = true;
			saveDialog = new SaveDialog();
			Grid.SetColumn(saveDialog, PAGES_COLUMN_INDEX);
			Panel.SetZIndex(saveDialog, 2);
			Paths.Children.Add(saveDialog);
		}

		public void StopCommandAction() {
			cancellationTokenSource.Cancel();
			ButtonCommand = RunCommand;
			ButtonText = L.GenericRun;
		}

		public async Task RunFullscreenCommandAction() {
			if (!IsFullscreen) {
				ToggleFullScreenAction();
			}
			if (ControlPanelHolderVisible) {
				ToggleControlPanel();
			}
			await RunCommandAction();
		}

		private void OpenSettingsAction() {
			Settings s = new Settings();
			IsEnabled = false;
			s.Closed += SettingsClosed;
			s.Show();

			void SettingsClosed(object sender, EventArgs e) {
				s.Closed -= SettingsClosed;
				IsEnabled = true;
				Focus();
				App.Instance.Cfg.Save();
			}
		}

		public bool NoDialogsActive => !(SaveDialogActive || LoadDialogActive || ExceptionDialogActive);

		#endregion
	}
}