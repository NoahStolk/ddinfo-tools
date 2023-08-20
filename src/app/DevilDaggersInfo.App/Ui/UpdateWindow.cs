using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui;

public static class UpdateWindow
{
	private static bool _updateInProgress;

	public static Version? AvailableUpdateVersion { get; set; }

	public static List<string> LogMessages { get; } = new();

	public static void Render(ref bool show)
	{
		if (!show)
			return;

		Vector2 center = ImGui.GetMainViewport().GetCenter();
		Vector2 windowSize = new(384, 384);
		ImGui.SetNextWindowPos(center, ImGuiCond.Always, new(0.5f, 0.5f));
		ImGui.SetNextWindowSize(windowSize);
		if (ImGui.Begin("Update available", ref show, ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize))
		{
			ImGui.PushTextWrapPos(windowSize.X - 16);

			ImGui.Text($"""
				Version {AvailableUpdateVersion} is available.
				The current version is {Root.Application.AppVersion}.
				""");
			ImGui.Spacing();

			ImGui.BeginDisabled(_updateInProgress);
			if (ImGui.Button("Update and restart", new(160, 24)))
			{
				LogMessages.Clear();
				_updateInProgress = true;
				Task.Run(async () =>
				{
					await UpdateLogic.RunAsync();
					_updateInProgress = false;
				});
			}

			ImGui.EndDisabled();

			for (int i = 0; i < LogMessages.Count; i++)
				ImGui.Text(LogMessages[i]);

			ImGui.PopTextWrapPos();
		}

		ImGui.End();
	}
}
