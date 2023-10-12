using DevilDaggersInfo.Tools.Ui;
using DevilDaggersInfo.Tools.Ui.Config;
using DevilDaggersInfo.Tools.Ui.CustomLeaderboards;
using DevilDaggersInfo.Tools.Ui.CustomLeaderboards.Results;
using DevilDaggersInfo.Tools.Ui.Main;
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
	private static bool _showSettings;
	private static bool _showAbout;
	private static bool _showUpdateAvailable;

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
				LayoutType.CustomLeaderboards => Colors.CustomLeaderboards,
				LayoutType.ReplayEditor => Colors.ReplayEditor,
				LayoutType.Practice => Colors.Practice,
				_ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
			});
		}
	}

	public static void ShowDemoWindow()
	{
		_showDemoWindow = true;
	}

	public static void ShowSettings()
	{
		_showSettings = true;
	}

	public static void ShowAbout()
	{
		_showAbout = true;
	}

	public static void ShowUpdateAvailable()
	{
		_showUpdateAvailable = true;
	}

	public static void Render(float delta)
	{
		if (_showDemoWindow)
			ImGuiNET.ImGui.ShowDemoWindow(ref _showDemoWindow);

		switch (Layout)
		{
			case LayoutType.Main:
				MainWindow.Render();
				MainScene.Render(delta);
				break;
			case LayoutType.Config:
				ConfigLayout.Render();
				break;
			case LayoutType.SpawnsetEditor:
				SpawnsetEditorWindow.Render();
				SpawnsetEditor3DWindow.Render(delta);
				break;
			case LayoutType.CustomLeaderboards:
				CustomLeaderboardsWindow.Update(delta);
				CustomLeaderboardsWindow.Render();
				CustomLeaderboardResultsWindow.Render();
				CustomLeaderboards3DWindow.Render(delta);
				break;
			case LayoutType.ReplayEditor:
				ReplayEditorWindow.Update(delta);
				ReplayEditorWindow.Render();
				ReplayEditor3DWindow.Render(delta);
				LeaderboardReplayBrowser.Render();
				break;
			case LayoutType.Practice:
				PracticeWindow.Render();
				RunAnalysisWindow.Update(delta);
				RunAnalysisWindow.Render();
				break;
		}

		if (UserSettings.Model.ShowDebug)
			DebugLayout.Render();

		SettingsWindow.Render(ref _showSettings);
		AboutWindow.Render(ref _showAbout);
		UpdateWindow.Render(ref _showUpdateAvailable);

		PopupManager.Render();
	}
}
