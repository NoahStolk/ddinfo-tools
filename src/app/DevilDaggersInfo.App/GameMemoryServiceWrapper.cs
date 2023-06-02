using DevilDaggersInfo.Api.App.ProcessMemory;
using DevilDaggersInfo.App.Networking;
using DevilDaggersInfo.App.Networking.TaskHandlers;
using DevilDaggersInfo.App.Ui;

namespace DevilDaggersInfo.App;

public static class GameMemoryServiceWrapper
{
	public static long? Marker { get; private set; }

	/// <summary>
	/// Scans game memory. If the marker is not known, fires the call to retrieve it, then returns false because memory can't be scanned until the HTTP call has returned successfully.
	/// </summary>
	/// <returns>Whether the marker is known.</returns>
	public static bool Scan()
	{
		if (!Marker.HasValue)
		{
			InitializeMarker();
			return false;
		}

		// Always initialize the process so we detach properly when the game exits.
		Root.GameMemoryService.Initialize(Marker.Value);
		Root.GameMemoryService.Scan();

		return true;
	}

	private static void InitializeMarker()
	{
		AsyncHandler.Run(SetMarker, () => FetchMarker.HandleAsync(Root.PlatformSpecificValues.OperatingSystem));

		void SetMarker(GetMarker? getMarker)
		{
			if (getMarker == null)
				Modals.ShowError("Failed to retrieve marker.");
			else
				Marker = getMarker.Value;
		}
	}
}
