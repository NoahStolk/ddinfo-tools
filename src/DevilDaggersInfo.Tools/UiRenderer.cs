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

	private bool _showDemoWindow;
	private bool _showAbout;
	private bool _showUpdate;

	public UiRenderer(UiLayoutManager uiLayoutManager, ConfigLayout configLayout, MainScene mainScene, MainWindow mainWindow, DebugWindow debugWindow)
	{
		_uiLayoutManager = uiLayoutManager;
		_configLayout = configLayout;
		_mainScene = mainScene;
		_mainWindow = mainWindow;
		_debugWindow = debugWindow;
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
				SpawnsetEditorMenu.Render();
				SpawnsWindow.Render();
				SettingsWindow.Render();
				ArenaWindow.Render();
				HistoryWindow.Render();
				SpawnsetEditor3DWindow.Render(delta);
				break;
			case LayoutType.AssetEditor:
				AssetEditorMenu.Render();
				AssetEditorWindow.Render();
				CompileModWindow.Render();
				ExtractModWindow.Render();
				break;
			case LayoutType.ReplayEditor:
				ReplayEditorWindow.Update(delta);
				ReplayEditorMenu.Render();
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

		AboutWindow.Render(ref _showAbout);
		UpdateWindow.Render(ref _showUpdate);

		PopupManager.Render();
	}
}
