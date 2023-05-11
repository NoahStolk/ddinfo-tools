using DevilDaggersInfo.Api.App.Updates;
using DevilDaggersInfo.Core.Versioning;

namespace DevilDaggersInfo.App.Networking.TaskHandlers;

public static class FetchLatestVersion
{
	public static async Task<AppVersion?> HandleAsync(AppVersion appVersion, ToolBuildType toolBuildType)
	{
		GetLatestVersion latestVersion = await AsyncHandler.Client.GetLatestVersion(ToolPublishMethod.SelfContained, toolBuildType);
		if (!AppVersion.TryParse(latestVersion.VersionNumber, out AppVersion? onlineVersion))
			return null;

		return onlineVersion > appVersion ? onlineVersion : null;
	}
}
