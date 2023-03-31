using NativeFileDialogSharp;

namespace DevilDaggersInfo.App.Core.NativeInterface.Services.Linux;

/// <summary>
/// Platform-specific code for interacting with the Linux file system.
/// </summary>
public class LinuxFileSystemService : INativeFileSystemService
{
	public string? CreateOpenFileDialog(string dialogTitle, string? extensionFilter)
	{
		return Dialog.FileOpen().Path;
	}

	public string? CreateSaveFileDialog(string dialogTitle, string? extensionFilter)
	{
		return Dialog.FileSave().Path;
	}

	public string? SelectDirectory()
	{
		return Dialog.FolderPicker().Path;
	}
}
