using DevilDaggersInfo.Web.ApiSpec.App;

namespace DevilDaggersInfo.App.Platforms;

public interface IPlatformSpecificValues
{
	AppOperatingSystem AppOperatingSystem { get; }

	string DefaultInstallationPath { get; }
}
