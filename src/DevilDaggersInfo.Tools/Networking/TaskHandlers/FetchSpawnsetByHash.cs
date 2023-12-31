using DevilDaggersInfo.Web.ApiSpec.Tools.Spawnsets;
using System.Net;

namespace DevilDaggersInfo.Tools.Networking.TaskHandlers;

public static class FetchSpawnsetByHash
{
	public static async Task<GetSpawnsetByHash?> HandleAsync(byte[] hash)
	{
		try
		{
			return await AsyncHandler.Client.GetSpawnsetByHash(hash);
		}
		catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
		{
			return null;
		}
	}
}
