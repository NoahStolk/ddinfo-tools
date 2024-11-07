using DevilDaggersInfo.Tools.Scenes.GameObjects;
using DevilDaggersInfo.Tools.Ui;
using DevilDaggersInfo.Tools.Ui.CustomLeaderboards;
using DevilDaggersInfo.Tools.Ui.Main;
using DevilDaggersInfo.Tools.Ui.ReplayEditor;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor;
using Silk.NET.OpenGL;

namespace DevilDaggersInfo.Tools;

public sealed class GameInstallationValidator
{
	private readonly GL _gl;
	private readonly UiLayoutManager _uiLayoutManager;
	private readonly ResourceManager _resourceManager;
	private readonly MainScene _mainScene;
	private readonly SpawnsetEditor3DWindow _spawnsetEditor3DWindow;
	private readonly CustomLeaderboards3DWindow _customLeaderboards3DWindow;
	private readonly ReplayEditor3DWindow _replayEditor3DWindow;

	private bool _contentInitialized;

	public GameInstallationValidator(GL gl, UiLayoutManager uiLayoutManager, ResourceManager resourceManager, MainScene mainScene, SpawnsetEditor3DWindow spawnsetEditor3DWindow, CustomLeaderboards3DWindow customLeaderboards3DWindow, ReplayEditor3DWindow replayEditor3DWindow)
	{
		_gl = gl;
		_uiLayoutManager = uiLayoutManager;
		_resourceManager = resourceManager;
		_mainScene = mainScene;
		_spawnsetEditor3DWindow = spawnsetEditor3DWindow;
		_customLeaderboards3DWindow = customLeaderboards3DWindow;
		_replayEditor3DWindow = replayEditor3DWindow;
	}

	public string? Error { get; private set; }

	/// <summary>
	/// Is called on launch, and when the user changes the installation directory.
	/// Must be called on the main thread.
	/// </summary>
	public void ValidateInstallation()
	{
		// TODO: Move this field (which is in ConfigLayout) to a state class or something.
		// _installationDirectoryInput = UserSettings.Model.DevilDaggersInstallationDirectory;

		try
		{
			ContentManager.Initialize();
		}
		catch (InvalidGameInstallationException ex)
		{
			Error = ex.Message;
			return;
		}

		_uiLayoutManager.Layout = LayoutType.Main;
		Error = null;

		if (_contentInitialized)
			return;

		// Initialize game resources.
		_resourceManager.LoadGameResources();

		// Initialize 3D rendering.
		Player.InitializeRendering(_gl);
		RaceDagger.InitializeRendering(_gl);
		Tile.InitializeRendering(_gl, _resourceManager);
		// Skull4.InitializeRendering();

		// Initialize scenes.
		_mainScene.Initialize();
		_spawnsetEditor3DWindow.InitializeScene();
		_customLeaderboards3DWindow.InitializeScene();
		_replayEditor3DWindow.InitializeScene();

		// Initialize file watchers.
		SurvivalFileWatcher.Initialize();

		_contentInitialized = true;
	}
}
