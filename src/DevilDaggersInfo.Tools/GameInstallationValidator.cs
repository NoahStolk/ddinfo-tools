using DevilDaggersInfo.Tools.Scenes;
using DevilDaggersInfo.Tools.Ui;
using DevilDaggersInfo.Tools.Ui.CustomLeaderboards;
using DevilDaggersInfo.Tools.Ui.Main;
using DevilDaggersInfo.Tools.Ui.ReplayEditor;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor;
using DevilDaggersInfo.Tools.User.Settings;

namespace DevilDaggersInfo.Tools;

internal sealed class GameInstallationValidator(
	UiLayoutManager uiLayoutManager,
	ResourceManager resourceManager,
	MainScene mainScene,
	SpawnsetEditor3DWindow spawnsetEditor3DWindow,
	CustomLeaderboards3DWindow customLeaderboards3DWindow,
	ReplayEditor3DWindow replayEditor3DWindow,
	ArenaSceneRendering arenaSceneRendering)
{
	private bool _contentInitialized;

	public string InstallationDirectoryInput = string.Empty;

	public string? Error { get; private set; }

	/// <summary>
	/// Is called on launch, and when the user changes the installation directory.
	/// Must be called on the main thread.
	/// </summary>
	public void ValidateInstallation()
	{
		InstallationDirectoryInput = UserSettings.Model.DevilDaggersInstallationDirectory;

		try
		{
			ContentManager.Initialize();
		}
		catch (InvalidGameInstallationException ex)
		{
			Error = ex.Message;
			return;
		}

		uiLayoutManager.Layout = LayoutType.Main;
		Error = null;

		if (_contentInitialized)
			return;

		// Initialize game resources.
		resourceManager.LoadGameResources();

		// Initialize 3D rendering.
		arenaSceneRendering.InitializeRendering();

		// Initialize scenes.
		mainScene.Initialize();
		spawnsetEditor3DWindow.InitializeScene();
		customLeaderboards3DWindow.InitializeScene();
		replayEditor3DWindow.InitializeScene();

		// Initialize file watchers.
		SurvivalFileWatcher.Initialize();

		_contentInitialized = true;
	}
}
