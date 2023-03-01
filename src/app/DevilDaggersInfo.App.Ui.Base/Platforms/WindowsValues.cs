using DevilDaggersInfo.Api.App.ProcessMemory;
using DevilDaggersInfo.Types.Web;

namespace DevilDaggersInfo.App.Ui.Base.Platforms;

public class WindowsValues : IPlatformSpecificValues
{
	public ToolBuildType BuildType => ToolBuildType.WindowsWarp;

	public SupportedOperatingSystem OperatingSystem => SupportedOperatingSystem.Windows;

	public string DefaultInstallationPath => @"C:\Program Files (x86)\Steam\steamapps\common\devildaggers";
}
