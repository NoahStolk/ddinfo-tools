using DevilDaggersInfo.App.Launcher;
using System.Reflection;

string launcherVersion = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "0.0.0.0";
Cmd.WriteLine(ConsoleColor.Yellow, $"DDINFO TOOLS LAUNCHER {launcherVersion}");
if (!await Client.IsLatestVersionAsync("ddinfo-tools-launcher", launcherVersion, Constants.AppLauncherPublishMethod, Constants.AppLauncherBuildType))
{
	Cmd.WriteLine(ConsoleColor.Red, "The launcher is outdated. Please go to https://devildaggers.info/ to download the latest launcher.");
	Environment.Exit(0);
}

if (!UpdateLogic.IsInstalled())
{
	Cmd.WriteLine(ConsoleColor.Yellow, "Downloading...");
	await UpdateLogic.DownloadAndInstallAsync();
	Cmd.WriteLine(ConsoleColor.Green, "App installed.");
}
else if (await UpdateLogic.ShouldUpdateApp())
{
	UpdateLogic.CleanOldInstallation();

	Cmd.WriteLine(ConsoleColor.Yellow, "Downloading new version...");
	await UpdateLogic.DownloadAndInstallAsync();
	Cmd.WriteLine(ConsoleColor.Green, "Update installed.");
}

UpdateLogic.Launch();
