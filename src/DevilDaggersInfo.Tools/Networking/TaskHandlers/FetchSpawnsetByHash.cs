using DevilDaggersInfo.Web.ApiSpec.Tools.Spawnsets;
using System.Net;

namespace DevilDaggersInfo.Tools.Networking.TaskHandlers;

public static class FetchSpawnsetByHash
{
	public static async Task<GetSpawnsetByHash> HandleAsync(byte[] hash)
	{
		return await AsyncHandler.Client.GetSpawnsetByHash(hash);
	}
}
