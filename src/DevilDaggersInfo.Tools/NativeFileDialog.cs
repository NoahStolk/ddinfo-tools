using NativeFileDialogSharp;

namespace DevilDaggersInfo.Tools;

/// <summary>
/// Wrapper to make sure key states do not get stuck when opening a dialog.
/// </summary>
public static class NativeFileDialog
{
	public static string? CreateOpenFileDialog(string? extensionFilter)
	{
		Root.Application.MainAppWindow.ImGuiController?.ForceClearImGuiInput();
		DialogResult dialogResult = Dialog.FileOpen(); // TODO: extensionFilter
		return dialogResult.Path;
	}

	public static string? CreateSaveFileDialog(string? extensionFilter)
	{
		Root.Application.MainAppWindow.ImGuiController?.ForceClearImGuiInput();
		DialogResult dialogResult = Dialog.FileSave(); // TODO: extensionFilter
		return dialogResult.Path;
	}

	public static string? SelectDirectory()
	{
		Root.Application.MainAppWindow.ImGuiController?.ForceClearImGuiInput();
		DialogResult dialogResult = Dialog.FolderPicker();
		return dialogResult.Path;
	}
}
