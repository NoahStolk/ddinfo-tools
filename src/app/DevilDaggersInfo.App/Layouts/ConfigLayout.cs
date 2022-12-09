using DevilDaggersInfo.App.Ui.Base;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern.Inversion.Layouts;
using DevilDaggersInfo.App.Ui.Base.Exceptions;
using DevilDaggersInfo.App.Ui.Base.Settings;
using DevilDaggersInfo.App.Ui.Base.States;
using DevilDaggersInfo.App.Ui.Scene.GameObjects;
using Warp.NET.RenderImpl.Ui.Components;
using Warp.NET.Text;
using Warp.NET.Ui;

namespace DevilDaggersInfo.App.Layouts;

public class ConfigLayout : Layout, IConfigLayout
{
	private readonly TextInput _textInput;
	private string? _error;

	public ConfigLayout()
	{
		_textInput = new(new PixelBounds(32, 128, 960, 16), false, null, null, null, GlobalStyles.TextInput);
		_textInput.KeyboardInput.SetText(UserSettings.DevilDaggersInstallationDirectory);
		NestingContext.Add(_textInput);

		NestingContext.Add(new TextButton(new PixelBounds(32, 320, 256, 32), Check, GlobalStyles.DefaultButtonStyle, GlobalStyles.ConfigButton, "Save and continue"));
	}

	private void Check()
	{
		UserSettings.DevilDaggersInstallationDirectory = _textInput.KeyboardInput.Value.ToString();
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
		Tile.Initialize();
	}

	public void Update()
	{
	}

	public void Render3d()
	{
	}

	public void Render()
	{
		Game.Self.RectangleRenderer.Schedule(new(CurrentWindowState.Width, CurrentWindowState.Height), default, -100, Color.Gray(0.1f));

		Game.Self.MonoSpaceFontRenderer16.Schedule(Vector2i<int>.One, new(32, 32), 0, Color.White, "SETTINGS", TextAlign.Left);

 #pragma warning disable S1075
#if LINUX
		const string examplePath = "/home/noah/.local/share/Steam/steamapps/common/devildaggers/";
#elif WINDOWS
		const string examplePath = """C:\Program Files (x86)\Steam\steamapps\common\devildaggers""";
#else
		const string examplePath = "(no example for this operating system)";
#endif
 #pragma warning restore S1075

		const string text = $"""
			Please configure your Devil Daggers installation directory.

			This is the directory containing the executable.

			Example: {examplePath}
			""";
		Game.Self.MonoSpaceFontRenderer12.Schedule(Vector2i<int>.One, new(32, 64), 0, Color.White, text, TextAlign.Left);
		if (!string.IsNullOrWhiteSpace(_error))
			Game.Self.MonoSpaceFontRenderer12.Schedule(Vector2i<int>.One, new(32, 160), 0, Color.Red, _error, TextAlign.Left);
	}
}
