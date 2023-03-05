using DevilDaggersInfo.Api.App.CustomLeaderboards;
using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.Styling;
using Warp.NET.Ui;

namespace DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Components.Leaderboard;

public class LeaderboardScrollArea : ScrollArea
{
	private readonly List<LeaderboardEntry> _leaderboardEntries = new();

	public LeaderboardScrollArea(IBounds bounds)
		: base(bounds, 48, 16, ScrollAreaStyles.Default)
	{
		Depth = 100;
	}

	public void SetContent(List<GetCustomEntry> sortedEntries)
	{
		Clear();

		for (int i = 0; i < sortedEntries.Count; i++)
		{
			GetCustomEntry customEntry = sortedEntries[i];
			LeaderboardEntry leaderboardEntry = new(Bounds.CreateNested(4, i * LeaderboardEntry.Height, 996, LeaderboardEntry.Height), customEntry) { Depth = 101 };
			_leaderboardEntries.Add(leaderboardEntry);

			NestingContext.Add(leaderboardEntry);
		}
	}

	public void Clear()
	{
		foreach (LeaderboardEntry leaderboardEntry in _leaderboardEntries)
			NestingContext.Remove(leaderboardEntry);

		_leaderboardEntries.Clear();
	}

	public override void Render(Vector2i<int> scrollOffset)
	{
		if (StateManager.LeaderboardListState.SelectedCustomLeaderboard == null)
			return;

		base.Render(scrollOffset);
	}
}
