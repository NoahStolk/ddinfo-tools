using NativeFileDialogSharp;

namespace DevilDaggersInfo.App.Core.NativeInterface.Services.Windows;

/// <summary>
/// Platform-specific code for interacting with the Windows file system.
/// </summary>
public class WindowsFileSystemService : INativeFileSystemService
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
