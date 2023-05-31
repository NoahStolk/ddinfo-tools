using DevilDaggersInfo.App.Scenes.GameObjects;
using DevilDaggersInfo.App.Ui.CustomLeaderboards;
using DevilDaggersInfo.App.Ui.Main;
using DevilDaggersInfo.App.Ui.ReplayEditor;
using DevilDaggersInfo.App.Ui.SpawnsetEditor;
using DevilDaggersInfo.App.User.Settings;
using ImGuiNET;

namespace DevilDaggersInfo.App.Ui.Config;

public static class ConfigLayout
{
	private static string? _error;
	private static bool _contentInitialized;
	private static string _installationDirectoryInput = string.Empty;

	/// <summary>
	/// Is called on launch, and when the user changes the installation directory.
	/// Must be called on the main thread.
	/// </summary>
	public static void ValidateInstallation()
	{
		_installationDirectoryInput = UserSettings.Model.DevilDaggersInstallationDirectory;

		try
		{
			ContentManager.Initialize();
		}
		catch (InvalidGameInstallationException ex)
		{
			_error = ex.Message;
			return;
		}

		UiRenderer.Layout = LayoutType.Main;
		_error = null;

		if (_contentInitialized)
			return;

		// Initialize game resources.
		Root.GameResources = GameResources.Create(Root.Gl);

		// Initialize 3D rendering.
		Player.InitializeRendering();
		RaceDagger.InitializeRendering();
		Tile.InitializeRendering();
		Skull4.InitializeRendering();

		// Initialize scenes.
		MainLayout.InitializeScene();
		SpawnsetEditor3DWindow.InitializeScene();
		CustomLeaderboards3DWindow.InitializeScene();
		ReplayEditor3DWindow.InitializeScene();

		// Initialize file watchers.
		SurvivalFileWatcher.Initialize();

		_contentInitialized = true;
	}

	public static void Render()
	{
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

		ImGui.Text(text);
		ImGui.Spacing();

		ImGui.BeginChild("Input", new(1366, 128));
		if (ImGui.Button("Select installation directory", new(224, 24)))
		{
			string? directory = NativeFileDialog.SelectDirectory();
			if (directory != null)
				_installationDirectoryInput = directory;
		}

		ImGui.InputText("##installationDirectoryInput", ref _installationDirectoryInput, 1024, ImGuiInputTextFlags.None);

		if (!string.IsNullOrWhiteSpace(_error))
			ImGui.TextColored(new(1, 0, 0, 1), _error);

		ImGui.EndChild();

		if (ImGui.Button("Save and continue", new(192, 96)))
		{
			UserSettings.Model = UserSettings.Model with
			{
				DevilDaggersInstallationDirectory = _installationDirectoryInput,
			};

			ValidateInstallation();
		}
	}
}
