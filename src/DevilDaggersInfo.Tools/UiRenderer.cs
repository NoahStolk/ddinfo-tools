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

public sealed class UiRenderer
{
	private readonly UiLayoutManager _uiLayoutManager;
	private readonly ConfigLayout _configLayout;
	private readonly MainScene _mainScene;

	private readonly MainWindow _mainWindow;
	private readonly DebugWindow _debugWindow;
	private readonly AboutWindow _aboutWindow;

	private readonly AssetEditorMenu _assetEditorMenu;
	private readonly ReplayEditorMenu _replayEditorMenu;
	private readonly SpawnsetEditorMenu _spawnsetEditorMenu;

	private bool _showDemoWindow;
	private bool _showAbout;
	private bool _showUpdate;

	public UiRenderer(
		UiLayoutManager uiLayoutManager,
		ConfigLayout configLayout,
		MainScene mainScene,
		MainWindow mainWindow,
		DebugWindow debugWindow,
		AboutWindow aboutWindow,
		AssetEditorMenu assetEditorMenu,
		ReplayEditorMenu replayEditorMenu,
		SpawnsetEditorMenu spawnsetEditorMenu)
	{
		_uiLayoutManager = uiLayoutManager;
		_configLayout = configLayout;
		_mainScene = mainScene;
		_mainWindow = mainWindow;
		_debugWindow = debugWindow;
		_aboutWindow = aboutWindow;
		_assetEditorMenu = assetEditorMenu;
		_replayEditorMenu = replayEditorMenu;
		_spawnsetEditorMenu = spawnsetEditorMenu;
	}

	public void ShowDemoWindow()
	{
		_showDemoWindow = true;
	}

	public void ShowAbout()
	{
		_showAbout = true;
	}

	public void ShowUpdate()
	{
		_showUpdate = true;
	}

	public void Render(float delta)
	{
		if (_showDemoWindow)
			ImGuiNET.ImGui.ShowDemoWindow(ref _showDemoWindow);

		switch (_uiLayoutManager.Layout)
		{
			case LayoutType.Config:
				_configLayout.Render();
				break;
			case LayoutType.Main:
				_mainWindow.Render();
				_mainScene.Render(delta);
				break;
			case LayoutType.SpawnsetEditor:
				_spawnsetEditorMenu.Render();
				SpawnsWindow.Render();
				SettingsWindow.Render();
				ArenaWindow.Render();
				HistoryWindow.Render();
				SpawnsetEditor3DWindow.Render(delta);
				break;
			case LayoutType.AssetEditor:
				_assetEditorMenu.Render();
				AssetEditorWindow.Render();
				CompileModWindow.Render();
				ExtractModWindow.Render();
				break;
			case LayoutType.ReplayEditor:
				ReplayEditorWindow.Update(delta);
				_replayEditorMenu.Render();
				ReplayEditorWindow.Render();
				ReplayEditor3DWindow.Render(delta);
				LeaderboardReplayBrowser.Render();
				break;
			case LayoutType.CustomLeaderboards:
				CustomLeaderboardsWindow.Update(delta);
				CustomLeaderboardsWindow.Render();
				CustomLeaderboardResultsWindow.Render();
				CustomLeaderboards3DWindow.Render(delta);
				break;
			case LayoutType.Practice:
				PracticeWindow.Render();
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
			_debugWindow.Render();

		_aboutWindow.Render(ref _showAbout);
		UpdateWindow.Render(ref _showUpdate);

		PopupManager.Render();
	}
}
