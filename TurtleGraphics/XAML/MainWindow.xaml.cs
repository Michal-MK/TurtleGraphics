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
using System.Windows.Interop;
using Igor.Configuration;

namespace TurtleGraphics {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged {

		#region Notifications

		public event PropertyChangedEventHandler PropertyChanged;

		private void Notify(string prop) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
		}

		#endregion

		public const int PAGES_COLUMN_INDEX = 2;

		#region Bindings

		private string _color;
		private double _brushSize;
		private Point _startPoint;
		private ICommand _runCommand;
		private string _commandsText = "";
		private double _angle;
		private double _x;
		private double _y;
		private int _delay = 0;
		private bool _penDown;
		private int _iterationCount = 1;
		private ICommand _buttonCommand;
		private ICommand _stopCommand;
		private string _buttonText;
		private ICommand _runFullscreenCommand;
		private bool _toggleFullscreenEnabled = true;
		private bool _showTurtleCheckBox = true;
		private string _inteliCommandsText;
		private ICommand _saveCommand;
		private ICommand _loadCommand;
		private int _anotherDelay = 1;
		private PenLineCap _lineCapping;
		private bool _controlsVisible = true;
		private ICommand _controlsVisibleCommand;
		private bool _animatePath;
		private ImageSource _imgSource;
		private ICommand _settingsCommand;

		public ICommand SettingsCommand { get => _settingsCommand; set { _settingsCommand = value; Notify(nameof(SettingsCommand)); } }
		public ImageSource ImgSource { get => _imgSource; set { _imgSource = value; Notify(nameof(ImgSource)); } }
		public bool AnimatePath { get => _animatePath; set { _animatePath = value; Notify(nameof(AnimatePath)); } }
		public ICommand ControlsVisibleCommand { get => _controlsVisibleCommand; set { _controlsVisibleCommand = value; Notify(nameof(ControlsVisibleCommand)); } }
		public bool ControlsVisible { get => _controlsVisible; set { _controlsVisible = value; Notify(nameof(ControlsVisible)); } }
		public string AngleStr { get => $"{Math.Floor(ContextExtensions.AsDeg(Angle))}°"; }
		public string XStr { get => $"{Math.Round(X, 2)}"; }
		public string YStr { get => $"{Math.Round(Y, 2)}"; }
		public PenLineCap LineCapping { get => _lineCapping; set { _lineCapping = value; Notify(nameof(LineCapping)); } }
		public int CalculationFramesPreUIUpdate { get => _anotherDelay; set { _anotherDelay = value; Notify(nameof(CalculationFramesPreUIUpdate)); } }
		public ICommand LoadCommand { get => _loadCommand; set { _loadCommand = value; Notify(nameof(LoadCommand)); } }
		public ICommand SaveCommand { get => _saveCommand; set { _saveCommand = value; Notify(nameof(SaveCommand)); } }
		public string InteliCommandsText { get => _inteliCommandsText; set { _inteliCommandsText = value; Notify(nameof(InteliCommandsText)); } }
		public bool ShowTurtleCheckBox { get => _showTurtleCheckBox; set { _showTurtleCheckBox = value; Notify(nameof(ShowTurtleCheckBox)); } }
		public bool ToggleFullscreenEnabled { get => _toggleFullscreenEnabled; set { _toggleFullscreenEnabled = value; Notify(nameof(ToggleFullscreenEnabled)); } }
		public ICommand RunFullscreenCommand { get => _runFullscreenCommand; set { _runFullscreenCommand = value; Notify(nameof(ToggleFullScreenAction)); } }
		public string ButtonText { get => _buttonText; set { _buttonText = value; Notify(nameof(ButtonText)); } }
		public ICommand StopCommand { get => _stopCommand; set { _stopCommand = value; Notify(nameof(StopCommand)); } }
		public ICommand ButtonCommand { get => _buttonCommand; set { _buttonCommand = value; Notify(nameof(ButtonCommand)); } }
		public int IterationCount { get => _iterationCount; set { _iterationCount = value; Notify(nameof(IterationCount)); } }
		public bool PenDown { get => _penDown; set { if (value == _penDown) return; _penDown = value; NewPath(); Notify(nameof(PenDown)); } }
		public int PathAnimationFrames { get => _delay; set { _delay = value; Notify(nameof(PathAnimationFrames)); } }
		public double Y { get => _y; set { _y = value; Notify(nameof(Y)); Notify(nameof(YStr)); } }
		public double X { get => _x; set { _x = value; Notify(nameof(X)); Notify(nameof(XStr)); } }
		public double Angle { get => _angle; set { _angle = value; Notify(nameof(Angle)); Notify(nameof(AngleStr)); } }
		public string CommandsText { get => _commandsText; set { _commandsText = value; Notify(nameof(CommandsText)); } }
		public ICommand RunCommand { get => _runCommand; set { _runCommand = value; Notify(nameof(RunCommand)); } }
		public Point StartPoint { get => _startPoint; set { _startPoint = value; Notify(nameof(StartPoint)); } }
		public double BrushSize { get => _brushSize; set { if (value == _brushSize) return; _brushSize = value; NewPath(); Notify(nameof(BrushSize)); } }
		public string Color { get => _color; set { if (value == _color) return; _color = value; NewPath(); Notify(nameof(Color)); } }


