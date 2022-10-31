using DevilDaggersInfo.App.Ui.Base;
using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern.Inversion.Layouts;
using DevilDaggersInfo.App.Ui.Base.Enums;
using DevilDaggersInfo.App.Ui.Base.Exceptions;
using DevilDaggersInfo.App.Ui.Base.Rendering;
using DevilDaggersInfo.App.Ui.Base.Settings;
using DevilDaggersInfo.App.Ui.Base.States;
using Warp.Ui;

namespace DevilDaggersInfo.App.Layouts;

public class ConfigLayout : Layout, IConfigLayout
{
	private readonly TextInput _textInput;
	private string? _error;

	public ConfigLayout()
		: base(Constants.Full)
	{
		_textInput = ComponentBuilder.CreateTextInput(Rectangle.At(32, 128, 960, 16), false, null, null, null);
		_textInput.SetText(UserSettings.DevilDaggersInstallationDirectory);
		NestingContext.Add(_textInput);

		NestingContext.Add(new TextButton(Rectangle.At(32, 320, 256, 32), Check, Color.Black, Color.White, Color.Gray(0.25f), Color.White, "Save and continue", TextAlign.Middle, 2, FontSize.F8X8));
	}

	private void Check()
	{
		UserSettings.DevilDaggersInstallationDirectory = _textInput.Value.ToString();
		ValidateInstallation();
	}

	public void ValidateInstallation()
	{
		try
		{
			ContentManager.Initialize();
		}
		catch (MissingContentException ex)
		{
			_error = ex.Message;
			return;
		}

		LayoutManager.ToMainLayout();
		Root.Game.MainLayout.InitializeScene();
		Root.Game.SurvivalEditor3dLayout.Initialize();
	}

	public void Update()
	{
	}

	public void Render3d()
	{
	}

	public void Render()
	{
		RenderBatchCollector.RenderRectangleTopLeft(new(WindowWidth, WindowHeight), default, -100, Color.Gray(0.1f));

		RenderBatchCollector.RenderMonoSpaceText(FontSize.F12X12, Vector2i<int>.One, new(32, 32), 0, Color.White, "SETTINGS", TextAlign.Left);

#if LINUX
		const string examplePath = "/home/noah/.local/share/Steam/steamapps/common/devildaggers/";
#elif WINDOWS
		const string examplePath = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\devildaggers";
#else
		const string examplePath = "(no example for this operating system)";
#endif

// 		const string text = """
// 			Please configure your Devil Daggers installation directory.
//
// 			This is the directory containing the executable.
//
// 			Example: {examplePath}
// 			""";
		const string text = $"Please configure your Devil Daggers installation directory.\n\nThis is the directory containing the executable.\n\nExample: {examplePath}";
		RenderBatchCollector.RenderMonoSpaceText(FontSize.F8X8, Vector2i<int>.One, new(32, 64), 0, Color.White, text, TextAlign.Left);
		RenderBatchCollector.RenderMonoSpaceText(FontSize.F8X8, Vector2i<int>.One, new(32, 160), 0, Color.Red, _error, TextAlign.Left);
	}
}
