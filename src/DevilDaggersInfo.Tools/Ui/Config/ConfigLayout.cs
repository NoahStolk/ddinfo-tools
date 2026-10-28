using DevilDaggersInfo.Tools.Dialogs;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.User.Settings;
using DevilDaggersInfo.Tools.User.Settings.Model;
using Hexa.NET.ImGui;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.Config;

internal sealed class ConfigLayout(GameInstallationValidator gameInstallationValidator, INativeFileDialog nativeFileDialog, FontService fontService, UserSettings userSettings)
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

		Vector2 center = ImGui.GetCenter(ImGui.GetMainViewport());
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

				Checkbox(
					"Show debug",
					s => s.ShowDebug,
					b => userSettings.Model with { ShowDebug = b },
					"Enables the debug window.");
				Checkbox(
					"Do not show message again when applying practice spawnset",
					s => s.DoNotShowAgainPracticeSpawnsetApplied,
					b => userSettings.Model with { DoNotShowAgainPracticeSpawnsetApplied = b },
					null);
				Checkbox(
					"Do not show message again when deleting practice spawnset",
					s => s.DoNotShowAgainPracticeSpawnsetDeleted,
					b => userSettings.Model with { DoNotShowAgainPracticeSpawnsetDeleted = b },
					null);
				Checkbox(
					"Do not show practice warning",
					s => s.DoNotShowAgainPracticeWarning,
					b => userSettings.Model with { DoNotShowAgainPracticeWarning = b },
					"Prevents showing the practice warning. If the app knows you already have 500 seconds or more, the warning won't show regardless of the setting.");
			}

			ImGui.EndChild();

			ImGui.PushFont(fontService.GoetheBold30);
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

	private void Checkbox(string label, Func<UserSettingsModel, bool> selector, Func<bool, UserSettingsModel> setter, string? tooltipText)
	{
		bool showDebug = selector(userSettings.Model);
		ImGui.Checkbox(label, ref showDebug);

		if (tooltipText != null)
		{
			ImGui.SameLine();
			ImGui.TextColored(new Vector4(0.7f, 0.7f, 0.7f, 1), "(?)");
			if (ImGui.IsItemHovered())
				ImGui.SetTooltip(tooltipText);
		}

		if (selector(userSettings.Model) != showDebug)
			userSettings.Model = setter(showDebug);
	}

	private void OpenInstallationDirectoryCallback(string? directory)
	{
		if (directory != null)
			gameInstallationValidator.InstallationDirectoryInput = directory;
	}
}
