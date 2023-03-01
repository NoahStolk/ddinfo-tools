using DevilDaggersInfo.Api.App.ProcessMemory;
using DevilDaggersInfo.Types.Web;

namespace DevilDaggersInfo.App.Ui.Base.Platforms;

public class LinuxValues : IPlatformSpecificValues
{
	public ToolBuildType BuildType => ToolBuildType.LinuxWarp;

	public SupportedOperatingSystem OperatingSystem => SupportedOperatingSystem.Linux;

	public string DefaultInstallationPath => string.Empty;
}
