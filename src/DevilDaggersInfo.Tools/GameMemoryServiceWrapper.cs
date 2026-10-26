using DevilDaggersInfo.Tools.GameMemory;
using DevilDaggersInfo.Tools.Networking;
using DevilDaggersInfo.Tools.Networking.TaskHandlers;
using DevilDaggersInfo.Tools.Platforms;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Web.ApiSpec.Tools;
using DevilDaggersInfo.Web.ApiSpec.Tools.ProcessMemory;
using Serilog;
using System.Collections.Concurrent;

namespace DevilDaggersInfo.Tools;

internal sealed class GameMemoryServiceWrapper(
	PopupManager popupManager,
	GameMemoryService gameMemoryService,
	IPlatformSpecificValues platformSpecificValues,
	ILogger logger)
{
	// Written from the HTTP callback thread.
	private readonly ConcurrentDictionary<AppOperatingSystem, long> _markers = new();

	private bool _tryDownloadMarker = true;

	public long? Marker { get; private set; }

	/// <summary>
	/// Scans game memory. If the marker is not known, fires the call to retrieve it, then returns false because memory can't be scanned until the HTTP call has returned successfully.
	/// </summary>
	/// <returns>Whether the marker is known.</returns>
	public bool Scan()
	{
		// Always find the process, so we detach properly when the game exits.
		gameMemoryService.FindProcess();

		// The marker depends on the build of the game rather than the OS the app runs on, since the Windows build can run on Linux through Proton.
		// Without a process, fall back to the app's OS so the marker is usually known by the time the game is started.
		AppOperatingSystem markerOperatingSystem = gameMemoryService.GameOperatingSystem ?? platformSpecificValues.AppOperatingSystem;
		if (!_markers.TryGetValue(markerOperatingSystem, out long marker))
		{
			Marker = null;
			if (_tryDownloadMarker)
				InitializeMarker(markerOperatingSystem);

			return false;
		}

		Marker = marker;
		gameMemoryService.Initialize(marker);
		gameMemoryService.Scan();

		return true;
	}

	private void InitializeMarker(AppOperatingSystem appOperatingSystem)
	{
		// Workaround to prevent multiple popups from appearing during continuous retry.
		if (popupManager.IsAnyOpen)
			return;

		AsyncHandler.Run(SetMarker, () => FetchMarker.HandleAsync(appOperatingSystem));

		void SetMarker(ApiResult<GetMarker> getMarkerResult)
		{
			getMarkerResult.Match(
				onSuccess: getMarker => _markers[appOperatingSystem] = getMarker.Value,
				onError: apiError =>
				{
					logger.Error(apiError.Exception, "API error: {Message}", apiError.Message);

					const string message = """
						Could not fetch marker from the DevilDaggers.info API.

						The marker is required in order to scan the game's memory. This means that:
						- Custom leaderboards won't work.
						- Some features in the replay editor won't work.
						- Practice will still work, but you won't be able to see your stats.

						Do you want to retry?

						If you select "No", you will have to restart the app to try again.
						""";

					popupManager.ShowQuestion("Failed to retrieve marker", message, () => _tryDownloadMarker = true, () => _tryDownloadMarker = false);
				});
		}
	}
}
