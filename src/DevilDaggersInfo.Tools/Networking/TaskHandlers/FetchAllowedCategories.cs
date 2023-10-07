using DevilDaggersInfo.Web.ApiSpec.App.CustomLeaderboards;

namespace DevilDaggersInfo.Tools.Networking.TaskHandlers;

public static class FetchAllowedCategories
{
	public static async Task<List<GetCustomLeaderboardAllowedCategory>> HandleAsync()
	{
		return await AsyncHandler.Client.GetCustomLeaderboardAllowedCategories();
	}
}
