using DevilDaggersInfo.Web.ApiSpec.Tools.Spawnsets;

namespace DevilDaggersInfo.Tools.Networking.TaskHandlers;

public static class FetchSpawnsetById
{
	public static async Task<GetSpawnset> HandleAsync(int spawnsetId)
	{
		return await AsyncHandler.Client.GetSpawnsetById(spawnsetId);
	}
}
