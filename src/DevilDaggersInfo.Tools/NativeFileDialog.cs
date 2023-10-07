using NativeFileDialogSharp;

namespace DevilDaggersInfo.Tools;

public static class NativeFileDialog
{
	public static string? CreateOpenFileDialog(string? extensionFilter)
	{
		DialogResult dialogResult = Dialog.FileOpen(); // TODO: extensionFilter
		return dialogResult.Path;
	}

	public static string? CreateSaveFileDialog(string? extensionFilter)
	{
		DialogResult dialogResult = Dialog.FileSave(); // TODO: extensionFilter
		return dialogResult.Path;
	}

	public static string? SelectDirectory()
	{
		DialogResult dialogResult = Dialog.FolderPicker();
		return dialogResult.Path;
	}
}
