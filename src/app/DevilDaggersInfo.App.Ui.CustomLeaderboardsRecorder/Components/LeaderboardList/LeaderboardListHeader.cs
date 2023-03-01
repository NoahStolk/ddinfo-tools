using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.Base.Styling;
using Warp.NET.Ui;

namespace DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Components.LeaderboardList;

public class LeaderboardListHeader : LeaderboardListRow
{
	public LeaderboardListHeader(IBounds bounds)
		: base(bounds)
	{
		int labelDepth = Depth + 100;
		Label name = new(bounds.CreateNested(GridName.Index, 0, GridName.Width, bounds.Size.Y), "Name", LabelStyles.DefaultLeft) { Depth = labelDepth };
		Label author = new(bounds.CreateNested(GridAuthor.Index, 0, GridAuthor.Width, bounds.Size.Y), "Author", LabelStyles.DefaultLeft) { Depth = labelDepth };
		Label criteria = new(bounds.CreateNested(GridCriteria.Index, 0, GridCriteria.Width, bounds.Size.Y), "Criteria", LabelStyles.DefaultLeft) { Depth = labelDepth };
		Label score = new(bounds.CreateNested(GridScore.Index, 0, GridScore.Width, bounds.Size.Y), "Score", LabelStyles.DefaultRight) { Depth = labelDepth };
		Label nextDagger = new(bounds.CreateNested(GridNextDagger.Index, 0, GridNextDagger.Width, bounds.Size.Y), "Next dagger", LabelStyles.DefaultRight) { Depth = labelDepth };
		Label rank = new(bounds.CreateNested(GridRank.Index, 0, GridRank.Width, bounds.Size.Y), "Rank", LabelStyles.DefaultRight) { Depth = labelDepth };
		Label players = new(bounds.CreateNested(GridPlayers.Index, 0, GridPlayers.Width, bounds.Size.Y), "Players", LabelStyles.DefaultRight) { Depth = labelDepth };
		Label worldRecord = new(bounds.CreateNested(GridWorldRecord.Index, 0, GridWorldRecord.Width, bounds.Size.Y), "World record", LabelStyles.DefaultRight) { Depth = labelDepth };

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
