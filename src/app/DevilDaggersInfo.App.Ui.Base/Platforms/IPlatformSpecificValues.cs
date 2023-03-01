using DevilDaggersInfo.Api.App.ProcessMemory;
using DevilDaggersInfo.Types.Web;

namespace DevilDaggersInfo.App.Ui.Base.Platforms;

public interface IPlatformSpecificValues
{
	ToolBuildType BuildType { get; }

	SupportedOperatingSystem OperatingSystem { get; }

	string DefaultInstallationPath { get; }
}
