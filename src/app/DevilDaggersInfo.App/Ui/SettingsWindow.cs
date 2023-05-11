using DevilDaggersInfo.App.User.Settings;
using DevilDaggersInfo.App.User.Settings.Model;
using ImGuiNET;

namespace DevilDaggersInfo.App.Ui;

public static class SettingsWindow
{
	public static void Render(ref bool show)
	{
		if (!show)
			return;

		ImGui.SetNextWindowSize(new(512, 256));

		ImGui.Begin("Settings", ref show, ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize);

		bool verticalSync = UserSettings.Model.VerticalSync;
		ImGui.Checkbox("V-sync", ref verticalSync);
		if (UserSettings.Model.VerticalSync != verticalSync)
			UserSettings.Model = UserSettings.Model with { VerticalSync = verticalSync };

		ImGui.BeginDisabled(verticalSync);
		int maxFps = UserSettings.Model.MaxFps;
		ImGui.SliderInt("Max FPS", ref maxFps, UserSettingsModel.MaxFpsMin, UserSettingsModel.MaxFpsMax);
		if (UserSettings.Model.MaxFps != maxFps)
			UserSettings.Model = UserSettings.Model with { MaxFps = maxFps };

		ImGui.EndDisabled();

		float lookSpeed = UserSettings.Model.LookSpeed;
		ImGui.SliderFloat("Look speed", ref lookSpeed, UserSettingsModel.LookSpeedMin, UserSettingsModel.LookSpeedMax);
		if (Math.Abs(UserSettings.Model.LookSpeed - lookSpeed) > 0.001f)
			UserSettings.Model = UserSettings.Model with { LookSpeed = lookSpeed };

		int fieldOfView = UserSettings.Model.FieldOfView;
		ImGui.SliderInt("Field of view", ref fieldOfView, UserSettingsModel.FieldOfViewMin, UserSettingsModel.FieldOfViewMax);
		if (UserSettings.Model.FieldOfView != fieldOfView)
			UserSettings.Model = UserSettings.Model with { FieldOfView = fieldOfView };

		bool showDebugWindow = UserSettings.Model.ShowDebugWindow;
		ImGui.Checkbox("Show debug window", ref showDebugWindow);
		if (UserSettings.Model.ShowDebugWindow != showDebugWindow)
			UserSettings.Model = UserSettings.Model with { ShowDebugWindow = showDebugWindow };

		ImGui.End();
	}
}
