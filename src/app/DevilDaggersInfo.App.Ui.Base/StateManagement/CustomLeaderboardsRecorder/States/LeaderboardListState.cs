using DevilDaggersInfo.Api.App.CustomLeaderboards;
using DevilDaggersInfo.App.Ui.Base.StateManagement.CustomLeaderboardsRecorder.Data;
using DevilDaggersInfo.Types.Web;
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
		IEnumerable<GetCustomLeaderboardForOverview> sorted = Sorting switch
		{
			LeaderboardListSorting.Name => Sort(CustomLeaderboards, cl => cl.SpawnsetName),
			LeaderboardListSorting.Author => Sort(CustomLeaderboards, cl => cl.SpawnsetAuthorName),
			LeaderboardListSorting.Criteria => Sort(CustomLeaderboards, cl => cl.Criteria.Count),
			LeaderboardListSorting.Score => Sort(CustomLeaderboards, cl => cl.SelectedPlayerStats?.Time),
			LeaderboardListSorting.NextDagger => Sort(CustomLeaderboards, cl => cl.SelectedPlayerStats?.NextDagger?.Time),
			LeaderboardListSorting.Rank => Sort(CustomLeaderboards, cl => cl.SelectedPlayerStats?.Rank),
			LeaderboardListSorting.Players => Sort(CustomLeaderboards, cl => cl.PlayerCount),
			LeaderboardListSorting.WorldRecord => Sort(CustomLeaderboards, cl => cl.WorldRecord?.Time),
			_ => throw new UnreachableException(),
		};

		return sorted
			.Where(Predicate)
			.Skip(PageIndex * Constants.CustomLeaderboardsPageSize)
			.Take(Constants.CustomLeaderboardsPageSize)
			.ToList();

		IEnumerable<GetCustomLeaderboardForOverview> Sort(IEnumerable<GetCustomLeaderboardForOverview> customLeaderboards, Func<GetCustomLeaderboardForOverview, object?> selector)
		{
			_ = SortingDirections.TryGetValue(Sorting, out bool isAscending);
			return isAscending
				? customLeaderboards.OrderBy(selector)
				: customLeaderboards.OrderByDescending(selector);
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
