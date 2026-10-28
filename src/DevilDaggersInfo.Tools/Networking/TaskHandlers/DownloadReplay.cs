namespace DevilDaggersInfo.Tools.Networking.TaskHandlers;

internal static class DownloadReplay
{
	public static async Task<LeaderboardReplayResponse> HandleAsync(int id)
	{
		byte[] data = await Clients.DevilDaggers.DownloadReplay(id);
		return new LeaderboardReplayResponse(data, id);
	}

	internal sealed record LeaderboardReplayResponse(byte[] Data, int PlayerId);
}
