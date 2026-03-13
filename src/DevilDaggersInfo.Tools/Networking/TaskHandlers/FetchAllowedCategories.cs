using DevilDaggersInfo.Web.ApiSpec.Tools.CustomLeaderboards;

namespace DevilDaggersInfo.Tools.Networking.TaskHandlers;

internal static class FetchAllowedCategories
{
	public static async Task<List<GetCustomLeaderboardAllowedCategory>> HandleAsync()
	{
		return await AsyncHandler.Client.GetCustomLeaderboardAllowedCategories();
	}
}
