using DevilDaggersInfo.Tools.Ui.CustomLeaderboards.Leaderboard;
using DevilDaggersInfo.Tools.Ui.CustomLeaderboards.LeaderboardList;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.CustomLeaderboards;

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
			foreach (DevilDaggersInfo.Web.ApiSpec.Tools.CustomLeaderboards.AddUploadRequestTimestamp timestamp in RecordingLogic.Timestamps)
			{
				ImGui.Text(Inline.Span($"{new DateTime(timestamp.Timestamp, DateTimeKind.Utc)} {timestamp.TimeInSeconds}"));
			}
		}

		ImGui.End();
#endif

		ImGuiUtils.SetNextWindowMinSize(Constants.MinWindowSize);
		if (ImGui.Begin("Custom Leaderboards", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollWithMouse))
		{
			if (ImGui.BeginChild("LeftRow", new Vector2(288, 464)))
			{
				StateChild.Render();
				RecordingChild.Render();
			}

			ImGui.EndChild();

			ImGui.SameLine();

			if (ImGui.BeginChild("RightRow", new Vector2(0, 464)))
			{
				LeaderboardListChild.Render();
				LeaderboardListViewChild.Render();
			}

			ImGui.EndChild();

			LeaderboardChild.Render();
		}

		ImGui.End();
	}
}
