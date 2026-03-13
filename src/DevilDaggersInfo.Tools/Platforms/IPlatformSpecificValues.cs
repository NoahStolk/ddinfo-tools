using DevilDaggersInfo.Web.ApiSpec.Tools;

namespace DevilDaggersInfo.Tools.Platforms;

internal interface IPlatformSpecificValues
{
	AppOperatingSystem AppOperatingSystem { get; }

	string DefaultInstallationPath { get; }
}
