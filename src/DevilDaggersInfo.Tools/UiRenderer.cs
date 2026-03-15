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
	PopupManager popupManager,
	MainWindow mainWindow,
	DebugWindow debugWindow,
	AboutWindow aboutWindow,

	SpawnsetEditorMenu spawnsetEditorMenu,
	SpawnsetEditor3DWindow spawnsetEditor3DWindow,
	ArenaWindow arenaWindow,
	SpawnsWindow spawnsWindow,
	SettingsWindow settingsWindow,
	HistoryWindow historyWindow,

	PracticeWindow practiceWindow,
	RunAnalysisWindow runAnalysisWindow,

	CustomLeaderboardsWindow customLeaderboardsWindow,
	CustomLeaderboards3DWindow customLeaderboards3DWindow,

	ReplayEditorMenu replayEditorMenu,
	ReplayEditorWindow replayEditorWindow,
	ReplayEditor3DWindow replayEditor3DWindow,
	LeaderboardReplayBrowser leaderboardReplayBrowser,

	AssetEditorMenu assetEditorMenu,
	AssetEditorWindow assetEditorWindow,
	CompileModWindow compileModWindow,
	ExtractModWindow extractModWindow,

	ModsDirectoryWindow modsDirectoryWindow,
	ModPreviewWindow modPreviewWindow,
	ModInstallationWindow modInstallationWindow)
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
				spawnsWindow.Render();
				settingsWindow.Render();
				arenaWindow.Render();
				historyWindow.Render();
				spawnsetEditor3DWindow.Render(delta);
				break;
			case LayoutType.AssetEditor:
				assetEditorMenu.Render();
				assetEditorWindow.Render();
				compileModWindow.Render();
				extractModWindow.Render();
				break;
			case LayoutType.ReplayEditor:
				replayEditorWindow.Update(delta);

				replayEditorMenu.Render();
				replayEditorWindow.Render();
				replayEditor3DWindow.Render(delta);
				leaderboardReplayBrowser.Render();
				break;
			case LayoutType.CustomLeaderboards:
				customLeaderboardsWindow.Update(delta);
				customLeaderboardsWindow.Render();
				CustomLeaderboardResultsWindow.Render();
				customLeaderboards3DWindow.Render(delta);
				break;
			case LayoutType.Practice:
				practiceWindow.Render();
				runAnalysisWindow.Update(delta);
				runAnalysisWindow.Render();
				break;
			case LayoutType.ModManager:
				modsDirectoryWindow.Render();
				modPreviewWindow.Render();
				modInstallationWindow.Render();
				break;
		}

		if (UserSettings.Model.ShowDebug)
			debugWindow.Render();

		aboutWindow.Render();
		UpdateWindow.Render();

		popupManager.Render();
	}
}
