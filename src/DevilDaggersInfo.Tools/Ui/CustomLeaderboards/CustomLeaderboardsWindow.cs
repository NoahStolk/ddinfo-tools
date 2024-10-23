using DevilDaggersInfo.Tools.Ui.CustomLeaderboards.Leaderboard;
using DevilDaggersInfo.Tools.Ui.CustomLeaderboards.LeaderboardList;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.CustomLeaderboards;

public sealed class CustomLeaderboardsWindow
{
	private readonly LeaderboardListChild _leaderboardListChild;
	private readonly LeaderboardListViewChild _leaderboardListViewChild;
	private readonly CustomLeaderboards3DWindow _customLeaderboards3DWindow;

	private float _recordingTimer;

	public CustomLeaderboardsWindow(LeaderboardListChild leaderboardListChild, LeaderboardListViewChild leaderboardListViewChild, CustomLeaderboards3DWindow customLeaderboards3DWindow)
	{
		_leaderboardListChild = leaderboardListChild;
		_leaderboardListViewChild = leaderboardListViewChild;
		_customLeaderboards3DWindow = customLeaderboards3DWindow;
	}

	public void Update(float delta)
	{
		_customLeaderboards3DWindow.Update(delta);
		RecordingChild.Update(delta);

		_recordingTimer += delta;
		if (_recordingTimer < 0.12f)
			return;

		_recordingTimer = 0;
		if (!GameMemoryServiceWrapper.Scan())
			return;

		RecordingLogic.Handle();
	}

	public void Render()
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
				_leaderboardListChild.Render();
				_leaderboardListViewChild.Render();
			}

			ImGui.EndChild();

			LeaderboardChild.Render();
		}

		ImGui.End();
	}
}
