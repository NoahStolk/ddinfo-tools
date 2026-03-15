namespace DevilDaggersInfo.Tools.Dialogs;

internal interface INativeFileDialog
{
	bool DialogOpen { get; }

	void CreateOpenFileDialog(Action<string?> callback, string? extensionFilter);

	void CreateSaveFileDialog(Action<string?> callback, string? extensionFilter);

	void SelectDirectory(Action<string?> callback);

	void Update();
}
