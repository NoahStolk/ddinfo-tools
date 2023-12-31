using DevilDaggersInfo.Web.ApiSpec.Tools;

namespace DevilDaggersInfo.Tools.Platforms;

public class WindowsValues : IPlatformSpecificValues
{
	public AppOperatingSystem AppOperatingSystem => AppOperatingSystem.Windows;

	public string DefaultInstallationPath => @"C:\Program Files (x86)\Steam\steamapps\common\devildaggers";
}
