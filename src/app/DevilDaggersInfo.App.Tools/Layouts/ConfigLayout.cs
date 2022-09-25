using DevilDaggersInfo.App.Ui.Base;
using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern.Inversion.Layouts;
using DevilDaggersInfo.App.Ui.Base.Enums;
using DevilDaggersInfo.App.Ui.Base.Settings;
using DevilDaggersInfo.App.Ui.Base.States;
using Warp.Ui;

namespace DevilDaggersInfo.App.Tools.Layouts;

public class ConfigLayout : Layout, IExtendedLayout
{
	private readonly TextInput _textInput;
	private readonly List<string> _errors = GetErrors().ToList();

	private bool _initialTryDone;

	public ConfigLayout()
		: base(Constants.Full)
	{
		_textInput = new(Rectangle.At(32, 128, 960, 16), false, Color.Black, Color.White, Color.Gray(32), Color.White, Color.White, Color.White, Color.Gray(64), 2, FontSize.F8X8);
		_textInput.SetText(UserSettings.DevilDaggersInstallationDirectory);
		NestingContext.Add(_textInput);

		NestingContext.Add(new Button(Rectangle.At(32, 320, 256, 32), Check, Color.Black, Color.White, Color.Gray(32), Color.White, "Save and continue", TextAlign.Middle, 2, FontSize.F8X8));
	}

	private void Check()
	{
		UserSettings.DevilDaggersInstallationDirectory = _textInput.Value.ToString();
		_errors.Clear();
		_errors.AddRange(GetErrors());
		ProceedIfOk();
	}

	private static IEnumerable<string> GetErrors()
	{
		string dir = UserSettings.DevilDaggersInstallationDirectory;
		if (!Directory.Exists(dir))
			yield return "Installation directory does not exist.";

		// TODO: Linux paths.
		string ddExe = Path.Combine(dir, "dd.exe");
		string survival = Path.Combine(dir, "dd", "survival");
		string resAudio = Path.Combine(dir, "res", "audio");
		string resDd = Path.Combine(dir, "res", "dd");

		if (!File.Exists(ddExe))
			yield return "Executable does not exist.";

		if (!File.Exists(survival))
			yield return "File 'survival' does not exist.";

		if (!File.Exists(resAudio))
			yield return "File 'res/audio' does not exist.";

		if (!File.Exists(resDd))
			yield return "File 'res/dd' does not exist.";

		// TODO: Might also want to check if the files themselves are actually valid.
	}

	private void ProceedIfOk()
	{
		if (_errors.Count > 0)
			return;

		LayoutManager.ToMainLayout();
		Root.Game.MainLayout.InitializeScene();
	}

	public void Update()
	{
		if (_initialTryDone)
			return;

		ProceedIfOk();
		_initialTryDone = true;
	}

	public void Render3d()
	{
	}

	public void Render()
	{
		Root.Game.UiRenderer.RenderTopLeft(new(WindowWidth, WindowHeight), default, -100, new(0.1f));
	}

	public void RenderText()
	{
		Root.Game.FontRenderer12X12.Render(Vector2i<int>.One, new(32, 32), 0, Color.White, "SETTINGS", TextAlign.Left);

// 		const string text = """
// 			Please configure your Devil Daggers installation directory.
//
// 			This is the directory containing the executable.
//
// 			Example: C:\Program Files (x86)\Steam\steamapps\common\devildaggers
// 			""";
		const string text = "Please configure your Devil Daggers installation directory.\n\nThis is the directory containing the executable.\n\nExample: C:\\Program Files (x86)\\Steam\\steamapps\\common\\devildaggers";
		Root.Game.FontRenderer8X8.Render(Vector2i<int>.One, new(32, 64), 0, Color.White, text, TextAlign.Left);
		Root.Game.FontRenderer8X8.Render(Vector2i<int>.One, new(32, 160), 0, Color.Red, string.Join("\n\n", _errors), TextAlign.Left);
	}
}
