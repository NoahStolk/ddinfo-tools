using DevilDaggersInfo.Api.App;
using DevilDaggersInfo.App.Ui;
using DevilDaggersInfo.App.Utils;
using System.Diagnostics;
using System.IO.Compression;

namespace DevilDaggersInfo.App;

public static class UpdateLogic
{
	private const string _oldTemporaryDirectoryName = "old";

#if WINDOWS
	private const AppOperatingSystem _operatingSystem = AppOperatingSystem.Windows;
	private const string _exeFileName = "ddinfo-tools.exe"; // TODO: Get this from the AssemblyName MSBuild property.
#elif LINUX
	private const AppOperatingSystem _operatingSystem = AppOperatingSystem.Linux;
	private const string _exeFileName = "ddinfo-tools";
#endif

	private static readonly Uri _baseAddress = new("https://devildaggers.info/");

	/// <summary>
	/// Can be called when the application starts up.
	/// </summary>
	public static void TryDeleteOldInstallation()
	{
		try
		{
			if (Directory.Exists(_oldTemporaryDirectoryName))
				Directory.Delete(_oldTemporaryDirectoryName, true);
		}
		catch (Exception ex)
		{
			Root.Log.Error(ex, "Could not delete old directory.");
		}
	}

	public static async Task RunAsync()
	{
		try
		{
			// If a previous attempt to update failed, delete the old directory first, so we can create it again.
			if (Directory.Exists(_oldTemporaryDirectoryName))
			{
				UpdateWindow.LogMessages.Add("Deleting old temporary directory...");
				Directory.Delete(_oldTemporaryDirectoryName, true);
			}

			// Move all the current files, including the currently running executable, to a temporary directory.
			// This way we can install the new files in the original directory without having to worry about file locks.
			UpdateWindow.LogMessages.Add("Creating temporary directory...");
			Directory.CreateDirectory(_oldTemporaryDirectoryName);
			foreach (string? currentFileName in Directory.GetFiles(AssemblyUtils.InstallationDirectory).Select(Path.GetFileName))
			{
				// Don't move the log file, because it's currently being written to.
				// It also makes sense to preserve these files, because they contain the logs of previous versions.
				// Log files should not be present in distributed builds, so these files can safely be ignored.
				if (currentFileName == null || Path.GetExtension(currentFileName).Equals(".log", StringComparison.OrdinalIgnoreCase))
					continue;

				UpdateWindow.LogMessages.Add($"Moving '{currentFileName}' to temporary directory...");
				File.Move(currentFileName, Path.Combine(_oldTemporaryDirectoryName, currentFileName));
			}

			UpdateWindow.LogMessages.Add("Downloading update...");
			byte[] zipFileContents = await DownloadAppAsync();

			UpdateWindow.LogMessages.Add("Opening ZIP...");

			using MemoryStream ms = new(zipFileContents);
			using ZipArchive archive = new(ms);

			UpdateWindow.LogMessages.Add("Installing update...");
			archive.ExtractToDirectory(AssemblyUtils.InstallationDirectory, true);

			UpdateWindow.LogMessages.Add("Launching new version...");
			Process process = new();
			process.StartInfo.FileName = _exeFileName;
			process.StartInfo.WorkingDirectory = AssemblyUtils.InstallationDirectory;
			process.Start();

			Environment.Exit(0);
		}
		catch (Exception ex)
		{
			const string errorMessage = "An error occurred while updating the application.";
			UpdateWindow.LogMessages.Add($"{errorMessage}\n{ex.Message}");
			Root.Log.Error(ex, errorMessage);
		}
	}

	private static async Task<byte[]> DownloadAppAsync()
	{
		using HttpRequestMessage request = new()
		{
			RequestUri = new($"api/app/updates/latest-file?appOperatingSystem={_operatingSystem}", UriKind.Relative),
			Method = HttpMethod.Get,
		};

		using HttpClient httpClient = new() { BaseAddress = _baseAddress };

		using HttpResponseMessage response = await httpClient.SendAsync(request);
		if (!response.IsSuccessStatusCode)
			throw new HttpRequestException(await response.Content.ReadAsStringAsync(), null, response.StatusCode);

		return await response.Content.ReadAsByteArrayAsync();
	}
}
