using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.PostProcessing.ReplaySimulation;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor;

public sealed class ReplayEditorMenu
{
	private readonly UiLayoutManager _uiLayoutManager;

	public ReplayEditorMenu(UiLayoutManager uiLayoutManager)
	{
		_uiLayoutManager = uiLayoutManager;
	}

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

	public static void NewReplay()
	{
		FileStates.Replay.Update(EditorReplayModel.CreateDefault());
		FileStates.Replay.SetFile(null, null);
		ReplayEditorWindow.Reset();
	}

	public static void OpenReplay()
	{
		NativeFileDialog.CreateOpenFileDialog(OpenReplayCallback, PathUtils.FileExtensionReplay);
	}

	private static void OpenReplayCallback(string? filePath)
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
			PopupManager.ShowError($"Could not open file '{filePath}'.", ex);
			Root.Log.Error(ex, "Could not open file");
			return;
		}

		if (ReplayBinary<LocalReplayBinaryHeader>.TryParse(fileContents, out ReplayBinary<LocalReplayBinaryHeader>? replayBinary))
		{
			FileStates.Replay.Update(EditorReplayModel.CreateFromLocalReplay(replayBinary));
			FileStates.Replay.SetFile(filePath, Path.GetFileName(filePath));
		}
		else
		{
			PopupManager.ShowError($"The file '{filePath}' could not be parsed as a local replay.");
			return;
		}

		ReplayEditorWindow.Reset();

		ReplaySimulation replaySimulation = ReplaySimulationBuilder.Build(replayBinary);
		ReplayEditor3DWindow.ArenaScene.SetPlayerMovement(replaySimulation);
	}

	public static void OpenLeaderboardReplay()
	{
		LeaderboardReplayBrowser.Show();
	}

	public static void OpenReplayFromGameMemory()
	{
		if (!GameMemoryServiceWrapper.Scan() || !Root.GameMemoryService.IsInitialized)
		{
			PopupManager.ShowError("Could not read replay from game memory. Make sure the game is running.");
			return;
		}

		byte[] replayBytes = Root.GameMemoryService.ReadReplayFromMemory();
		if (ReplayBinary<LocalReplayBinaryHeader>.TryParse(replayBytes, out ReplayBinary<LocalReplayBinaryHeader>? replayBinary))
		{
			FileStates.Replay.Update(EditorReplayModel.CreateFromLocalReplay(replayBinary));
			FileStates.Replay.SetFile(null, "(untitled from game memory)");
		}
		else
		{
			PopupManager.ShowError("The data from game memory could not be parsed as a local replay. Make sure to open a replay first.");
			return;
		}

		ReplayEditorWindow.Reset();

		ReplaySimulation replaySimulation = ReplaySimulationBuilder.Build(replayBinary);
		ReplayEditor3DWindow.ArenaScene.SetPlayerMovement(replaySimulation);
	}

	public static void SaveReplay()
	{
		NativeFileDialog.CreateSaveFileDialog(SaveReplayCallback, PathUtils.FileExtensionReplay);
	}

	private static void SaveReplayCallback(string? filePath)
	{
		if (filePath == null)
			return;

		filePath = Path.ChangeExtension(filePath, PathUtils.FileExtensionReplay);
		FileStates.Replay.SaveFile(filePath);
	}

	public static void InjectReplay()
	{
		if (!GameMemoryServiceWrapper.Scan() || !Root.GameMemoryService.IsInitialized)
		{
			PopupManager.ShowError("Could not inject replay into game memory. Make sure the game is running.");
			return;
		}

		Root.GameMemoryService.WriteReplayToMemory(FileStates.Replay.Object.ToLocalReplay().Compile());
	}

	public void Close()
	{
		_uiLayoutManager.Layout = LayoutType.Main;
	}
}
