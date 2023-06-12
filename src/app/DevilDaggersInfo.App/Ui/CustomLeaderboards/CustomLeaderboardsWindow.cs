using DevilDaggersInfo.App.Ui.CustomLeaderboards.Leaderboard;
using DevilDaggersInfo.App.Ui.CustomLeaderboards.LeaderboardList;
using ImGuiNET;

namespace DevilDaggersInfo.App.Ui.CustomLeaderboards;

public static class CustomLeaderboardsWindow
{
	private static float _recordingTimer;

	public static void Update(float delta)
	{
		CustomLeaderboards3DWindow.Update(delta);
		RecordingChild.Update(delta);

		_recordingTimer += delta;
		if (_recordingTimer < 0.12f)
			return;

		_recordingTimer = 0;
		if (!GameMemoryServiceWrapper.Scan())
			return;

		RecordingLogic.Handle();
	}

	public static void Render()
	{
#if DEBUG
		if (ImGui.Begin("Timestamps"))
		{
			foreach (DevilDaggersInfo.Api.App.CustomLeaderboards.AddUploadRequestTimestamp timestamp in RecordingLogic.Timestamps)
			{
				ImGui.Text($"{new DateTime(timestamp.Timestamp, DateTimeKind.Utc)} {timestamp.TimeInSeconds}");
			}
		}

		ImGui.End();
#endif

		ImGui.PushStyleVar(ImGuiStyleVar.WindowMinSize, Constants.MinWindowSize);
		ImGui.Begin("Custom Leaderboards", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollWithMouse);
		ImGui.PopStyleVar();

		ImGui.BeginChild("LeftRow", new(288, 464));

		StateChild.Render();
		RecordingChild.Render();

		ImGui.EndChild();

		ImGui.SameLine();

		ImGui.BeginChild("RightRow", new(0, 464));

		LeaderboardListChild.Render();
		LeaderboardListViewChild.Render();

		ImGui.EndChild();

		LeaderboardChild.Render();

		ImGui.End();

		if (ImGui.IsKeyPressed(ImGuiKey.Escape) || ImGui.IsKeyPressed((ImGuiKey)526))
			UiRenderer.Layout = LayoutType.Main;
	}
}
