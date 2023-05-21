using DevilDaggersInfo.Api.App.CustomLeaderboards;

namespace DevilDaggersInfo.App.Networking.TaskHandlers;

public static class FetchAllowedCategories
{
	public static async Task<List<GetCustomLeaderboardAllowedCategory>> HandleAsync()
	{
		return await AsyncHandler.Client.GetCustomLeaderboardAllowedCategories();
	}
}
