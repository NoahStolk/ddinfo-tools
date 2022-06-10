using DevilDaggersInfo.Core.NativeInterface;

namespace DevilDaggersInfo.App.AssetEditor.Photino.Services;

/// <summary>
/// Platform-specific code for interacting with the OS file system.
/// </summary>
public class NativeFileSystemService : INativeFileSystemService
{
	public INativeFileSystemService.FileResult? OpenFile(string? extensionFilter)
	{
		throw new NotImplementedException();
	}

	public void SaveDataToFile(byte[] data)
	{
		throw new NotImplementedException();
	}

	public string? SelectDirectory()
	{
		throw new NotImplementedException();
	}
}
