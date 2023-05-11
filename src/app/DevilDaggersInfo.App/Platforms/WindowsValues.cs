using DevilDaggersInfo.Api.App.ProcessMemory;
using DevilDaggersInfo.Api.App.Updates;

namespace DevilDaggersInfo.App.Platforms;

public class WindowsValues : IPlatformSpecificValues
{
	public ToolBuildType BuildType => ToolBuildType.WindowsWarp;

	public SupportedOperatingSystem OperatingSystem => SupportedOperatingSystem.Windows;

	public string DefaultInstallationPath => @"C:\Program Files (x86)\Steam\steamapps\common\devildaggers";
}
