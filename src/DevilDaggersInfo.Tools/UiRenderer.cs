using DevilDaggersInfo.Tools.Ui;
using DevilDaggersInfo.Tools.Ui.AssetEditor;
using DevilDaggersInfo.Tools.Ui.Config;
using DevilDaggersInfo.Tools.Ui.CustomLeaderboards;
using DevilDaggersInfo.Tools.Ui.CustomLeaderboards.Results;
using DevilDaggersInfo.Tools.Ui.Main;
using DevilDaggersInfo.Tools.Ui.ModManager;
using DevilDaggersInfo.Tools.Ui.ModManager.ModsDirectory;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Tools.Ui.Practice.Main;
using DevilDaggersInfo.Tools.Ui.Practice.RunAnalysis;
using DevilDaggersInfo.Tools.Ui.ReplayEditor;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Arena;
using DevilDaggersInfo.Tools.User.Settings;

namespace DevilDaggersInfo.Tools;

internal sealed class UiRenderer(
	UiLayoutManager uiLayoutManager,
	ConfigLayout configLayout,
	MainScene mainScene,
	MainWindow mainWindow,
	DebugWindow debugWindow,
	AboutWindow aboutWindow,
	AssetEditorMenu assetEditorMenu,
	ReplayEditorMenu replayEditorMenu,
	SpawnsetEditorMenu spawnsetEditorMenu,
	ArenaWindow arenaWindow,
	SpawnsetEditor3DWindow spawnsetEditor3DWindow,
	PracticeWindow practiceWindow,
	CustomLeaderboardsWindow customLeaderboardsWindow,
	CustomLeaderboards3DWindow customLeaderboards3DWindow,
	ReplayEditorWindow replayEditorWindow,
	ReplayEditor3DWindow replayEditor3DWindow)
{
	public void Render(float delta)
	{
		if (debugWindow.ShowDemoWindow)
			ImGuiNET.ImGui.ShowDemoWindow(ref debugWindow.ShowDemoWindow);

		switch (uiLayoutManager.Layout)
		{
			case LayoutType.Config:
				configLayout.Render();
				break;
			case LayoutType.Main:
				mainWindow.Render();
				mainScene.Render(delta);
				break;
			case LayoutType.SpawnsetEditor:
				spawnsetEditorMenu.Render();
				SpawnsWindow.Render();
				SettingsWindow.Render();
				arenaWindow.Render();
				HistoryWindow.Render();
				spawnsetEditor3DWindow.Render(delta);
				break;
			case LayoutType.AssetEditor:
				assetEditorMenu.Render();
				AssetEditorWindow.Render();
				CompileModWindow.Render();
				ExtractModWindow.Render();
				break;
			case LayoutType.ReplayEditor:
				replayEditorWindow.Update(delta);
				replayEditorMenu.Render();
				replayEditorWindow.Render();
				replayEditor3DWindow.Render(delta);
				LeaderboardReplayBrowser.Render();
				break;
			case LayoutType.CustomLeaderboards:
				customLeaderboardsWindow.Update(delta);
				customLeaderboardsWindow.Render();
				CustomLeaderboardResultsWindow.Render();
				customLeaderboards3DWindow.Render(delta);
				break;
			case LayoutType.Practice:
				practiceWindow.Render();
				RunAnalysisWindow.Update(delta);
				RunAnalysisWindow.Render();
				break;
			case LayoutType.ModManager:
				ModsDirectoryWindow.Render();
				ModPreviewWindow.Render();
				ModInstallationWindow.Render();
				break;
		}

		if (UserSettings.Model.ShowDebug)
			debugWindow.Render();

		aboutWindow.Render();
		UpdateWindow.Render();

		PopupManager.Render();
	}
}
