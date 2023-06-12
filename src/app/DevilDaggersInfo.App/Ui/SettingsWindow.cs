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

		if (ImGui.Begin("Settings", ref show, ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize))
		{
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
		}

		ImGui.End();
	}
}
