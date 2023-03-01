namespace DevilDaggersInfo.App.Core.NativeInterface.Services;

public interface INativeFileSystemService
{
	string? CreateOpenFileDialog(string dialogTitle, string? extensionFilter);

	string? CreateSaveFileDialog(string dialogTitle, string? extensionFilter);

	string? SelectDirectory();
}
