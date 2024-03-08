using DevilDaggersInfo.Tools.Ui;
using DevilDaggersInfo.Tools.Ui.AssetEditor;
using DevilDaggersInfo.Tools.Ui.Config;
using DevilDaggersInfo.Tools.Ui.CustomLeaderboards;
using DevilDaggersInfo.Tools.Ui.CustomLeaderboards.Results;
using DevilDaggersInfo.Tools.Ui.Main;
using DevilDaggersInfo.Tools.Ui.MemoryTool;
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

public static class UiRenderer
{
	private static LayoutType _layout;

	private static bool _showDemoWindow;
	private static bool _showAbout;
	private static bool _showUpdate;

	public static LayoutType Layout
	{
		get => _layout;
		set
		{
			_layout = value;
			Colors.SetColors(value switch
			{
				LayoutType.Config or LayoutType.Main => Colors.Main,
				LayoutType.SpawnsetEditor => Colors.SpawnsetEditor,
				LayoutType.AssetEditor => Colors.AssetEditor,
				LayoutType.ReplayEditor => Colors.ReplayEditor,
				LayoutType.CustomLeaderboards => Colors.CustomLeaderboards,
				LayoutType.Practice => Colors.Practice,
				LayoutType.ModManager => Colors.ModManager,
				LayoutType.MemoryTool => Colors.MemoryTool,
				_ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
			});
		}
	}

	public static void ShowDemoWindow()
	{
		_showDemoWindow = true;
	}

	public static void ShowAbout()
	{
		_showAbout = true;
	}

	public static void ShowUpdate()
	{
		_showUpdate = true;
	}

	public static void Render(float delta)
	{
		if (_showDemoWindow)
			ImGuiNET.ImGui.ShowDemoWindow(ref _showDemoWindow);

		switch (Layout)
		{
			case LayoutType.Config:
				ConfigLayout.Render();
				break;
			case LayoutType.Main:
				MainWindow.Render();
				MainScene.Render(delta);
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
			case LayoutType.MemoryTool:
				MemoryToolWindow.Update(delta);
				MemoryToolWindow.Render();
				break;
		}

		if (UserSettings.Model.ShowDebug)
			DebugWindow.Render();

		AboutWindow.Render(ref _showAbout);
		UpdateWindow.Render(ref _showUpdate);

		PopupManager.Render();
	}
}
