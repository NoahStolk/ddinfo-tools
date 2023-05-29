using DevilDaggersInfo.Api.App.Updates;
using DevilDaggersInfo.Core.Versioning;

namespace DevilDaggersInfo.App.Networking.TaskHandlers;

public static class FetchLatestVersion
{
	public static async Task<AppVersion?> HandleAsync(AppVersion appVersion, ToolBuildType toolBuildType)
	{
		try
		{
			GetLatestVersion latestVersion = await AsyncHandler.Client.GetLatestVersion(ToolPublishMethod.SelfContained, toolBuildType);
			if (!AppVersion.TryParse(latestVersion.VersionNumber, out AppVersion? onlineVersion))
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
