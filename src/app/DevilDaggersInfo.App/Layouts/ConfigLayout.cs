using DevilDaggersInfo.App.Ui.Base;
using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.Exceptions;
using DevilDaggersInfo.App.Ui.Base.Rendering.Text;
using DevilDaggersInfo.App.Ui.Base.Settings;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.StateManagement.Base.Actions;
using DevilDaggersInfo.App.Ui.Base.Styling;
using Warp.NET.Text;
using Warp.NET.Ui;

namespace DevilDaggersInfo.App.Layouts;

public class ConfigLayout : Layout, IExtendedLayout
{
	private readonly TextInput _installationDirectoryInput;

	private string? _error;
	private bool _contentInitialized;

	public ConfigLayout()
	{
		TextButton installationDirectoryButton = new(new PixelBounds(32, 208, 256, 32), PickInstallationDirectory, ButtonStyles.Default, new(Color.White, TextAlign.Middle, FontSize.H12), "Select installation directory");
		NestingContext.Add(installationDirectoryButton);

		_installationDirectoryInput = new(new PixelBounds(320, 208, 640, 32), false, null, null, null, TextInputStyles.Default with { FontSize = FontSize.H16 });
		NestingContext.Add(_installationDirectoryInput);

		NestingContext.Add(new TextButton(new PixelBounds(384, 384, 256, 64), CheckInstallationDirectory, ButtonStyles.Default, new(Color.White, TextAlign.Middle, FontSize.H24), "Save and continue"));

		StateManager.Subscribe<SetLayout>(SetLayout);
		StateManager.Subscribe<UserSettingsLoaded>(ValidateInstallation);
	}

	private void PickInstallationDirectory()
	{
		string? directory = Root.Dependencies.NativeFileSystemService.SelectDirectory();
		if (directory != null)
			_installationDirectoryInput.KeyboardInput.SetText(directory);
	}

	private void CheckInstallationDirectory()
	{
		UserSettings.Model = UserSettings.Model with
		{
			DevilDaggersInstallationDirectory = _installationDirectoryInput.KeyboardInput.Value.ToString(),
		};
		ValidateInstallation();
	}

	private void ValidateInstallation()
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

		StateManager.Dispatch(new SetLayout(Root.Dependencies.MainLayout));
		_error = null;

		if (_contentInitialized)
			return;

		StateManager.Dispatch(new InitializeContent());
		_contentInitialized = true;
	}

	private void SetLayout()
	{
		if (StateManager.LayoutState.CurrentLayout != Root.Dependencies.ConfigLayout)
			return;

		_installationDirectoryInput.KeyboardInput.SetText(UserSettings.Model.DevilDaggersInstallationDirectory);
	}

	public void Update()
	{
	}

	public void Render3d()
	{
	}

	public void Render()
	{
		Vector2i<int> windowScale = new(CurrentWindowState.Width, CurrentWindowState.Height);
		Game.Self.RectangleRenderer.Schedule(windowScale, windowScale / 2, -100, Color.Gray(0.1f));

		Game.Self.MonoSpaceFontRenderer32.Schedule(Vector2i<int>.One, new(512, 64), 0, Color.White, "Configuration", TextAlign.Middle);

#pragma warning disable S1075
#if LINUX
		const string examplePath = "/home/{USERNAME}/.local/share/Steam/steamapps/common/devildaggers/";
#elif WINDOWS
		const string examplePath = """C:\Program Files (x86)\Steam\steamapps\common\devildaggers""";
#endif
#pragma warning restore S1075

		const string text = $"""
			Please configure your Devil Daggers installation directory.

			This is the directory containing the executable.

			Example: {examplePath}
			""";
		Game.Self.MonoSpaceFontRenderer12.Schedule(Vector2i<int>.One, new(32, 128), 0, Color.White, text, TextAlign.Left);
		if (!string.IsNullOrWhiteSpace(_error))
			Game.Self.MonoSpaceFontRenderer12.Schedule(Vector2i<int>.One, new(32, 256), 0, Color.Red, _error, TextAlign.Left);
	}
}