		#endregion

		#region Language

		public class Lang {
			public string Main_WindowName => LocaleProvider.Instance.Get(Locale.MAIN__WINDOW_NAME);
			public string Main_Angle => LocaleProvider.Instance.Get(Locale.MAIN__ANGLE);
			public string Main_AnimatePath => LocaleProvider.Instance.Get(Locale.MAIN__ANIMATE_PATH);
			public string Main_BackgroundCol => LocaleProvider.Instance.Get(Locale.MAIN__BACKGROUND_COL);
			public string Main_PathAnimSpeed => LocaleProvider.Instance.Get(Locale.MAIN__PATH_ANIM_SPEED);
			public string Main_ToggleControlPanel => LocaleProvider.Instance.Get(Locale.MAIN__TOGGLE_CONTROL_PANEL);
			public string Main_TurtleSpeed => LocaleProvider.Instance.Get(Locale.MAIN__TURTLE_SPEED);
			public string Main_ShowTurtle => LocaleProvider.Instance.Get(Locale.MAIN__SHOW_TURTLE);
			public string GenericLoad => LocaleProvider.Instance.Get(Locale.GENERIC_LOAD);
			public string GenericSave => LocaleProvider.Instance.Get(Locale.GENERIC_SAVE);
			public string Main_RunFullscreen => LocaleProvider.Instance.Get(Locale.MAIN__RUN_FULLSCREEN) + " (Ctrl + F5)";
			public string Main_Settings => LocaleProvider.Instance.Get(Locale.MAIN__SETTINGS);
			public string GenericRun => LocaleProvider.Instance.Get(Locale.GENERIC_RUN) + " (F5)";
			public string GenericStop => LocaleProvider.Instance.Get(Locale.GENERIC_STOP) + " (F5)";
		}

		public Lang L { get; } = new Lang();

		#endregion

		#region Standard Properties

		private Path _currentPath;
		private PathFigure _currentFigure;
		private PolyLineSegment _currentSegment;
		private CancellationTokenSource cancellationTokenSource;
		private readonly InteliCommandsHandler _inteliCommands = new InteliCommandsHandler();
		private ScrollViewer _inteliCommandsScroller;
		private readonly CompilationStatus _compilationStatus = new CompilationStatus();
		private readonly ExceptionDisplay _exceptionDisplay = new ExceptionDisplay();
		private readonly BrushConverter _brushConverter = new BrushConverter();

		public double DrawWidth { get; set; }
		public double DrawHeight { get; set; }
		public static MainWindow Instance { get; set; }
		public FileSystemManager FSSManager { get; set; }
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

