using DevilDaggersInfo.Web.ApiSpec.Tools;

namespace DevilDaggersInfo.Tools.Platforms;

public interface IPlatformSpecificValues
{
	AppOperatingSystem AppOperatingSystem { get; }

	string DefaultInstallationPath { get; }
}
