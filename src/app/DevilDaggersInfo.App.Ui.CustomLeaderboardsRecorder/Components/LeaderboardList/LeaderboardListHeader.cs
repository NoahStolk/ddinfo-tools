using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Engine.Text;
using DevilDaggersInfo.App.Engine.Ui;
using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.Base.Components.Styles;
using DevilDaggersInfo.App.Ui.Base.Rendering.Text;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.StateManagement.CustomLeaderboardsRecorder.Actions;
using DevilDaggersInfo.App.Ui.Base.StateManagement.CustomLeaderboardsRecorder.Data;

namespace DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Components.LeaderboardList;

public class LeaderboardListHeader : LeaderboardListRow
{
	public LeaderboardListHeader(IBounds bounds)
		: base(bounds)
	{
		ClickableLabelStyle styleLeft = new(Color.Yellow, Color.Red, TextAlign.Left, FontSize.H12, 4);
		ClickableLabelStyle styleRight = new(Color.Yellow, Color.Red, TextAlign.Right, FontSize.H12, 4);

		int labelDepth = Depth + 100;
		ClickableLabel name = new(bounds.CreateNested(GridName.Index, 0, GridName.Width, bounds.Size.Y), "Name", () => StateManager.Dispatch(new SetSorting(LeaderboardListSorting.Name)), styleLeft) { Depth = labelDepth };
		ClickableLabel author = new(bounds.CreateNested(GridAuthor.Index, 0, GridAuthor.Width, bounds.Size.Y), "Author", () => StateManager.Dispatch(new SetSorting(LeaderboardListSorting.Author)), styleLeft) { Depth = labelDepth };
		ClickableLabel criteria = new(bounds.CreateNested(GridCriteria.Index, 0, GridCriteria.Width, bounds.Size.Y), "Criteria", () => StateManager.Dispatch(new SetSorting(LeaderboardListSorting.Criteria)), styleLeft) { Depth = labelDepth };
		ClickableLabel score = new(bounds.CreateNested(GridScore.Index, 0, GridScore.Width, bounds.Size.Y), "Score", () => StateManager.Dispatch(new SetSorting(LeaderboardListSorting.Score)), styleRight) { Depth = labelDepth };
		ClickableLabel nextDagger = new(bounds.CreateNested(GridNextDagger.Index, 0, GridNextDagger.Width, bounds.Size.Y), "Next dagger", () => StateManager.Dispatch(new SetSorting(LeaderboardListSorting.NextDagger)), styleRight) { Depth = labelDepth };
		ClickableLabel rank = new(bounds.CreateNested(GridRank.Index, 0, GridRank.Width, bounds.Size.Y), "Rank", () => StateManager.Dispatch(new SetSorting(LeaderboardListSorting.Rank)), styleRight) { Depth = labelDepth };
		ClickableLabel players = new(bounds.CreateNested(GridPlayers.Index, 0, GridPlayers.Width, bounds.Size.Y), "Players", () => StateManager.Dispatch(new SetSorting(LeaderboardListSorting.Players)), styleRight) { Depth = labelDepth };
		ClickableLabel worldRecord = new(bounds.CreateNested(GridWorldRecord.Index, 0, GridWorldRecord.Width, bounds.Size.Y), "World record", () => StateManager.Dispatch(new SetSorting(LeaderboardListSorting.WorldRecord)), styleRight) { Depth = labelDepth };

		NestingContext.Add(name);
		NestingContext.Add(author);
		NestingContext.Add(criteria);
		NestingContext.Add(rank);
		NestingContext.Add(players);
		NestingContext.Add(score);
		NestingContext.Add(nextDagger);
		NestingContext.Add(worldRecord);
	}
}
