using DevilDaggersInfo.Api.App.ProcessMemory;

namespace DevilDaggersInfo.App.Networking.TaskHandlers;

public static class FetchMarker
{
	public static async Task<GetMarker> HandleAsync(SupportedOperatingSystem supportedOperatingSystem)
	{
		return await AsyncHandler.Client.GetMarker(supportedOperatingSystem);
	}
}
