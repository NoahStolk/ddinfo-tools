using DevilDaggersInfo.App.Networking;
using DevilDaggersInfo.App.Ui.ReplayEditor.State;
using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Spawnset;
using ImGuiNET;

namespace DevilDaggersInfo.App.Ui.ReplayEditor;

public static class LeaderboardReplayBrowser
{
	private static bool _showWindow;
	private static bool _isDownloading;
	private static int _selectedPlayerId;

	public static void Show()
	{
		_showWindow = true;
	}

	public static void Render()
	{
		if (!_showWindow)
			return;

		ImGui.SetNextWindowSize(new(256, 128));
		if (ImGui.Begin("Leaderboard Replay Browser", ref _showWindow, ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse))
		{
			ImGui.InputInt("Player ID", ref _selectedPlayerId, 1, 1, ImGuiInputTextFlags.CharsDecimal);

			ImGui.BeginDisabled(_isDownloading);
			if (ImGui.Button("Download and open"))
			{
				_isDownloading = true;
				AsyncHandler.Run(HandleDownloadedReplay, () => DownloadReplayAsync(_selectedPlayerId));
			}

			ImGui.EndDisabled();

			if (_isDownloading)
				ImGui.Text("Downloading...");
		}

		ImGui.End(); // Leaderboard Replay Browser
	}

	private static void HandleDownloadedReplay(Response? response)
	{
		if (response == null)
		{
			Modals.ShowError("The Devil Daggers leaderboard servers did not return a successful response.");
			_isDownloading = false;
			return;
		}

		ReplayBinary<LeaderboardReplayBinaryHeader>? leaderboardReplay;

		try
		{
			leaderboardReplay = new(response.Data);
		}
		catch (Exception ex)
		{
			Root.Log.Warning(ex, "The replay could not be parsed.");
			Modals.ShowError("The replay could not be parsed.");
			_isDownloading = false;
			return;
		}

		LocalReplayBinaryHeader header = new(
			version: 1,
			timestampSinceGameRelease: 0,
			time: 0,
			startTime: 0,
			daggersFired: 0,
			deathType: 0,
			gems: 0,
			daggersHit: 0,
			kills: 0,
			playerId: response.PlayerId,
			username: leaderboardReplay.Header.Username,
			unknown: new byte[10],
			spawnsetBuffer: SpawnsetBinary.CreateDefault().ToBytes());
		ReplayState.Replay = new(header, leaderboardReplay.EventsData);

		_isDownloading = false;
		_showWindow = false;
	}

	private static async Task<Response?> DownloadReplayAsync(int id)
	{
		using FormUrlEncodedContent content = new(new List<KeyValuePair<string, string>> { new("replay", id.ToString()) });
		using HttpClient httpClient = new();
		using HttpResponseMessage response = await httpClient.PostAsync("http://dd.hasmodai.com/backend16/get_replay.php", content);
		if (response.IsSuccessStatusCode)
			return new(await response.Content.ReadAsByteArrayAsync(), id);

		Root.Log.Warning($"The leaderboard servers returned an unsuccessful response (HTTP {(int)response.StatusCode} {response.StatusCode}).");
		return null;
	}

	private sealed record Response(byte[] Data, int PlayerId);
}
