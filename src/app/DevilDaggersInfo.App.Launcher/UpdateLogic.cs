using System.Diagnostics;
using System.IO.Compression;

namespace DevilDaggersInfo.App.Launcher;

public static class UpdateLogic
{
	public static async Task<bool> ShouldUpdateApp()
	{
		string executableFileName = FindExecutableFileName() ?? throw new InvalidOperationException("ShouldUpdate can only be called when the app is installed.");
		FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(executableFileName);
		if (versionInfo.FileVersion == null)
		{
			Cmd.WriteLine(ConsoleColor.Red, "Could not get file version from installed executable.");
			return true;
		}

		Cmd.WriteLine(ConsoleColor.Yellow, "Checking for updates...");
		bool isLatestApp = await Client.IsLatestVersionAsync("ddinfo-tools", versionInfo.FileVersion, Constants.AppPublishMethod, Constants.AppBuildType);
		if (isLatestApp)
		{
			Cmd.WriteLine(ConsoleColor.Yellow, "No new version available.");
			return false;
		}

		Cmd.WriteLine(ConsoleColor.Green, "New version available.");
		return true;
	}

	public static void CleanOldInstallation()
	{
		if (!Directory.Exists(Constants.InstallationDirectory))
			return;

		foreach (string existingFile in Directory.GetFiles(Constants.InstallationDirectory))
			File.Delete(existingFile);
	}

	public static async Task DownloadAndInstallAsync()
	{
		byte[] zipFileContents = await Client.DownloadAppAsync();
		using MemoryStream ms = new(zipFileContents);
		using ZipArchive archive = new(ms);
		archive.ExtractToDirectory(Constants.InstallationDirectory, true);
	}

	public static void Launch()
	{
		string? executableFileName = FindExecutableFileName();
		if (executableFileName == null)
		{
			Cmd.WriteLine(ConsoleColor.Red, $"Could not launch executable. It should be installed at '{Constants.InstallationDirectory}'.");
			return;
		}

		Process process = new();
		process.StartInfo.FileName = executableFileName;
		process.StartInfo.WorkingDirectory = Constants.InstallationDirectory;
		process.Start();
	}

	private static string? FindExecutableFileName()
	{
		// TODO: Use something else on Linux.
		return !Directory.Exists(Constants.InstallationDirectory) ? null : Array.Find(Directory.GetFiles(Constants.InstallationDirectory), f => Path.GetExtension(f) == ".exe");
	}

	public static bool IsInstalled()
	{
		return FindExecutableFileName() != null;
	}
}
