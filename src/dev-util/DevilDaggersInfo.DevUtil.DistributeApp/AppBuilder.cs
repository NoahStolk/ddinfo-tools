using CliWrap;
using DevilDaggersInfo.Api.Admin.Tools;
using System.Text;

namespace DevilDaggersInfo.DevUtil.DistributeApp;

public static class AppBuilder
{
	public static async Task BuildAsync(string projectFilePath, string publishDirectoryName, ToolBuildType toolBuildType, ToolPublishMethod toolPublishMethod)
	{
		// TODO: Print output to console so we can inspect trim warnings from the build process.
		await Cli.Wrap("dotnet")
			.WithArguments(GetArguments(projectFilePath, publishDirectoryName, toolBuildType, toolPublishMethod))
			.ExecuteAsync();
	}

	private static string GetArguments(string projectFilePath, string publishDirectoryName, ToolBuildType toolBuildType, ToolPublishMethod toolPublishMethod)
	{
		string runtimeIdentifier = toolBuildType switch
		{
			ToolBuildType.LinuxWarp or ToolBuildType.LinuxConsole => "linux-x64",
			ToolBuildType.WindowsWarp or ToolBuildType.WindowsConsole => "win-x64",
			_ => throw new NotImplementedException(),
		};

		// Example:
		// dotnet publish -p:PublishTrimmed=True -p:EnableCompressionInSingleFile=True -p:PublishReadyToRun=False -p:PublishProtocol=FileSystem -p:TargetFramework=net7.0 -p:RuntimeIdentifier=win-x64 -p:Platform=x64 -p:Configuration=Release -p:PublishDir=bin\Release\net7.0\win-x64\publish -p:PublishSingleFile=True -p:SelfContained=True -p:PublishMethod=SELF_CONTAINED
		Dictionary<string, string> dictionary = new()
		{
			{ "PublishTrimmed", "True" },
			{ "EnableCompressionInSingleFile", "True" },
			{ "PublishReadyToRun", "False" },
			{ "PublishProtocol", "FileSystem" },
			{ "TargetFramework", "net7.0" },
			{ "RuntimeIdentifier", runtimeIdentifier },
			{ "Platform", "x64" },
			{ "Configuration", "Release" },
			{ "PublishDir", publishDirectoryName },
		};

		switch (toolPublishMethod)
		{
			case ToolPublishMethod.SelfContained:
				dictionary.Add("PublishSingleFile", "True");
				dictionary.Add("SelfContained", "True");
				dictionary.Add("PublishMethod", "SELF_CONTAINED");
				break;
			case ToolPublishMethod.Aot:
				dictionary.Add("PublishAot", "True");
				break;
			case ToolPublishMethod.Default: throw new NotImplementedException();
			default: throw new ArgumentOutOfRangeException(nameof(toolPublishMethod), toolPublishMethod, null);
		}

		StringBuilder stringBuilder = new($"publish {projectFilePath}");
		foreach (KeyValuePair<string, string> keyValuePair in dictionary)
			stringBuilder.Append(" -p:").Append(keyValuePair.Key).Append('=').Append(keyValuePair.Value);

		return stringBuilder.ToString();
	}
}
