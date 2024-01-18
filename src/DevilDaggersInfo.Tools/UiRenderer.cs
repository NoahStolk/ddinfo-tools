using DevilDaggersInfo.Tools.Ui;
using DevilDaggersInfo.Tools.Ui.Config;
using DevilDaggersInfo.Tools.Ui.CustomLeaderboards;
using DevilDaggersInfo.Tools.Ui.CustomLeaderboards.Results;
using DevilDaggersInfo.Tools.Ui.Main;
using DevilDaggersInfo.Tools.Ui.ModManager;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Tools.Ui.Practice.Main;
using DevilDaggersInfo.Tools.Ui.Practice.RunAnalysis;
using DevilDaggersInfo.Tools.Ui.ReplayEditor;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor;
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
				LayoutType.ReplayEditor => Colors.ReplayEditor,
				LayoutType.CustomLeaderboards => Colors.CustomLeaderboards,
				LayoutType.Practice => Colors.Practice,
				LayoutType.ModManager => Colors.ModManager,
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
				SpawnsetEditorWindow.Render();
				SpawnsetEditor3DWindow.Render(delta);
				break;
			case LayoutType.ReplayEditor:
				ReplayEditorWindow.Update(delta);
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
				ModManagerWindow.Render();
				break;
		}

		if (UserSettings.Model.ShowDebug)
			DebugLayout.Render();

		AboutWindow.Render(ref _showAbout);
		UpdateWindow.Render(ref _showUpdate);

		PopupManager.Render();
	}
}
