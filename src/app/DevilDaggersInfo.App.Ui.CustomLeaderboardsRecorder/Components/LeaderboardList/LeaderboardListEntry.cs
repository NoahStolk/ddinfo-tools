using DevilDaggersInfo.Api.App.CustomLeaderboards;
using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.Base.Components.Styles;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.Extensions;
using DevilDaggersInfo.App.Ui.Base.Rendering.Text;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.StateManagement.CustomLeaderboardsRecorder.Actions;
using DevilDaggersInfo.App.Ui.Base.Styling;
using DevilDaggersInfo.Common;
using Silk.NET.GLFW;
using Warp.NET;
using Warp.NET.Text;
using Warp.NET.Ui;

namespace DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Components.LeaderboardList;

public class LeaderboardListEntry : LeaderboardListRow
{
	private readonly GetCustomLeaderboardForOverview _customLeaderboard;

	private bool _isHovering;

	public LeaderboardListEntry(IBounds bounds, GetCustomLeaderboardForOverview customLeaderboard)
		: base(bounds)
	{
		_customLeaderboard = customLeaderboard;

		int iconOffset = 0;
		foreach (GetCustomLeaderboardCriteria criteria in customLeaderboard.Criteria)
		{
			CriteriaIcon icon = new(bounds.CreateNested(GridCriteria.Index + iconOffset, 0, 16, 16), criteria) { Depth = Depth + 1 };
			NestingContext.Add(icon);

			iconOffset += 16;
		}

		LabelStyle scoreStyle = new(customLeaderboard.SelectedPlayerStats?.Dagger?.GetColor() ?? Color.White, TextAlign.Right, FontSize.H12);
		LabelStyle nextDaggerStyle = new(customLeaderboard.SelectedPlayerStats?.NextDagger?.Dagger.GetColor() ?? Color.White, TextAlign.Right, FontSize.H12);
		LabelStyle worldRecordStyle = new(customLeaderboard.WorldRecord?.Dagger?.GetColor() ?? Color.White, TextAlign.Right, FontSize.H12);

		int labelDepth = Depth + 100;
		Label name = new(bounds.CreateNested(GridName.Index, 0, GridName.Width, bounds.Size.Y), customLeaderboard.SpawnsetName, LabelStyles.DefaultLeft) { Depth = labelDepth, RenderOverflow = false };
		Label author = new(bounds.CreateNested(GridAuthor.Index, 0, GridAuthor.Width, bounds.Size.Y), customLeaderboard.SpawnsetAuthorName, LabelStyles.DefaultLeft) { Depth = labelDepth, RenderOverflow = false };
		Label score = new(bounds.CreateNested(GridScore.Index, 0, GridScore.Width, bounds.Size.Y), customLeaderboard.SelectedPlayerStats?.Time.ToString(StringFormats.TimeFormat) ?? "-", scoreStyle) { Depth = labelDepth };
		Label nextDagger = new(bounds.CreateNested(GridNextDagger.Index, 0, GridNextDagger.Width, bounds.Size.Y), customLeaderboard.SelectedPlayerStats?.NextDagger?.Time.ToString(StringFormats.TimeFormat) ?? "-", nextDaggerStyle) { Depth = labelDepth };
		Label rank = new(bounds.CreateNested(GridRank.Index, 0, GridRank.Width, bounds.Size.Y), customLeaderboard.SelectedPlayerStats == null ? "-" : customLeaderboard.SelectedPlayerStats.Rank.ToString(), LabelStyles.DefaultRight) { Depth = labelDepth };
		Label players = new(bounds.CreateNested(GridPlayers.Index, 0, GridPlayers.Width, bounds.Size.Y), customLeaderboard.PlayerCount.ToString(), LabelStyles.DefaultRight) { Depth = labelDepth };
		Label worldRecord = new(bounds.CreateNested(GridWorldRecord.Index, 0, GridWorldRecord.Width, bounds.Size.Y), customLeaderboard.WorldRecord?.Time.ToString(StringFormats.TimeFormat) ?? "-", worldRecordStyle) { Depth = labelDepth };

		NestingContext.Add(name);
		NestingContext.Add(author);
		NestingContext.Add(rank);
		NestingContext.Add(players);
		NestingContext.Add(score);
		NestingContext.Add(nextDagger);
		NestingContext.Add(worldRecord);
	}

	public override void Update(Vector2i<int> scrollOffset)
	{
		base.Update(scrollOffset);

		_isHovering = MouseUiContext.Contains(scrollOffset, Bounds);
		if (!_isHovering || !Input.IsButtonPressed(MouseButton.Left))
			return;

		StateManager.Dispatch(new SetSelectedCustomLeaderboard(_customLeaderboard));
	}

	public override void Render(Vector2i<int> scrollOffset)
	{
		base.Render(scrollOffset);

		if (_isHovering)
			Root.Game.RectangleRenderer.Schedule(Bounds.Size, scrollOffset + Bounds.Center, Depth - 1, GlobalColors.EntrySelect);
	}
}
