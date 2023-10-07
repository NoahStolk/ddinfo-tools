using DevilDaggersInfo.Web.ApiSpec.App;

namespace DevilDaggersInfo.Tools.Platforms;

public interface IPlatformSpecificValues
{
	AppOperatingSystem AppOperatingSystem { get; }

	string DefaultInstallationPath { get; }
}
