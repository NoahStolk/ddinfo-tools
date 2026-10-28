namespace DevilDaggersInfo.Tools.Networking.TaskHandlers;

internal sealed class DownloadReplay
{
	public static async Task<LeaderboardReplayResponse> HandleAsync(int id)
	{
		using FormUrlEncodedContent content = new(new List<KeyValuePair<string, string>> { new("replay", id.ToString()) });
		using HttpClient httpClient = new();
		using HttpResponseMessage response = await httpClient.PostAsync("http://dd.hasmodai.com/dd3/get_replay.php", content);
		if (response.IsSuccessStatusCode)
			return new LeaderboardReplayResponse(await response.Content.ReadAsByteArrayAsync(), id);

		throw new HttpRequestException($"The leaderboard servers returned an unsuccessful response (HTTP {(int)response.StatusCode} {response.StatusCode}).");
	}

	internal sealed record LeaderboardReplayResponse(byte[] Data, int PlayerId);
}
