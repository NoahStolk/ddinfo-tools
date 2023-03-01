using DevilDaggersInfo.Api.App;
using DevilDaggersInfo.Api.App.CustomLeaderboards;
using DevilDaggersInfo.Types.Web;

namespace DevilDaggersInfo.App.Ui.Base.Networking.TaskHandlers;

public static class FetchCustomLeaderboards
{
	public static async Task<Page<GetCustomLeaderboardForOverview>> HandleAsync(CustomLeaderboardCategory category, int pageIndex, int pageSize, int selectedPlayerId, bool onlyFeatured)
	{
		return await AsyncHandler.Client.GetCustomLeaderboardOverview(category, pageIndex, pageSize, selectedPlayerId, onlyFeatured);
	}
}
