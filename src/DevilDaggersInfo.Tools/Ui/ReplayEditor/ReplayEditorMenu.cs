using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.PostProcessing.ReplaySimulation;
using DevilDaggersInfo.Tools.Dialogs;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.GameMemory;
using DevilDaggersInfo.Tools.NativeInterface.Services;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using DevilDaggersInfo.Tools.Utils;
using Hexa.NET.ImGui;
using Serilog;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor;

internal sealed class ReplayEditorMenu(
	UiLayoutManager uiLayoutManager,
	ReplayEditorWindow replayEditorWindow,
	ReplayEditor3DWindow replayEditor3DWindow,
	LeaderboardReplayBrowser leaderboardReplayBrowser,
	INativeFileDialog nativeFileDialog,
	PopupManager popupManager,
	FileStates fileStates,
	GameMemoryServiceWrapper gameMemoryServiceWrapper,
	GameMemoryService gameMemoryService,
	ILogger logger)
{
	public void Render()
	{
		if (ImGui.BeginMainMenuBar())
		{
			if (ImGui.BeginMenu("File"))
			{
				RenderFileMenu();
				ImGui.EndMenu();
			}

			ImGui.EndMainMenuBar();
		}
	}

	private void RenderFileMenu()
	{
		if (ImGui.MenuItem("New", "Ctrl+N"))
			NewReplay();

		if (ImGui.MenuItem("Open", "Ctrl+O"))
			OpenReplay();

		if (ImGui.MenuItem("Open from leaderboard", "Ctrl+Shift+O"))
			OpenLeaderboardReplay();

		if (ImGui.MenuItem("Open from game (local replays only)", "Ctrl+G"))
			OpenReplayFromGameMemory();

		if (ImGui.MenuItem("Save", "Ctrl+S"))
			SaveReplay();

		ImGui.Separator();

		if (ImGui.MenuItem("Inject", "Ctrl+I"))
			InjectReplay();

		ImGui.Separator();

		if (ImGui.MenuItem("Close", "Esc"))
			Close();
	}

	public void NewReplay()
	{
		fileStates.Replay.Update(EditorReplayModel.CreateDefault());
		fileStates.Replay.SetFile(null, null);
		replayEditorWindow.Reset();
	}

	public void OpenReplay()
	{
		nativeFileDialog.CreateOpenFileDialog(OpenReplayCallback, PathUtils.FileExtensionReplay);
	}

	private void OpenReplayCallback(string? filePath)
	{
		if (filePath == null)
			return;

		byte[] fileContents;
		try
		{
			fileContents = File.ReadAllBytes(filePath);
		}
		catch (Exception ex)
		{
			popupManager.ShowError($"Could not open file '{filePath}'.", ex);
			logger.Error(ex, "Could not open file");
			return;
		}

		if (ReplayBinary<LocalReplayBinaryHeader>.TryParse(fileContents, out ReplayBinary<LocalReplayBinaryHeader>? replayBinary))
		{
			fileStates.Replay.Update(EditorReplayModel.CreateFromLocalReplay(replayBinary));
			fileStates.Replay.SetFile(filePath, Path.GetFileName(filePath));
		}
		else
		{
			popupManager.ShowError($"The file '{filePath}' could not be parsed as a local replay.");
			return;
		}

		replayEditorWindow.Reset();

		ReplaySimulation replaySimulation = ReplaySimulationBuilder.Build(replayBinary);
		replayEditor3DWindow.ArenaScene.SetPlayerMovement(replaySimulation);
	}

	public void OpenLeaderboardReplay()
	{
		leaderboardReplayBrowser.Show();
	}

	public void OpenReplayFromGameMemory()
	{
		if (!gameMemoryServiceWrapper.Scan() || !gameMemoryService.IsInitialized)
		{
			popupManager.ShowError(DescribeGameMemoryUnavailable("Could not read replay from game memory."));
			return;
		}

		byte[] replayBytes = gameMemoryService.ReadReplayFromMemory();
		if (ReplayBinary<LocalReplayBinaryHeader>.TryParse(replayBytes, out ReplayBinary<LocalReplayBinaryHeader>? replayBinary))
		{
			fileStates.Replay.Update(EditorReplayModel.CreateFromLocalReplay(replayBinary));
			fileStates.Replay.SetFile(null, "(untitled from game memory)");
		}
		else
		{
			popupManager.ShowError("The data from game memory could not be parsed as a local replay. Make sure to open a replay first.");
			return;
		}

		replayEditorWindow.Reset();

		ReplaySimulation replaySimulation = ReplaySimulationBuilder.Build(replayBinary);
		replayEditor3DWindow.ArenaScene.SetPlayerMovement(replaySimulation);
	}

	public void SaveReplay()
	{
		nativeFileDialog.CreateSaveFileDialog(SaveReplayCallback, PathUtils.FileExtensionReplay);
	}

	private void SaveReplayCallback(string? filePath)
	{
		if (filePath == null)
			return;

		filePath = Path.ChangeExtension(filePath, PathUtils.FileExtensionReplay);
		fileStates.Replay.SaveFile(filePath);
	}

	public void InjectReplay()
	{
		if (!gameMemoryServiceWrapper.Scan() || !gameMemoryService.IsInitialized)
		{
			popupManager.ShowError(DescribeGameMemoryUnavailable("Could not inject replay into game memory."));
			return;
		}

		gameMemoryService.WriteReplayToMemory(fileStates.Replay.Object.ToLocalReplay().Compile());
	}

	/// <summary>
	/// Combines an action-specific prefix with why game memory is unavailable. On macOS, a refusal to read the game's
	/// memory is not the same problem as the game not running, and telling the user to "make sure the game is running"
	/// when it is running and the app just was not launched under sudo would send them looking in the wrong place.
	/// </summary>
	private string DescribeGameMemoryUnavailable(string actionPrefix)
	{
		return gameMemoryService.BlockAddressStatus == BlockAddressStatus.MemoryUnreadable
			? $"{actionPrefix} {gameMemoryServiceWrapper.DescribeUnavailability()}"
			: $"{actionPrefix} Make sure the game is running.";
	}

	public void Close()
	{
		uiLayoutManager.Layout = LayoutType.Main;
	}
}
