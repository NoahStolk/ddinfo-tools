using DevilDaggersInfo.Api.Admin.Tools;
using DevilDaggersInfo.Api.Main.Authentication;
using DevilDaggersInfo.DevUtil.DistributeApp;
using System.Diagnostics;
using System.IO.Compression;

const string publishDirectoryName = "_temp-release";

if (Question("Build app for Windows?"))
	await DistributeAsync("ddinfo-tools", "app", "DevilDaggersInfo.App", ToolBuildType.WindowsWarp, ToolPublishMethod.SelfContained);

if (Question("Build app for Linux?"))
	await DistributeAsync("ddinfo-tools", "app", "DevilDaggersInfo.App", ToolBuildType.LinuxWarp, ToolPublishMethod.SelfContained);

static bool Question(string question)
{
	Console.WriteLine($"{question} y/n");
	bool result = Console.ReadKey().KeyChar == 'y';
	Console.WriteLine();
	return result;
}

static async Task DistributeAsync(string toolName, string srcDir, string projectName, ToolBuildType toolBuildType, ToolPublishMethod toolPublishMethod)
{
	string root = Path.Combine("..", "..", "..", "..", "..");
	string projectFilePath = Path.Combine(root, srcDir, projectName, $"{projectName}.csproj");
	string zipOutputDirectory = Path.Combine(root, srcDir, projectName, "bin");

	Console.WriteLine("Building...");
	await AppBuilder.BuildAsync(projectFilePath, publishDirectoryName, toolBuildType, toolPublishMethod);

	string publishDirectoryPath = Path.Combine(Path.GetDirectoryName(projectFilePath) ?? throw new($"Cannot get root directory of {projectFilePath}."), publishDirectoryName);

	// Copy content file if it exists.
	string contentFilePathSrc = Path.Combine(projectFilePath, "..", "bin", "Debug", "net7.0", "ddinfo-assets");
	string contentFilePathDst = Path.Combine(publishDirectoryPath, "ddinfo-assets");
	if (File.Exists(contentFilePathSrc))
	{
		if (File.Exists(contentFilePathDst))
		{
			Console.WriteLine("Deleting old content file...");
			File.Delete(contentFilePathDst);
		}

		Console.WriteLine("Copying new content file...");
		File.Copy(contentFilePathSrc, contentFilePathDst);
	}

	if (!Question("Distribute?"))
	{
		Process.Start("explorer.exe", publishDirectoryPath);
		return;
	}

	Console.WriteLine("Getting version from compiled application...");
	string version = ProjectReader.ReadVersionFromProjectFile(projectFilePath);
	string outputZipFilePath = Path.Combine(zipOutputDirectory, $"{toolName}-{version}-{toolBuildType}-{toolPublishMethod}.zip");

	Console.WriteLine($"Deleting previous .zip file '{outputZipFilePath}' (if present)...");
	File.Delete(outputZipFilePath);

	Console.WriteLine($"Creating '{outputZipFilePath}' from temporary directory '{publishDirectoryPath}'...");
	ZipFile.CreateFromDirectory(publishDirectoryPath, outputZipFilePath);

	Console.WriteLine($"Deleting temporary directory '{publishDirectoryPath}'...");
	Directory.Delete(publishDirectoryPath, true);

	Console.WriteLine("Fetching credentials...");
	LoginResponse loginToken = await ApiHttpClient.LoginAsync();

	Console.WriteLine("Uploading zip file...");
	await ApiHttpClient.UploadAsync(toolName, version, toolBuildType, toolPublishMethod, outputZipFilePath, loginToken.Token);

	Console.WriteLine($"Deleting zip file '{outputZipFilePath}'...");
	File.Delete(outputZipFilePath);

	Console.WriteLine("Done.");
}
