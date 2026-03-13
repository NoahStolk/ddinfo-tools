using DevilDaggersInfo.Web.ApiSpec.Tools;
using DevilDaggersInfo.Web.ApiSpec.Tools.ProcessMemory;

namespace DevilDaggersInfo.Tools.Networking.TaskHandlers;

internal static class FetchMarker
{
	public static async Task<GetMarker> HandleAsync(AppOperatingSystem appOperatingSystem)
	{
		return await AsyncHandler.Client.GetMarker(appOperatingSystem);
	}
}
