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
		AsyncHandler.Run(SetMarker, () => FetchMarker.HandleAsync(Root.PlatformSpecificValues.AppOperatingSystem));

		void SetMarker(ApiResult<GetMarker> getMarker)
		{
			Marker = getMarker.Match<long?>(
				onSuccess: getMarker => getMarker.Value,
				onError: apiError =>
				{
					Root.Log.Error(apiError.Exception, "API error: " + apiError.Message);
					PopupManager.ShowQuestion("Failed to retrieve marker", "Could not fetch marker from the DevilDaggersInfo API. The marker is required in order to scan the game's memory. Certain features may not work if this is cancelled. Do you want to retry?", () => _tryDownloadMarker = true, () => _tryDownloadMarker = false);
					return null;
				});
		}
	}
}
