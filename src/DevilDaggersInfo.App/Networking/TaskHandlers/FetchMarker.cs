using DevilDaggersInfo.Web.ApiSpec.App;
using DevilDaggersInfo.Web.ApiSpec.App.ProcessMemory;

namespace DevilDaggersInfo.App.Networking.TaskHandlers;

public static class FetchMarker
{
	public static async Task<GetMarker> HandleAsync(AppOperatingSystem appOperatingSystem)
	{
		return await AsyncHandler.Client.GetMarker(appOperatingSystem);
	}
}
