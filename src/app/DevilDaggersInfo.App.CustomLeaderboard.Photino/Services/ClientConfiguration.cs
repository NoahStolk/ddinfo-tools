using DevilDaggersInfo.Api.Ddcl.ProcessMemory;
using DevilDaggersInfo.Common.Utils;
using DevilDaggersInfo.Razor.CustomLeaderboard.Services;
using DevilDaggersInfo.Types.Web;

namespace DevilDaggersInfo.App.CustomLeaderboard.Photino.Services;

public class ClientConfiguration : IClientConfiguration
{
	public string GetApplicationName() => "DevilDaggersCustomLeaderboards";

	public string GetApplicationVersion() => VersionUtils.EntryAssemblyVersion;

	public string GetBuildMode()
	{
#if DEBUG
		return "Debug";
#else
		return "Release";
#endif
	}

	public string GetHostBaseUrl()
	{
#if TESTING
		return "https://localhost:44318";
#else
		return "https://devildaggers.info";
#endif
	}

	public SupportedOperatingSystem GetOperatingSystem() => SupportedOperatingSystem.Windows; // TODO: Get whether Linux is used from build.

	public ToolBuildType GetToolBuildType() => ToolBuildType.WindowsPhotino; // TODO: Get whether Linux is used from build.

	public ToolPublishMethod GetToolPublishMethod()
	{
#if TODO_WIN7_FLAG
		return ToolPublishMethod.Default;
#else
		return ToolPublishMethod.SelfContained;
#endif
	}
}
