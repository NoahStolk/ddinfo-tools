using DevilDaggersInfo.Tools.Networking;
using DevilDaggersInfo.Tools.Networking.TaskHandlers;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Web.ApiSpec.Tools.ProcessMemory;

namespace DevilDaggersInfo.Tools;

public static class GameMemoryServiceWrapper
{
	private static bool _tryDownloadMarker = true;

	public static long? Marker { get; private set; }

	/// <summary>
	/// Scans game memory. If the marker is not known, fires the call to retrieve it, then returns false because memory can't be scanned until the HTTP call has returned successfully.
	/// </summary>
	/// <returns>Whether the marker is known.</returns>
	public static bool Scan()
	{
		if (!Marker.HasValue)
		{
			if (!_tryDownloadMarker)
				return false;

			InitializeMarker();
			return false;
		}

		// Always initialize the process, so we detach properly when the game exits.
		Root.GameMemoryService.Initialize(Marker.Value);
		Root.GameMemoryService.Scan();

		return true;
	}

	private static void InitializeMarker()
	{
		// Workaround to prevent multiple popups from appearing during continuous retry.
		if (PopupManager.IsAnyOpen)
			return;

		AsyncHandler.Run(SetMarker, () => FetchMarker.HandleAsync(Root.PlatformSpecificValues.AppOperatingSystem));

		void SetMarker(ApiResult<GetMarker> getMarkerResult)
		{
			getMarkerResult.Match(
				onSuccess: getMarker => Marker = getMarker.Value,
				onError: apiError =>
				{
					Root.Log.Error(apiError.Exception, "API error: " + apiError.Message);

					const string message = """
						Could not fetch marker from the DevilDaggers.info API.

						The marker is required in order to scan the game's memory. This means that:
						- Custom leaderboards won't work.
						- Some features in the replay editor won't work.
						- Practice will still work, but you won't be able to see your stats.

						Do you want to retry?

						If you select "No", you will have to restart the app to try again.
						""";

					PopupManager.ShowQuestion("Failed to retrieve marker", message, () => _tryDownloadMarker = true, () => _tryDownloadMarker = false);
				});
		}
	}
}
