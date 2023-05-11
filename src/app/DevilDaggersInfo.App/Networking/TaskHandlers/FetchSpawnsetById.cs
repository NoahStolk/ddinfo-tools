using DevilDaggersInfo.Api.App.Spawnsets;

namespace DevilDaggersInfo.App.Networking.TaskHandlers;

public static class FetchSpawnsetById
{
	public static async Task<GetSpawnset> HandleAsync(int spawnsetId)
	{
		return await AsyncHandler.Client.GetSpawnsetById(spawnsetId);
	}
}
