using DevilDaggersInfo.Tools.Engine;
using DevilDaggersInfo.Tools.Scenes.Rendering;
using DevilDaggersInfo.Tools.Ui;
using DevilDaggersInfo.Tools.Ui.CustomLeaderboards;
using DevilDaggersInfo.Tools.Ui.Main;
using DevilDaggersInfo.Tools.Ui.ReplayEditor;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor;
using DevilDaggersInfo.Tools.User.Settings;

namespace DevilDaggersInfo.Tools;

internal sealed class GameInstallationValidator(
	UiLayoutManager uiLayoutManager,
	MeshCache meshCache,
	ResourceManager resourceManager,
	ArenaRenderer arenaRenderer,
	MainScene mainScene,
	SpawnsetEditor3DWindow spawnsetEditor3DWindow,
	CustomLeaderboards3DWindow customLeaderboards3DWindow,
	ReplayEditor3DWindow replayEditor3DWindow,
	UserSettings userSettings,
	ContentManager contentManager,
	SurvivalFileWatcher survivalFileWatcher)
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

		// ContentManager.Initialize builds fresh content objects every time, so the GPU-side resources must be rebuilt
		// every time too. Skipping this left the meshes and textures belonging to the previously loaded installation.
		meshCache.Clear();
		arenaRenderer.InvalidateMeshes();
		resourceManager.LoadGameResources();
		arenaRenderer.WarmUpMeshes();

		if (_contentInitialized)
			return;

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
