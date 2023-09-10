using DevilDaggersInfo.App.Ui.ReplayEditor.State;
using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.PostProcessing.ReplaySimulation;
using ImGuiNET;

namespace DevilDaggersInfo.App.Ui.ReplayEditor;

public static class ReplayEditorMenu
{
	public static void Render()
	{
		if (ImGui.BeginMenuBar())
		{
			if (ImGui.BeginMenu("File"))
			{
				RenderFileMenu();
				ImGui.EndMenu();
			}

			ImGui.EndMenuBar();
		}
	}

	private static void RenderFileMenu()
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
		ReplayState.ReplayName = "(untitled)";
		ReplayState.Replay = ReplayBinary<LocalReplayBinaryHeader>.CreateDefault();
	}

	public static void OpenReplay()
	{
		string? filePath = NativeFileDialog.CreateOpenFileDialog("Devil Daggers replay files (*.ddreplay)|*.ddreplay");
		if (filePath == null)
			return;

		byte[] fileContents;
		try
		{
			fileContents = File.ReadAllBytes(filePath);
		}
		catch (Exception ex)
		{
			Modals.ShowError($"Could not open file '{filePath}'.");
			Root.Log.Error(ex, "Could not open file");
			return;
		}

		if (ReplayBinary<LocalReplayBinaryHeader>.TryParse(fileContents, out ReplayBinary<LocalReplayBinaryHeader>? replayBinary))
		{
			ReplayState.ReplayName = Path.GetFileName(filePath);
			ReplayState.Replay = replayBinary;
		}
		else
		{
			Modals.ShowError($"The file '{filePath}' could not be parsed as a local replay.");
			return;
		}

		ReplayEditorWindow.Reset();

		ReplaySimulation replaySimulation = ReplaySimulationBuilder.Build(ReplayState.Replay);
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
			Modals.ShowError("Could not read replay from game memory. Make sure the game is running.");
			return;
		}

		byte[] replayBytes = Root.GameMemoryService.ReadReplayFromMemory();
		if (ReplayBinary<LocalReplayBinaryHeader>.TryParse(replayBytes, out ReplayBinary<LocalReplayBinaryHeader>? replayBinary))
		{
			ReplayState.ReplayName = "(untitled from game memory)";
			ReplayState.Replay = replayBinary;
		}
		else
		{
			Modals.ShowError("The data from game memory could not be parsed as a local replay. Make sure to open a replay first.");
			return;
		}

		ReplayEditorWindow.Reset();

		ReplaySimulation replaySimulation = ReplaySimulationBuilder.Build(ReplayState.Replay);
		ReplayEditor3DWindow.ArenaScene.SetPlayerMovement(replaySimulation);
	}

	public static void SaveReplay()
	{
		string? filePath = NativeFileDialog.CreateSaveFileDialog("Devil Daggers replay files (*.ddreplay)|*.ddreplay");
		if (filePath != null)
			File.WriteAllBytes(filePath, ReplayState.Replay.Compile());
	}

	public static void InjectReplay()
	{
		if (!GameMemoryServiceWrapper.Scan() || !Root.GameMemoryService.IsInitialized)
			return;

		Root.GameMemoryService.WriteReplayToMemory(ReplayState.Replay.Compile());
	}

	public static void Close()
	{
		UiRenderer.Layout = LayoutType.Main;
	}
}
