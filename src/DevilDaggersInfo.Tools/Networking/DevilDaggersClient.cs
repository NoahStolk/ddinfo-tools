namespace DevilDaggersInfo.Tools.Networking;

internal sealed class DevilDaggersClient(HttpClient client)
{
	public async Task<byte[]> DownloadReplay(int id)
	{
		using FormUrlEncodedContent content = new(new List<KeyValuePair<string, string>> { new("replay", id.ToString()) });
		using HttpResponseMessage response = await client.SendRequest(HttpMethod.Post, "dd3/get_replay.php", content);
		if (response.IsSuccessStatusCode)
			return await response.Content.ReadAsByteArrayAsync();

		throw new HttpRequestException($"The leaderboard servers returned an unsuccessful response (HTTP {(int)response.StatusCode} {response.StatusCode}).");
	}
}
