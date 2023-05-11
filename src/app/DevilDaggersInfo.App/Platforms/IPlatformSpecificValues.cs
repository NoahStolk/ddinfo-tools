using DevilDaggersInfo.Api.App.ProcessMemory;
using DevilDaggersInfo.Api.App.Updates;

namespace DevilDaggersInfo.App.Platforms;

public interface IPlatformSpecificValues
{
	ToolBuildType BuildType { get; }

	SupportedOperatingSystem OperatingSystem { get; }

	string DefaultInstallationPath { get; }
}
