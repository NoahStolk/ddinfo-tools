using System.Net;

namespace DevilDaggersInfo.Tools.Networking.TaskHandlers;

internal static class CheckIfLeaderboardExists
{
	public static async Task<Result> HandleAsync(byte[] survivalHash)
	{
		HttpResponseMessage hrm = await Clients.DevilDaggersInfo.CustomLeaderboardExistsBySpawnsetHash(survivalHash);
		return new Result(hrm.StatusCode != HttpStatusCode.NotFound);
	}

	// Handler must return a reference type.
	internal sealed record Result(bool Exists);
}
