using DevilDaggersInfo.Tools.Scenes.GameObjects;
using DevilDaggersInfo.Tools.Ui.CustomLeaderboards;
using DevilDaggersInfo.Tools.Ui.Main;
using DevilDaggersInfo.Tools.Ui.ReplayEditor;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor;
using DevilDaggersInfo.Tools.User.Settings;
using DevilDaggersInfo.Tools.User.Settings.Model;
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
		Root.GameResources = GameResources.Create();

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

		const string installationConfigurationText = $"""
			Please configure your Devil Daggers installation directory. This is the directory containing the executable.

			The app will only start when a proper Devil Daggers installation is configured.

			Example: {examplePath}
			""";

		Vector2 center = ImGui.GetMainViewport().GetCenter();
		ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
		ImGui.SetNextWindowSize(new Vector2(768, 512));
		const ImGuiWindowFlags flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNavFocus | ImGuiWindowFlags.NoDocking;
		if (ImGui.Begin("Configuration", flags))
		{
			ImGui.SeparatorText("Installation Directory");

			ImGui.TextWrapped(installationConfigurationText);

			ImGui.Spacing();
			ImGui.Spacing();

			if (ImGui.BeginChild("Input", new Vector2(0, 296)))
			{
				if (ImGui.BeginChild("InputDirectory", new Vector2(0, 64)))
				{
					if (ImGui.Button("Browse", new Vector2(96, 20)))
						NativeFileDialog.SelectDirectory(OpenInstallationDirectoryCallback);

					ImGui.SameLine();
					ImGui.InputText("##installationDirectoryInput", ref _installationDirectoryInput, 1024, ImGuiInputTextFlags.None);

					ImGui.Spacing();
					ImGui.Spacing();

					if (!string.IsNullOrWhiteSpace(_error))
						ImGui.TextColored(new Vector4(1, 0, 0, 1), _error);
				}

				ImGui.EndChild(); // End InputDirectory

				ImGui.SeparatorText("Settings");

				float lookSpeed = UserSettings.Model.LookSpeed;
				ImGui.SliderFloat("Look speed", ref lookSpeed, UserSettingsModel.LookSpeedMin, UserSettingsModel.LookSpeedMax, "%.2f");
				if (Math.Abs(UserSettings.Model.LookSpeed - lookSpeed) > 0.001f)
					UserSettings.Model = UserSettings.Model with { LookSpeed = lookSpeed };

				int fieldOfView = UserSettings.Model.FieldOfView;
				ImGui.SliderInt("Field of view", ref fieldOfView, UserSettingsModel.FieldOfViewMin, UserSettingsModel.FieldOfViewMax);
				if (UserSettings.Model.FieldOfView != fieldOfView)
					UserSettings.Model = UserSettings.Model with { FieldOfView = fieldOfView };

				bool showDebug = UserSettings.Model.ShowDebug;
				ImGui.Checkbox("Show debug", ref showDebug);
				if (UserSettings.Model.ShowDebug != showDebug)
					UserSettings.Model = UserSettings.Model with { ShowDebug = showDebug };

				bool doNotShowAgainPracticeSpawnsetApplied = UserSettings.Model.DoNotShowAgainPracticeSpawnsetApplied;
				ImGui.Checkbox("Do not show message again when applying practice spawnset", ref doNotShowAgainPracticeSpawnsetApplied);
				if (UserSettings.Model.DoNotShowAgainPracticeSpawnsetApplied != doNotShowAgainPracticeSpawnsetApplied)
					UserSettings.Model = UserSettings.Model with { DoNotShowAgainPracticeSpawnsetApplied = doNotShowAgainPracticeSpawnsetApplied };

				bool doNotShowAgainPracticeSpawnsetDeleted = UserSettings.Model.DoNotShowAgainPracticeSpawnsetDeleted;
				ImGui.Checkbox("Do not show message again when deleting practice spawnset", ref doNotShowAgainPracticeSpawnsetDeleted);
				if (UserSettings.Model.DoNotShowAgainPracticeSpawnsetDeleted != doNotShowAgainPracticeSpawnsetDeleted)
					UserSettings.Model = UserSettings.Model with { DoNotShowAgainPracticeSpawnsetDeleted = doNotShowAgainPracticeSpawnsetDeleted };
			}

			ImGui.EndChild(); // End Input

			ImGui.PushFont(Root.FontGoetheBold30);
			if (ImGui.Button("Save and continue", new Vector2(752, 64)))
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

		if (ImGui.IsKeyPressed(ImGuiKey.Escape))
			ValidateInstallation();
	}

	private static void OpenInstallationDirectoryCallback(string? directory)
	{
		if (directory != null)
			_installationDirectoryInput = directory;
	}
}
