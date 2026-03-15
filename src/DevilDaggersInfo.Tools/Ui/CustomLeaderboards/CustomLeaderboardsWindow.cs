using DevilDaggersInfo.Tools.Ui.CustomLeaderboards.Leaderboard;
using DevilDaggersInfo.Tools.Ui.CustomLeaderboards.LeaderboardList;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.CustomLeaderboards;

internal sealed class CustomLeaderboardsWindow(
	LeaderboardListChild leaderboardListChild,
	LeaderboardListViewChild leaderboardListViewChild,
	CustomLeaderboards3DWindow customLeaderboards3DWindow,
	RecordingChild recordingChild,
	LeaderboardChild leaderboardChild,
	RecordingLogic recordingLogic,
	StateChild stateChild,
	GameMemoryServiceWrapper gameMemoryServiceWrapper)
{
	private float _recordingTimer;

	public void Update(float delta)
	{
		customLeaderboards3DWindow.Update(delta);
		recordingChild.Update(delta);

		_recordingTimer += delta;
		if (_recordingTimer < 0.12f)
			return;

		_recordingTimer = 0;
		if (!gameMemoryServiceWrapper.Scan())
			return;

		recordingLogic.Handle();
	}

	public void Render()
	{
#if DEBUG
		if (ImGui.Begin("Timestamps"))
		{
			foreach (DevilDaggersInfo.Web.ApiSpec.Tools.CustomLeaderboards.AddUploadRequestTimestamp timestamp in recordingLogic.Timestamps)
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
				stateChild.Render();
				recordingChild.Render();
			}

			ImGui.EndChild();

			ImGui.SameLine();

			if (ImGui.BeginChild("RightRow", new Vector2(0, 464)))
			{
				leaderboardListChild.Render();
				leaderboardListViewChild.Render();
			}

			ImGui.EndChild();

			leaderboardChild.Render();
		}

		ImGui.End();
	}
}
