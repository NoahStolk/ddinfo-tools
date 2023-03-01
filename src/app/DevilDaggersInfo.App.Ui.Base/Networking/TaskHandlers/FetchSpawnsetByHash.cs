using DevilDaggersInfo.Api.App.Spawnsets;
using System.Net;

namespace DevilDaggersInfo.App.Ui.Base.Networking.TaskHandlers;

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
