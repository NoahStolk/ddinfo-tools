using DevilDaggersInfo.DevUtil.DistributeApp;
using DevilDaggersInfo.DevUtil.DistributeApp.Model;
using DevilDaggersInfo.Tools.Engine.Content.Conversion;
using DevilDaggersInfo.Web.ApiSpec.Admin.Tools;
using DevilDaggersInfo.Web.ApiSpec.Main.Authentication;
using System.Diagnostics;
using System.IO.Compression;

if (Question("Build app for Windows?"))
	await DistributeAsync(ToolBuildType.WindowsWarp, ToolPublishMethod.SelfContained);

if (Question("Build app for Linux?"))
	await DistributeAsync(ToolBuildType.LinuxWarp, ToolPublishMethod.SelfContained);

static bool Question(string question)
{
	Console.WriteLine($"{question} y/n");
	bool result = Console.ReadKey().KeyChar == 'y';
	Console.WriteLine();
	return result;
}

static async Task DistributeAsync(ToolBuildType toolBuildType, ToolPublishMethod toolPublishMethod)
{
	const string toolName = "ddinfo-tools";
	const string toolProjectName = "DevilDaggersInfo.Tools";
	const string publishDirectoryName = "_temp-release";

	string root = Path.Combine("..", "..", "..", "..");
	string projectFilePath = Path.Combine(root, toolProjectName, $"{toolProjectName}.csproj");
	if (!File.Exists(projectFilePath))
		throw new InvalidOperationException($"Cannot find project file {projectFilePath}.");

	string zipOutputDirectory = Path.Combine(root, toolProjectName, "bin");

	Console.WriteLine("Building...");
	await AppBuilder.BuildAsync(projectFilePath, publishDirectoryName, toolBuildType, toolPublishMethod);

	string publishDirectoryPath = Path.Combine(Path.GetDirectoryName(projectFilePath) ?? throw new InvalidOperationException($"Cannot get root directory of {projectFilePath}."), publishDirectoryName);

	ContentFileWriter.GenerateContentFile(Path.Combine(root, toolProjectName, "Content"), Path.Combine(publishDirectoryPath, "ddinfo-assets"));

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