			FSSManager = new FileSystemManager();

			Loaded += MainWindow_Loaded;
			Closed += MainWindow_Closed;

			SizeChanged += MainWindow_SizeChanged;
			CommandsTextInput.SelectionChanged += CommandsTextInput_SelectionChanged;
			CommandsTextInput.TextChanged += CommandsTextInput_TextChanged;

			SetWindowState(App.Instance.LaunchFullScreen);
			if (App.Instance.LaunchHiddenControlPanel) {
				ToggleControlPanel();
			}

			DataContext = this;
			Instance = this;
		}

		public void RemoveAllPaths() {
			List<UIElement> toRemove = new List<UIElement>();
			foreach (UIElement child in Paths.Children) {
				if (child.GetType() == typeof(Path)) {
					toRemove.Add(child);
				}
			}

			foreach (var item in toRemove) {
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
			StartPoint = new Point(X, Y);
			Angle = 0;
			BrushSize = 4;
			PathAnimationFrames = 5;
			LineCapping = PenLineCap.Round;

			NewPath();
		}

		#region Events

		private void MainWindow_Loaded(object sender, RoutedEventArgs e) {
			DrawWidth = DrawAreaX.ActualWidth;
			DrawHeight = DrawAreaY.ActualHeight;
			_inteliCommandsScroller = FindDescendant<ScrollViewer>(InteliCommands);
			Init();
			CommandsText = FSSManager.RestoreCodeIfExists();
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
			PathAnimationFrames = deserialized.PathAnimationSpeed;
			CalculationFramesPreUIUpdate = deserialized.TurtleSpeed;
			BackgroundColor.Text = deserialized.BackgroundColor;
			ButtonCommand = StopCommand;
			await DrawData(deserialized.Data);
		}

		private void MainWindow_Closed(object sender, EventArgs e) {
			FSSManager.RestoreCodeIfExists();
		}

		private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e) {
			DrawWidth = DrawAreaX.ActualWidth;
			DrawHeight = DrawAreaY.ActualHeight;
			TurtleTranslation.X = DrawWidth / 2;
			TurtleTranslation.Y = DrawHeight / 2;
		}

		private void MainWindow_KeyDown(object sender, KeyEventArgs e) {
			if (e.Key == Key.Escape) {
				if (!ControlPanelHolderVisible) {
					ToggleControlPanel();
				}
				ImgSource = null;
				RemoveAllPaths();
				UpdateLayout();
				ToggleFullScreenAction();
			}
		}

		private void CommandsTextInput_SelectionChanged(object sender, RoutedEventArgs e) {
#if INTELI_COMMANDS
			_inteliCommands.Handle(this, CommandsTextInput);
			Notify(nameof(CommandsText));
			Notify(nameof(InteliCommandsText));
#endif
		}

		private void CommandsTextInput_ScrollChanged(object sender, ScrollChangedEventArgs e) {
#if INTELI_COMMANDS
			if (_inteliCommandsScroller != null) {
				_inteliCommandsScroller.ScrollToVerticalOffset(e.VerticalOffset);
				_inteliCommandsScroller.ScrollToHorizontalOffset(e.HorizontalOffset);
			}
#endif
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
#if INTELI_COMMANDS
			InteliCommandsText = CommandsTextInput.Text;
#endif
		}

		protected override void OnKeyDown(KeyEventArgs e) {
			if (e.Key == Key.F11 && ControlPanelHolderVisible) {
				SetWindowState(!IsFullscreen);
			}
			if (e.Key == Key.F5) {
				if (!ExceptionDialogActive) {
					if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.LeftCtrl)) {
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

		private async Task Capture() {
			var (actualWidth, actualHeight) = GetActualScreenSize();

			await Task.Run(() => {
				using (MemoryStream ms = CaptureScreenshot(actualWidth, actualHeight)) {
					var i = new BitmapImage();
					i.BeginInit();
					i.CacheOption = BitmapCacheOption.OnLoad;
					i.StreamSource = ms;
					i.EndInit();
					i.Freeze();
					Dispatcher.Invoke(() => {
						ImgSource = i;
						const int OFFSET = 3;
						Paths.Children.RemoveRange(OFFSET, Paths.Children.Count - OFFSET - 80);
					});
				}
			});
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
			PresentationSource presentationSource = PresentationSource.FromVisual(Application.Current.MainWindow);
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
					case ParsedAction.NONE: { break; }
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
						(Point _point, double _angle) = storedPositions.Peek();
						if (data.PopPosition) {
							storedPositions.Pop();
						}
						X = _point.X;
						Y = _point.Y;
						Rotate(_angle, true);
						NewPath();
						break;
					}
					case ParsedAction.ScreenCapture: {
						if (IsFullscreen && !ControlPanelHolderVisible) {
							await Capture();
						}
						break;
					}
				}
			}
		}

		public async Task Draw(double _x, double _y) {
			_currentSegment.Points.Add(new Point(X, Y));
			X = _x;
			Y = _y;
			await Displace(_x, _y);
		}


		public async Task Displace(double _x, double _y) {
			int last = _currentSegment.Points.Count - 1;
			Point origin = _currentSegment.Points[last];
			double increment = 1d / IterationCount;
			double currentInterpolation = 0;

			for (int i = 0; i <= IterationCount; i++) {
				if (cancellationTokenSource.Token.IsCancellationRequested) {
					break;
				}
				_currentSegment.Points[last] = new Point(Lerp(origin.X, _x, currentInterpolation), Lerp(origin.Y, _y, currentInterpolation));
				TurtleTranslation.X = _currentSegment.Points[last].X;
				TurtleTranslation.Y = _currentSegment.Points[last].Y;
				currentInterpolation += increment;
				if (AnimatePath) {
					await Task.Delay(1);
				}
			}
		}

		#endregion


		#region Actions

		private async Task RunCommandAction() {
			_compilationStatus.Start();
			ToggleFullscreenEnabled = false;
			Init();
			cancellationTokenSource = new CancellationTokenSource();
			ButtonCommand = StopCommand;
			ButtonText = L.GenericStop;
			try {
				FSSManager.CreateCodeBackup(CommandsText);
				Queue<ParsedData> tasks = CommandParser.ParseCommands(CommandsText, this);
				List<TurtleData> compiledTasks = await CompileTasks(tasks, cancellationTokenSource.Token);
				_compilationStatus.Stop();
				Stopwatch s = new Stopwatch();
				s.Start();
				await DrawData(compiledTasks);
				s.Stop();
			}
			catch (OperationCanceledException) {
				//Operation was canceled
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
				List<TurtleData> ret = new List<TurtleData>(8192) {
					new TurtleData() { Angle = Angle, Brush = Brushes.Blue, BrushThickness = BrushSize, MoveTo = new Point(X, Y), PenDown = true }
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
			});
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
			}
			else {
				SplitterCol.Width = new GridLength(0);
				ControlArea.Width = new GridLength(0, GridUnitType.Pixel);
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
				if (WindowState == WindowState.Minimized) {
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
			if (!NoWindowsActive)
				return;
			LoadDialogActive = true;
			SavedData data = await FSSManager.Load();
			LoadDialogActive = false;
			if (data.Name != null) {
				CommandsText = data.Code;
			}
		}

		public void SaveCommandAction() {
			if (!NoWindowsActive)
				return;
			SaveDialogActive = true;
			SaveDialog d = new SaveDialog();
			Grid.SetColumn(d, PAGES_COLUMN_INDEX);
			Paths.Children.Add(d);
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
				BringIntoView();
				App.Instance.Cfg.Save();
			}
		}


		public bool NoWindowsActive => !(SaveDialogActive || LoadDialogActive || ExceptionDialogActive);

		#endregion
	}
}
