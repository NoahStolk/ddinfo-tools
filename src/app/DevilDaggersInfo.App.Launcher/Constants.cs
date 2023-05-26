namespace DevilDaggersInfo.App.Launcher;

public static class Constants
{
#if WINDOWS
	public const ToolBuildType AppBuildType = ToolBuildType.WindowsWarp;
	public const ToolBuildType AppLauncherBuildType = ToolBuildType.WindowsConsole;
#elif LINUX
	public const ToolBuildType AppBuildType = ToolBuildType.LinuxWarp;
	public const ToolBuildType AppLauncherBuildType = ToolBuildType.LinuxConsole;
#endif

	public const ToolPublishMethod AppPublishMethod = ToolPublishMethod.SelfContained; // TODO: What if we want to publish the app using native AOT?
	public const ToolPublishMethod AppLauncherPublishMethod = ToolPublishMethod.Aot;

	public const string InstallationDirectory = "DDINFO TOOLS";
}
