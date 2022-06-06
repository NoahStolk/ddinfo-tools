using DevilDaggersInfo.Api.Ddcl.ProcessMemory;
using DevilDaggersInfo.Api.Ddcl.Tools;
using DevilDaggersInfo.Core.CustomLeaderboard.Services;

namespace DevilDaggersInfo.App.CustomLeaderboard.Wpf.Services;

public class ClientConfiguration : IClientConfiguration
{
	public string GetApplicationName() => "Devil Daggers Custom Leaderboards";

	public string GetApplicationVersion()
	{
		throw new NotImplementedException();
	}

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

	public SupportedOperatingSystem GetOperatingSystem() => SupportedOperatingSystem.Windows;

	public ToolBuildType GetToolBuildType() => ToolBuildType.WindowsWpf;

	public ToolPublishMethod GetToolPublishMethod()
	{
#if TODO_WIN7_FLAG
		return ToolPublishMethod.Default;
#else
		return ToolPublishMethod.SelfContained;
#endif
	}
}
