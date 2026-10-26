using DevilDaggersInfo.Web.ApiSpec.Tools;

namespace DevilDaggersInfo.Tools.Platforms;

internal sealed class WindowsValues : IPlatformSpecificValues
{
	public AppOperatingSystem AppOperatingSystem => AppOperatingSystem.Windows;
}
