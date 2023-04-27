using DevilDaggersInfo.Api.App.CustomLeaderboards;
using DevilDaggersInfo.App.Ui.Base.StateManagement.CustomLeaderboardsRecorder.Data;
using System.Diagnostics;

namespace DevilDaggersInfo.App.Ui.Base.StateManagement.CustomLeaderboardsRecorder.States;

public record LeaderboardListState(
	CustomLeaderboardCategory Category,
	int PageIndex,
	bool IsLoading,
	string SpawnsetFilter,
	string AuthorFilter,
	bool FeaturedOnly,
	IReadOnlyDictionary<LeaderboardListSorting, bool> SortingDirections,
	LeaderboardListSorting Sorting,
	LeaderboardListState.CustomLeaderboard? SelectedCustomLeaderboard,
	IReadOnlyList<GetCustomLeaderboardForOverview> CustomLeaderboards)
{
	public static LeaderboardListState GetDefault()
	{
		return new(
			Category: CustomLeaderboardCategory.Survival,
			PageIndex: 0,
			IsLoading: false,
			SpawnsetFilter: string.Empty,
			AuthorFilter: string.Empty,
			FeaturedOnly: false,
			SortingDirections: Enum.GetValues<LeaderboardListSorting>().ToDictionary(s => s, _ => true),
			Sorting: LeaderboardListSorting.Name,
			SelectedCustomLeaderboard: null,
			CustomLeaderboards: new List<GetCustomLeaderboardForOverview>());
	}

	public int GetTotal()
	{
		return CustomLeaderboards.Count(Predicate);
	}

	public int GetTotalPages()
	{
		return (int)Math.Ceiling(GetTotal() / (float)Constants.CustomLeaderboardsPageSize);
	}

	public int GetMaxPageIndex()
	{
		return Math.Max(0, GetTotalPages() - 1);
	}

	public List<GetCustomLeaderboardForOverview> GetPagedCustomLeaderboards()
	{
		_ = SortingDirections.TryGetValue(Sorting, out bool isAscending);
		IEnumerable<GetCustomLeaderboardForOverview> sorted = Sorting switch
		{
			LeaderboardListSorting.Name => isAscending ? CustomLeaderboards.OrderBy(cl => cl.SpawnsetName.ToLower()) : CustomLeaderboards.OrderByDescending(cl => cl.SpawnsetName.ToLower()),
			LeaderboardListSorting.Author => isAscending ? CustomLeaderboards.OrderBy(cl => cl.SpawnsetAuthorName.ToLower()) : CustomLeaderboards.OrderByDescending(cl => cl.SpawnsetAuthorName.ToLower()),
			LeaderboardListSorting.Criteria => isAscending ? CustomLeaderboards.OrderBy(cl => cl.Criteria.Count) : CustomLeaderboards.OrderByDescending(cl => cl.Criteria.Count),
			LeaderboardListSorting.Score => isAscending ? CustomLeaderboards.OrderBy(cl => cl.SelectedPlayerStats?.Time) : CustomLeaderboards.OrderByDescending(cl => cl.SelectedPlayerStats?.Time),
			LeaderboardListSorting.NextDagger => isAscending ? CustomLeaderboards.OrderBy(GetNextDaggerSortingKey) : CustomLeaderboards.OrderByDescending(GetNextDaggerSortingKey),
			LeaderboardListSorting.Rank => isAscending ? CustomLeaderboards.OrderBy(GetRankSortingKey) : CustomLeaderboards.OrderByDescending(GetRankSortingKey),
			LeaderboardListSorting.Players => isAscending ? CustomLeaderboards.OrderBy(cl => cl.PlayerCount) : CustomLeaderboards.OrderByDescending(cl => cl.PlayerCount),
			LeaderboardListSorting.WorldRecord => isAscending ? CustomLeaderboards.OrderBy(cl => cl.WorldRecord?.Time) : CustomLeaderboards.OrderByDescending(cl => cl.WorldRecord?.Time),
			_ => throw new UnreachableException(),
		};

		return sorted
			.Where(Predicate)
			.Skip(PageIndex * Constants.CustomLeaderboardsPageSize)
			.Take(Constants.CustomLeaderboardsPageSize)
			.ToList();

		static int GetRankSortingKey(GetCustomLeaderboardForOverview customLeaderboard)
		{
			if (customLeaderboard.SelectedPlayerStats == null)
				return int.MaxValue;

			return customLeaderboard.SelectedPlayerStats.Rank;
		}

		static double GetNextDaggerSortingKey(GetCustomLeaderboardForOverview customLeaderboard)
		{
			if (customLeaderboard.Daggers == null || customLeaderboard.SelectedPlayerStats == null)
				return double.MinValue;

			if (customLeaderboard.SelectedPlayerStats.NextDagger == null)
				return double.MaxValue;

			return customLeaderboard.SelectedPlayerStats.NextDagger.Time;
		}
	}

	private bool Predicate(GetCustomLeaderboardForOverview cl)
	{
		return
			cl.Category == Category &&
			(string.IsNullOrEmpty(SpawnsetFilter) || cl.SpawnsetName.Contains(SpawnsetFilter, StringComparison.OrdinalIgnoreCase)) &&
			(string.IsNullOrEmpty(AuthorFilter) || cl.SpawnsetAuthorName.Contains(AuthorFilter, StringComparison.OrdinalIgnoreCase)) &&
			(!FeaturedOnly || cl.Daggers != null);
	}

	public record CustomLeaderboard(int Id, int SpawnsetId, string SpawnsetName);
}
