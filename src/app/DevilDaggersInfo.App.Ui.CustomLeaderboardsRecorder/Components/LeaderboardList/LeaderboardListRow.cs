using Warp.NET.Ui;
using Warp.NET.Ui.Components;

namespace DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Components.LeaderboardList;

public abstract class LeaderboardListRow : AbstractComponent
{
	protected LeaderboardListRow(IBounds bounds)
		: base(bounds)
	{
		int fullWidth = bounds.Size.X;
		int columnWidth = fullWidth / 16;

		int index = 0;
		GridName = CreateColumn(ref index, columnWidth * 4);
		GridAuthor = CreateColumn(ref index, columnWidth * 2);
		GridCriteria = CreateColumn(ref index, columnWidth * 2);
		GridScore = CreateColumn(ref index, columnWidth * 2);
		GridNextDagger = CreateColumn(ref index, columnWidth * 2);
		GridRank = CreateColumn(ref index, columnWidth * 1);
		GridPlayers = CreateColumn(ref index, columnWidth * 1);
		GridWorldRecord = CreateColumn(ref index, columnWidth * 2);

		static Column CreateColumn(ref int index, int width)
		{
			Column column = new(index, width);
			index += width;
			return column;
		}
	}

	protected Column GridName { get; }
	protected Column GridAuthor { get; }
	protected Column GridCriteria { get; }
	protected Column GridScore { get; }
	protected Column GridNextDagger { get; }
	protected Column GridRank { get; }
	protected Column GridPlayers { get; }
	protected Column GridWorldRecord { get; }

	protected struct Column
	{
		public Column(int index, int width)
		{
			Index = index;
			Width = width;
		}

		public int Index { get; }
		public int Width { get; }
	}
}
