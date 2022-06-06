using DevilDaggersInfo.Core.NativeInterface;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System.IO;

namespace DevilDaggersInfo.App.AssetEditor.Wpf.Services;

/// <summary>
/// Platform-specific code for interacting with the OS file system.
/// </summary>
public class NativeFileSystemService : INativeFileSystemService
{
	public INativeFileSystemService.FileResult? OpenFile(string? extensionFilter)
	{
		OpenFileDialog dialog = new()
		{
			Filter = extensionFilter ?? string.Empty,
		};
		bool? openResult = dialog.ShowDialog();
		if (!openResult.HasValue || !openResult.Value)
			return null;

		return new(dialog.FileName, File.ReadAllBytes(dialog.FileName));
	}

	public void SaveDataToFile(byte[] data)
	{
		SaveFileDialog dialog = new();
		bool? saveResult = dialog.ShowDialog();
		if (!saveResult.HasValue || !saveResult.Value)
			return;

		File.WriteAllBytes(dialog.FileName, data);
	}

	public string? SelectDirectory()
	{
		VistaFolderBrowserDialog folderDialog = new();
		if (folderDialog.ShowDialog() == true)
			return folderDialog.SelectedPath;

		return null;
	}
}
