using DevilDaggersInfo.Tools.Scenes.GameObjects;
using DevilDaggersInfo.Tools.Ui.CustomLeaderboards;
using DevilDaggersInfo.Tools.Ui.Main;
using DevilDaggersInfo.Tools.Ui.ReplayEditor;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor;
using DevilDaggersInfo.Tools.User.Settings;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.Config;

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
		MainScene.Initialize();
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

		Vector2 center = ImGui.GetMainViewport().GetCenter();
		ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new(0.5f, 0.5f));
		ImGui.SetNextWindowSize(new(768, 512));
		ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(16, 16));
		const ImGuiWindowFlags flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNavFocus;
		if (ImGui.Begin("Configuration", flags))
		{
			ImGui.Text(text);

			ImGui.Spacing();
			ImGui.Spacing();

			if (ImGui.BeginChild("Input", new(0, 316)))
			{
				if (ImGui.Button("Browse", new(96, 20)))
				{
					string? directory = NativeFileDialog.SelectDirectory();
					if (directory != null)
						_installationDirectoryInput = directory;
				}

				ImGui.SameLine();
				ImGui.InputText("##installationDirectoryInput", ref _installationDirectoryInput, 1024, ImGuiInputTextFlags.None);

				ImGui.Spacing();
				ImGui.Spacing();

				if (!string.IsNullOrWhiteSpace(_error))
					ImGui.TextColored(new(1, 0, 0, 1), _error);
			}

			ImGui.EndChild(); // End Input

			ImGui.PushFont(Root.FontGoetheBold30);
			if (ImGui.Button("Save and continue", new(736, 64)))
			{
				UserSettings.Model = UserSettings.Model with
				{
					DevilDaggersInstallationDirectory = _installationDirectoryInput,
				};

				ValidateInstallation();
			}

			ImGui.PopFont();
		}

		ImGui.End(); // End Configuration

		ImGui.PopStyleVar();

		if (ImGui.IsKeyPressed(ImGuiKey.Escape))
			ValidateInstallation();
	}
}
