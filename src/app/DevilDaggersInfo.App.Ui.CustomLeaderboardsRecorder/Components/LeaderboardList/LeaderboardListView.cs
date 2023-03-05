using DevilDaggersInfo.Api.App.CustomLeaderboards;
using DevilDaggersInfo.App.Ui.Base;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using Warp.NET.Text;
using Warp.NET.Ui;
using Warp.NET.Ui.Components;

namespace DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Components.LeaderboardList;

public class LeaderboardListView : AbstractComponent
{
	private const int _rowHeight = 16;
	private const int _borderSize = 1;

	private readonly List<LeaderboardListEntry> _leaderboardListEntries = new();

	public LeaderboardListView(IBounds bounds)
		: base(bounds)
	{
		LeaderboardListHeader header = new(bounds.CreateNested(_borderSize, 16, bounds.Size.X - _borderSize * 2, _rowHeight));
		NestingContext.Add(header);

		int y = 32;
		for (int i = 0; i < Constants.CustomLeaderboardsPageSize; i++)
		{
			LeaderboardListEntry leaderboardListEntry = new(Bounds.CreateNested(_borderSize, y, Bounds.Size.X - _borderSize * 2, _rowHeight)) { Depth = Depth + 3 };
			_leaderboardListEntries.Add(leaderboardListEntry);
			NestingContext.Add(leaderboardListEntry);

			y += _rowHeight;
		}
	}

	public void Set()
	{
		List<GetCustomLeaderboardForOverview> pagedCustomLeaderboards = StateManager.LeaderboardListState.GetPagedCustomLeaderboards();
		for (int i = 0; i < Constants.CustomLeaderboardsPageSize; i++)
			_leaderboardListEntries[i].SetCustomLeaderboard(i < pagedCustomLeaderboards.Count ? pagedCustomLeaderboards[i] : null);
	}

	public override void Render(Vector2i<int> scrollOffset)
	{
		base.Render(scrollOffset);

		string text;
		Color color;
		if (StateManager.LeaderboardListState.IsLoading)
		{
			text = "Loading...";
			color = Color.Yellow;
		}
		else
		{
			int total = StateManager.LeaderboardListState.GetTotal();
			int totalPages = StateManager.LeaderboardListState.GetTotalPages();

			int page = StateManager.LeaderboardListState.PageIndex + 1;
			int start = StateManager.LeaderboardListState.PageIndex * Constants.CustomLeaderboardsPageSize + 1;
			int end = Math.Min(total, (StateManager.LeaderboardListState.PageIndex + 1) * Constants.CustomLeaderboardsPageSize);
			text = $"Page {page} of {totalPages} ({start} - {end} of {total})";
			color = Color.White;
		}

		Root.Game.MonoSpaceFontRenderer12.Schedule(new(1), scrollOffset + Bounds.TopLeft + new Vector2i<int>(4, 0), Depth + 2, color, text, TextAlign.Left);
	}
}
