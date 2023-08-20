using DevilDaggersInfo.Api.App;
using DevilDaggersInfo.Api.App.Updates;

namespace DevilDaggersInfo.App.Networking.TaskHandlers;

public static class FetchLatestVersion
{
	public static async Task<Version?> HandleAsync(Version appVersion, AppOperatingSystem appOperatingSystem)
	{
		try
		{
			GetLatestVersion latestVersion = await AsyncHandler.Client.GetLatest(appOperatingSystem);
			if (!Version.TryParse(latestVersion.VersionNumber, out Version? onlineVersion))
				return null;

			return onlineVersion > appVersion ? onlineVersion : null;
		}
		catch (Exception ex)
		{
			Root.Log.Error(ex, "Could not fetch latest version.");

			// We still want to be able to run the app in case the API is down.
			return null;
		}
	}
}
