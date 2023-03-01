namespace DevilDaggersInfo.App.Core.NativeInterface.Services.Linux;

/// <summary>
/// Platform-specific code for interacting with the Linux file system.
/// </summary>
public class LinuxFileSystemService : INativeFileSystemService
{
	public string? CreateOpenFileDialog(string dialogTitle, string? extensionFilter)
	{
		throw new NotImplementedException();
	}

	public string? CreateSaveFileDialog(string dialogTitle, string? extensionFilter)
	{
		throw new NotImplementedException();
	}

	public string? SelectDirectory()
	{
		throw new NotImplementedException();
	}
}
