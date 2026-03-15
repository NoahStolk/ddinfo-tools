using DevilDaggersInfo.Tools.Scenes.GameObjects;
using DevilDaggersInfo.Tools.Ui;
using DevilDaggersInfo.Tools.Ui.CustomLeaderboards;
using DevilDaggersInfo.Tools.Ui.Main;
using DevilDaggersInfo.Tools.Ui.ReplayEditor;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor;
using DevilDaggersInfo.Tools.User.Settings;
using Silk.NET.OpenGL;

namespace DevilDaggersInfo.Tools;

internal sealed class GameInstallationValidator(
	GL gl,
	UiLayoutManager uiLayoutManager,
	ResourceManager resourceManager,
	MainScene mainScene,
	SpawnsetEditor3DWindow spawnsetEditor3DWindow,
	CustomLeaderboards3DWindow customLeaderboards3DWindow,
	ReplayEditor3DWindow replayEditor3DWindow,
	SurvivalFileWatcher survivalFileWatcher,
	UserSettings userSettings,
	ContentManager contentManager)
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
		InstallationDirectoryInput = userSettings.Model.DevilDaggersInstallationDirectory;

		try
		{
			contentManager.Initialize();
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
		Player.InitializeRendering(gl);
		RaceDagger.InitializeRendering(gl);
		Tile.InitializeRendering(gl, resourceManager);
		Skull4.InitializeRendering(gl);

		// Initialize scenes.
		mainScene.Initialize();
		spawnsetEditor3DWindow.InitializeScene();
		customLeaderboards3DWindow.InitializeScene();
		replayEditor3DWindow.InitializeScene();

		// Initialize file watchers.
		survivalFileWatcher.Initialize();

		_contentInitialized = true;
	}
}
