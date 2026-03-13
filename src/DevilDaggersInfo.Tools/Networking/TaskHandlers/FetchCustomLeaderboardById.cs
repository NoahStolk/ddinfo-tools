using DevilDaggersInfo.Web.ApiSpec.Tools.CustomLeaderboards;

namespace DevilDaggersInfo.Tools.Networking.TaskHandlers;

internal static class FetchCustomLeaderboardById
{
	public static async Task<GetCustomLeaderboard> HandleAsync(int customLeaderboardId)
	{
		return await AsyncHandler.Client.GetCustomLeaderboardById(customLeaderboardId);
	}
}
