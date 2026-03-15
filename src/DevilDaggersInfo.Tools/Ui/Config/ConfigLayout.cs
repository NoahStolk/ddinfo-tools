using DevilDaggersInfo.Tools.User.Settings;
using DevilDaggersInfo.Tools.User.Settings.Model;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.Config;

internal sealed class ConfigLayout(GameInstallationValidator gameInstallationValidator, NativeFileDialog nativeFileDialog, UserSettings userSettings)
{
	public void Render()
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
						nativeFileDialog.SelectDirectory(OpenInstallationDirectoryCallback);

					ImGui.SameLine();
					ImGui.InputText("##installationDirectoryInput", ref gameInstallationValidator.InstallationDirectoryInput, 1024, ImGuiInputTextFlags.None);

					ImGui.Spacing();
					ImGui.Spacing();

					if (!string.IsNullOrWhiteSpace(gameInstallationValidator.Error))
						ImGui.TextColored(new Vector4(1, 0, 0, 1), gameInstallationValidator.Error);
				}

				ImGui.EndChild();

				ImGui.SeparatorText("Settings");

				float lookSpeed = userSettings.Model.LookSpeed;
				ImGui.SliderFloat("Look speed", ref lookSpeed, UserSettingsModel.LookSpeedMin, UserSettingsModel.LookSpeedMax, "%.2f");
				if (Math.Abs(userSettings.Model.LookSpeed - lookSpeed) > 0.001f)
					userSettings.Model = userSettings.Model with { LookSpeed = lookSpeed };

				int fieldOfView = userSettings.Model.FieldOfView;
				ImGui.SliderInt("Field of view", ref fieldOfView, UserSettingsModel.FieldOfViewMin, UserSettingsModel.FieldOfViewMax);
				if (userSettings.Model.FieldOfView != fieldOfView)
					userSettings.Model = userSettings.Model with { FieldOfView = fieldOfView };

				bool showDebug = userSettings.Model.ShowDebug;
				ImGui.Checkbox("Show debug", ref showDebug);
				if (userSettings.Model.ShowDebug != showDebug)
					userSettings.Model = userSettings.Model with { ShowDebug = showDebug };

				bool doNotShowAgainPracticeSpawnsetApplied = userSettings.Model.DoNotShowAgainPracticeSpawnsetApplied;
				ImGui.Checkbox("Do not show message again when applying practice spawnset", ref doNotShowAgainPracticeSpawnsetApplied);
				if (userSettings.Model.DoNotShowAgainPracticeSpawnsetApplied != doNotShowAgainPracticeSpawnsetApplied)
					userSettings.Model = userSettings.Model with { DoNotShowAgainPracticeSpawnsetApplied = doNotShowAgainPracticeSpawnsetApplied };

				bool doNotShowAgainPracticeSpawnsetDeleted = userSettings.Model.DoNotShowAgainPracticeSpawnsetDeleted;
				ImGui.Checkbox("Do not show message again when deleting practice spawnset", ref doNotShowAgainPracticeSpawnsetDeleted);
				if (userSettings.Model.DoNotShowAgainPracticeSpawnsetDeleted != doNotShowAgainPracticeSpawnsetDeleted)
					userSettings.Model = userSettings.Model with { DoNotShowAgainPracticeSpawnsetDeleted = doNotShowAgainPracticeSpawnsetDeleted };
			}

			ImGui.EndChild();

			ImGui.PushFont(Root.FontGoetheBold30);
			if (ImGui.Button("Save and continue", new Vector2(752, 64)))
			{
				userSettings.Model = userSettings.Model with
				{
					DevilDaggersInstallationDirectory = gameInstallationValidator.InstallationDirectoryInput,
				};

				gameInstallationValidator.ValidateInstallation();
			}

			ImGui.PopFont();
		}

		ImGui.End();

		if (ImGui.IsKeyPressed(ImGuiKey.Escape))
			gameInstallationValidator.ValidateInstallation();
	}

	private void OpenInstallationDirectoryCallback(string? directory)
	{
		if (directory != null)
			gameInstallationValidator.InstallationDirectoryInput = directory;
	}
}
