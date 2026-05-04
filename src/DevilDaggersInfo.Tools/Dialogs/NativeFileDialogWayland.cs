using NativeFileDialogSharp;

namespace DevilDaggersInfo.Tools.Dialogs;

internal sealed class NativeFileDialogWayland : INativeFileDialog
{
	private Func<string?>? _pendingDialog;
	private Action<string?>? _callback;

	/// <summary>
	/// Used to prevent multiple dialogs from being opened from the main thread.
	/// </summary>
	public bool DialogOpen => _pendingDialog != null;

	public void CreateOpenFileDialog(Action<string?> callback, string? extensionFilter)
	{
		if (DialogOpen)
			return;

		_pendingDialog = () =>
		{
			DialogResult result = Dialog.FileOpen(extensionFilter);
			return result.Path;
		};
		_callback = callback;
	}

	public void CreateSaveFileDialog(Action<string?> callback, string? extensionFilter)
	{
		if (DialogOpen)
			return;

		_pendingDialog = () =>
		{
			DialogResult result = Dialog.FileSave(extensionFilter);
			return result.Path;
		};
		_callback = callback;
	}

	public void SelectDirectory(Action<string?> callback)
	{
		if (DialogOpen)
			return;

		_pendingDialog = () =>
		{
			DialogResult result = Dialog.FolderPicker();
			return result.Path;
		};
		_callback = callback;
	}

	// Call this once per frame on the main thread
	public void Update()
	{
		if (_pendingDialog == null)
			return;

		Func<string?> dialog = _pendingDialog;
		Action<string?>? callback = _callback;

		_pendingDialog = null;
		_callback = null;

		string? result = dialog();
		callback?.Invoke(result);
	}
}
