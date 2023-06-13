using DevilDaggersInfo.Api.App;

namespace DevilDaggersInfo.App.Platforms;

public interface IPlatformSpecificValues
{
	AppOperatingSystem AppOperatingSystem { get; }

	string DefaultInstallationPath { get; }
}
