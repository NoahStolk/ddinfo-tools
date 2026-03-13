using System.Net;

namespace DevilDaggersInfo.Tools.Networking.TaskHandlers;

internal static class CheckIfLeaderboardExists
{
	public static async Task<Result> HandleAsync(byte[] survivalHash)
	{
		HttpResponseMessage hrm = await AsyncHandler.Client.CustomLeaderboardExistsBySpawnsetHash(survivalHash);
		return new Result(hrm.StatusCode != HttpStatusCode.NotFound);
	}

	internal sealed record Result(bool Exists);
}
