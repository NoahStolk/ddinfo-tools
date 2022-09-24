using DevilDaggersInfo.App.Tools.States;
using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern.Inversion.Layouts;
using DevilDaggersInfo.App.Ui.Base.Enums;
using Warp.Ui;

namespace DevilDaggersInfo.App.Tools.Layouts;

public class ConfigLayout : Layout, IExtendedLayout
{
	private readonly TextInput _textInput;
	private readonly List<string> _errors = GetErrors().ToList();

	public ConfigLayout()
		: base(new(0, 0, 1920, 1080))
	{
		_textInput = new(Rectangle.At(256, 320, 320, 32), false, Color.Black, Color.White, Color.Purple, Color.White, Color.White, Color.White, Color.White, 2, 2);
		_textInput.SetText(ConfigStateManager.DevilDaggersInstallationDirectory);
		NestingContext.Add(_textInput);

		NestingContext.Add(new Button(Rectangle.At(256, 420, 320, 32), Check, Color.Black, Color.White, Color.Purple, Color.White, "Check", TextAlign.Middle, 10, false));
		NestingContext.Add(new Button(Rectangle.At(256, 480, 320, 32), GoToMain, Color.Black, Color.White, Color.Purple, Color.White, "OK", TextAlign.Middle, 10, false));
	}

	private void Check()
	{
		ConfigStateManager.DevilDaggersInstallationDirectory = _textInput.Value.ToString();
		_errors.Clear();
		_errors.AddRange(GetErrors());
	}

	private static IEnumerable<string> GetErrors()
	{
		string dir = ConfigStateManager.DevilDaggersInstallationDirectory;
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

	private void GoToMain()
	{
		if (_errors.Count > 0)
			return;

		Root.Game.ActiveLayout = Root.Game.MainLayout;
		Root.Game.MainLayout.InitializeScene();
	}

	public void Update()
	{
	}

	public void Render3d()
	{
	}

	public void Render()
	{
	}

	public void RenderText()
	{
		string text = _errors.Count == 0 ? "OK!" : string.Join("\n", _errors);
		Root.Game.MonoSpaceFontRenderer.Render(Vector2i<int>.One, new(640, 640), 0, _errors.Count == 0 ? Color.Green : Color.Red, text, TextAlign.Left);
	}
}
