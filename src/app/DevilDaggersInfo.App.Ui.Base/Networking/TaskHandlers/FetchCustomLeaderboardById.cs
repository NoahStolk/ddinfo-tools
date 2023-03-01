using DevilDaggersInfo.Api.App.CustomLeaderboards;

namespace DevilDaggersInfo.App.Ui.Base.Networking.TaskHandlers;

public static class FetchCustomLeaderboardById
{
	public static async Task<GetCustomLeaderboard> HandleAsync(int customLeaderboardId)
	{
		return await AsyncHandler.Client.GetCustomLeaderboardById(customLeaderboardId);
	}
}
