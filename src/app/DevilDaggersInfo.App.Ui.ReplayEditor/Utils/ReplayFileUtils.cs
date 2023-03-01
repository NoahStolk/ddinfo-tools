using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.StateManagement.ReplayEditor.Actions;
using DevilDaggersInfo.Core.Replay;

namespace DevilDaggersInfo.App.Ui.ReplayEditor.Utils;

public static class ReplayFileUtils
{
	public static void OpenReplay()
	{
		string? filePath = Root.Dependencies.NativeFileSystemService.CreateOpenFileDialog("Open replay file", "Devil Daggers replay files (*.ddreplay)|*.ddreplay");
		if (filePath == null)
			return;

		byte[] fileContents;
		try
		{
			fileContents = File.ReadAllBytes(filePath);
		}
		catch (Exception ex)
		{
			Root.Dependencies.NativeDialogService.ReportError($"Could not open file '{filePath}'.", ex);
			return;
		}

		ReplayBinary<LocalReplayBinaryHeader> replayBinary;
		try
		{
			replayBinary = new(fileContents);
		}
		catch (Exception ex)
		{
			Root.Dependencies.NativeDialogService.ReportError($"Could not parse replay file '{filePath}'.", ex);
			return;
		}

		StateManager.Dispatch(new LoadReplay(Path.GetFileName(filePath), replayBinary));
	}

	public static void SaveReplay()
	{
		string? filePath = Root.Dependencies.NativeFileSystemService.CreateSaveFileDialog("Save replay file", "Devil Daggers replay files (*.ddreplay)|*.ddreplay");
		if (filePath == null)
			return;

		if (Directory.Exists(filePath))
		{
			Root.Dependencies.NativeDialogService.ReportError("Specified file path is an existing directory. Please specify a file path.");
			return;
		}

		if (File.Exists(filePath))
		{
			bool? result = Root.Dependencies.NativeDialogService.PromptYesNo("File already exists", "The specified file path already exists. Do you want to overwrite it?");
			if (result is null or false)
				return;
		}

		File.WriteAllBytes(filePath, StateManager.ReplayState.Replay.Compile());
	}
}
