using DevilDaggersInfo.App.Ui;
using DevilDaggersInfo.App.Ui.Config;
using DevilDaggersInfo.App.Ui.CustomLeaderboards;
using DevilDaggersInfo.App.Ui.CustomLeaderboards.Results;
using DevilDaggersInfo.App.Ui.Main;
using DevilDaggersInfo.App.Ui.Practice.Main;
using DevilDaggersInfo.App.Ui.ReplayEditor;
using DevilDaggersInfo.App.Ui.SpawnsetEditor;
using DevilDaggersInfo.App.User.Settings;

namespace DevilDaggersInfo.App;

public static class UiRenderer
{
	private static LayoutType _layout;

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
		//ImGuiNET.ImGui.ShowUserGuide();
		//ImGuiNET.ImGui.ShowDemoWindow();

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
				break;
			case LayoutType.Practice:
				PracticeWindow.Render();

				// TODO: Fix hardcoded 350 in SplitsChild. Fix graphs in GemCollectionChild.
				// RunAnalysisWindow.Update(delta);
				// RunAnalysisWindow.Render();
				break;
		}

		if (UserSettings.Model.ShowDebug)
			DebugLayout.Render();

		SettingsWindow.Render(ref _showSettings);
		AboutWindow.Render(ref _showAbout);
		UpdateWindow.Render(ref _showUpdateAvailable);

		Modals.Render();
	}
}
