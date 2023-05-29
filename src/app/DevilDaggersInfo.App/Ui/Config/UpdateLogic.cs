using DevilDaggersInfo.Api.App.Updates;
using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;

namespace DevilDaggersInfo.App.Ui.Config;

public static class UpdateLogic
{
#if WINDOWS
	private const ToolBuildType _appBuildType = ToolBuildType.WindowsWarp;
	private const string _exeFileName = "ddinfo-tools.exe";
	private const string _oldExeFileName = "ddinfo-tools.old.exe";
#elif LINUX
	private const ToolBuildType _appBuildType = ToolBuildType.LinuxWarp;
#endif

	private static readonly Uri _baseAddress = new("https://devildaggers.info/");

	public static async Task RunAsync()
	{
		string installationDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new($"Could not get installation directory of '{Assembly.GetExecutingAssembly().Location}'.");

		// Rename the currently running executable, so the new executable can be installed.
		File.Move(_exeFileName, _oldExeFileName);

		byte[] zipFileContents = await DownloadAppAsync();
		using MemoryStream ms = new(zipFileContents);
		using ZipArchive archive = new(ms);

		// This will overwrite all the existing files.
		archive.ExtractToDirectory(installationDirectory, true);

		Process process = new();
		process.StartInfo.FileName = _exeFileName;
		process.StartInfo.WorkingDirectory = installationDirectory;
		process.Start();

		Environment.Exit(0);
	}

	private static async Task<byte[]> DownloadAppAsync()
	{
		using HttpRequestMessage request = new()
		{
			RequestUri = new($"api/app-launcher/latest-version-file?publishMethod={ToolPublishMethod.SelfContained}&buildType={_appBuildType}", UriKind.Relative),
			Method = HttpMethod.Get,
		};

		using HttpClient httpClient = new() { BaseAddress = _baseAddress };

		using HttpResponseMessage response = await httpClient.SendAsync(request);
		if (!response.IsSuccessStatusCode)
			throw new HttpRequestException(await response.Content.ReadAsStringAsync(), null, response.StatusCode);

		return await response.Content.ReadAsByteArrayAsync();
	}
}
