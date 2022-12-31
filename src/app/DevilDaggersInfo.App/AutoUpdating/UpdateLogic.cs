using DevilDaggersInfo.Api.App.Updates;
using DevilDaggersInfo.Core.Versioning;
using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;

namespace DevilDaggersInfo.App.AutoUpdating;

public static class UpdateLogic
{
	public static void InstallUpdate(GetLatestVersionFile? latestVersionFile, AppVersion? newAppVersion)
	{
		if (latestVersionFile == null)
			throw new UpdateException("Download failed.");

		string? currentFileDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		if (currentFileDirectory == null)
			throw new UpdateException("Could not get current file directory.");

		string parentDirectory = Path.GetFullPath(Path.Combine(currentFileDirectory, ".."));
		string newDirectory = Path.Combine(parentDirectory, $"ddinfo-tools-{newAppVersion}");
		if (Directory.Exists(newDirectory))
			throw new UpdateException($"Installation destination directory already exists.\n{newDirectory}");

		if (File.Exists(newDirectory))
			throw new UpdateException($"There is a file at the installation destination directory\nwhich conflicts with the installation destination directory.\n{newDirectory}");

		Directory.CreateDirectory(newDirectory);

		using MemoryStream ms = new(latestVersionFile.ZipFileContents);
		using ZipArchive archive = new(ms);
		archive.ExtractToDirectory(newDirectory, true);

		// TODO: Use something else on Linux.
		string? newExecutableFileName = Array.Find(Directory.GetFiles(newDirectory), f => Path.GetExtension(f) == ".exe");
		if (newExecutableFileName == null)
			throw new UpdateException($"Could not launch new version. Please launch it manually. It is installed at {newDirectory}.");

		Process process = new();
		process.StartInfo.FileName = newExecutableFileName;
		process.StartInfo.WorkingDirectory = newDirectory;
		process.Start();

		// TODO: Find a way to delete the current executable.
		Environment.Exit(0);
	}
}
