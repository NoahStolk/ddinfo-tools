using DevilDaggersInfo.Api.App.CustomLeaderboards;

namespace DevilDaggersInfo.App.Ui.Base.Networking.TaskHandlers;

public static class FetchCustomLeaderboards
{
	public static async Task<List<GetCustomLeaderboardForOverview>> HandleAsync(int selectedPlayerId)
	{
		return await AsyncHandler.Client.GetCustomLeaderboards(selectedPlayerId);
	}
}
