using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Exceptions;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Networking;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using Hexa.NET.ImGui;
using Serilog;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor;

internal sealed class LeaderboardReplayBrowser(PopupManager popupManager, FileStates fileStates, ILogger logger)
{
	private bool _showWindow;
	private bool _isDownloading;
	private int _selectedPlayerId;

	/// <summary>
	/// The response body returned by the leaderboard servers when there is no replay for the requested player ID (for example, when using an ID like -1).
	/// </summary>
	private static ReadOnlySpan<byte> ReplayNotFoundResponse => "DF_RPL0Replay not found."u8;

	public void Show()
	{
		_showWindow = true;
	}

	public void Render()
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

	private void HandleDownloadedReplay(ApiResult<Response> responseResult)
	{
		responseResult.Match(
			onSuccess: response =>
			{
				if (response.Data.AsSpan().StartsWith(ReplayNotFoundResponse))
				{
					logger.Warning("No replay was found for player ID {PlayerId}.", response.PlayerId);
					popupManager.ShowError($"No replay was found for player ID {response.PlayerId}.");
					_isDownloading = false;
					return;
				}

				ReplayBinary<LeaderboardReplayBinaryHeader>? leaderboardReplay;

				try
				{
					leaderboardReplay = new ReplayBinary<LeaderboardReplayBinaryHeader>(response.Data);
				}
				catch (Exception ex)
				{
					string diagnostics = GetReplayDiagnostics(response.Data);
					logger.Warning(ex, "The replay for player ID {PlayerId} could not be parsed.\n{Diagnostics}", response.PlayerId, diagnostics);
					popupManager.ShowError(
						$"""
						The replay for player ID {response.PlayerId} could not be parsed.
						{diagnostics}
						""",
						ex);
					_isDownloading = false;
					return;
				}

				fileStates.Replay.Update(EditorReplayModel.CreateFromLeaderboardReplay(response.PlayerId, leaderboardReplay.Header.Username, leaderboardReplay.Events));

				_isDownloading = false;
				_showWindow = false;
			},
			onError: apiError =>
			{
				logger.Warning("The Devil Daggers leaderboard servers did not return a successful response: {Message}", apiError.Message ?? "(empty)");
				popupManager.ShowError("The Devil Daggers leaderboard servers did not return a successful response.", apiError);
				_isDownloading = false;
			});
	}

	private static string GetReplayDiagnostics(byte[] data)
	{
		StringBuilder sb = new();
		sb.Append(CultureInfo.InvariantCulture, $"- Total size: {data.Length} bytes");

		try
		{
			using MemoryStream ms = new(data);
			using BinaryReader br = new(ms);
			LeaderboardReplayBinaryHeader header = LeaderboardReplayBinaryHeader.CreateFromBinaryReader(br);

			// BinaryReader.ReadBytes does not throw when the stream ends early, so a truncated header can still "parse" with a shortened username or buffer.
			sb.AppendLine();
			sb.Append(CultureInfo.InvariantCulture, $"- Username: {header.Username}");
			sb.AppendLine();
			sb.Append(CultureInfo.InvariantCulture, $"- Unknown buffer ({header.UnknownBuffer.Length} bytes): {Convert.ToHexString(header.UnknownBuffer)}");
			sb.AppendLine();
			sb.Append(CultureInfo.InvariantCulture, $"- Header size: {ms.Position} bytes");
			sb.AppendLine();
			sb.Append(CultureInfo.InvariantCulture, $"- Compressed events size: {data.Length - ms.Position} bytes");
		}
		catch (Exception ex) when (ex is EndOfStreamException or InvalidReplayBinaryException or ArgumentOutOfRangeException)
		{
			sb.AppendLine();
			sb.Append(CultureInfo.InvariantCulture, $"- Header could not be parsed: {ex.Message}");
		}

		return sb.ToString();
	}

	private static async Task<Response> DownloadReplayAsync(int id)
	{
		using FormUrlEncodedContent content = new(new List<KeyValuePair<string, string>> { new("replay", id.ToString()) });
		using HttpClient httpClient = new();
		using HttpResponseMessage response = await httpClient.PostAsync("http://dd.hasmodai.com/dd3/get_replay.php", content);
		if (response.IsSuccessStatusCode)
			return new Response(await response.Content.ReadAsByteArrayAsync(), id);

		throw new HttpRequestException($"The leaderboard servers returned an unsuccessful response (HTTP {(int)response.StatusCode} {response.StatusCode}).");
	}

	private sealed record Response(byte[] Data, int PlayerId);
}
