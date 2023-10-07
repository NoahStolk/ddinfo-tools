using DevilDaggersInfo.Web.ApiSpec.App;

namespace DevilDaggersInfo.App.Platforms;

public class LinuxValues : IPlatformSpecificValues
{
	public AppOperatingSystem AppOperatingSystem => AppOperatingSystem.Linux;

	public string DefaultInstallationPath => string.Empty;
}
