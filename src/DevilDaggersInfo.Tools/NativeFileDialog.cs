using NativeFileDialogSharp;

namespace DevilDaggersInfo.Tools;

/// <summary>
/// Wrapper to make sure the <see cref="Dialog"/> class doesn't block the main thread and cause other problems like key states getting stuck.
/// </summary>
public static class NativeFileDialog
{
	/// <summary>
	/// Used to prevent multiple dialogs from being opened from the main thread.
	/// </summary>
	public static bool DialogOpen { get; private set; }

	public static void CreateOpenFileDialog(Action<string?> callback, string? extensionFilter)
	{
		if (DialogOpen)
			return;

		DialogOpen = true;
		OpenDialog(callback, async () =>
		{
			await Task.Yield();
			DialogResult dialogResult = Dialog.FileOpen(); // TODO: extensionFilter
			DialogOpen = false;
			return dialogResult.Path;
		});
	}

	public static void CreateSaveFileDialog(Action<string?> callback, string? extensionFilter)
	{
		if (DialogOpen)
			return;

		DialogOpen = true;
		OpenDialog(callback, async () =>
		{
			await Task.Yield();
			DialogResult dialogResult = Dialog.FileSave(); // TODO: extensionFilter
			DialogOpen = false;
			return dialogResult.Path;
		});
	}

	public static void SelectDirectory(Action<string?> callback)
	{
		if (DialogOpen)
			return;

		DialogOpen = true;
		OpenDialog(callback, async () =>
		{
			await Task.Yield();
			DialogResult dialogResult = Dialog.FolderPicker();
			DialogOpen = false;
			return dialogResult.Path;
		});
	}

	private static void OpenDialog(Action<string?> callback, Func<Task<string?>> call)
	{
		Task.Run(async () => callback(await call()));
	}
}
