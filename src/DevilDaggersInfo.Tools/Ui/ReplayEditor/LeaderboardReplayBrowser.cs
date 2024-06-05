using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Networking;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor;

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

		ImGui.SetNextWindowSize(new Vector2(256, 128));
		if (ImGui.Begin("Leaderboard Replay Browser", ref _showWindow, ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoDocking))
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

	private static void HandleDownloadedReplay(ApiResult<Response> responseResult)
	{
		responseResult.Match(
			onSuccess: response =>
			{
				ReplayBinary<LeaderboardReplayBinaryHeader>? leaderboardReplay;

				try
				{
					leaderboardReplay = new ReplayBinary<LeaderboardReplayBinaryHeader>(response.Data);
				}
				catch (Exception ex)
				{
					// When using an id like -1, this gives us the following data:
					// DF_RPL0Replay not found.
					// We could parse this, but it's not worth the effort.
					Root.Log.Warning(ex, "The replay could not be parsed.");
					PopupManager.ShowError("The replay could not be parsed.", ex);
					_isDownloading = false;
					return;
				}

				FileStates.Replay.Update(EditorReplayModel.CreateFromLeaderboardReplay(response.PlayerId, leaderboardReplay.Header.Username, leaderboardReplay.Events));

				_isDownloading = false;
				_showWindow = false;
			},
			onError: apiError =>
			{
				Root.Log.Warning(apiError.Message);
				PopupManager.ShowError("The Devil Daggers leaderboard servers did not return a successful response.", apiError);
				_isDownloading = false;
			});
	}

	private static async Task<Response> DownloadReplayAsync(int id)
	{
		using FormUrlEncodedContent content = new(new List<KeyValuePair<string, string>> { new("replay", id.ToString()) });
		using HttpClient httpClient = new();
		using HttpResponseMessage response = await httpClient.PostAsync("http://dd.hasmodai.com/backend16/get_replay.php", content);
		if (response.IsSuccessStatusCode)
			return new Response(await response.Content.ReadAsByteArrayAsync(), id);

		throw new HttpRequestException($"The leaderboard servers returned an unsuccessful response (HTTP {(int)response.StatusCode} {response.StatusCode}).");
	}

	private sealed record Response(byte[] Data, int PlayerId);
}
