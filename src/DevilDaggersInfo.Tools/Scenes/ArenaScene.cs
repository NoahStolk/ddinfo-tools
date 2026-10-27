using DevilDaggersInfo.Core.Replay.PostProcessing.ReplaySimulation;
using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Scenes.GameObjects;
using DevilDaggersInfo.Tools.Ui;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Utils;
using DevilDaggersInfo.Tools.User.Settings;
using Silk.NET.GLFW;
using System.Numerics;

// ReSharper disable ForCanBeConvertedToForeach
namespace DevilDaggersInfo.Tools.Scenes;

/// <summary>
/// State for one arena view. Holds no GPU resources and issues no draw calls; see
/// <see cref="Rendering.ArenaRenderer"/>.
/// </summary>
internal sealed class ArenaScene
{
	public const float MinRenderTileHeight = -3;

	private readonly Func<SpawnsetBinary> _getSpawnset;
	private readonly bool _useMenuCamera;

	// Scratch buffers for the transparent hitbox pass, reused every frame.
	private readonly Tile[] _hitboxTiles = new Tile[SpawnsetBinary.ArenaDimensionMax * SpawnsetBinary.ArenaDimensionMax];
	private readonly float[] _hitboxSquaredDistances = new float[SpawnsetBinary.ArenaDimensionMax * SpawnsetBinary.ArenaDimensionMax];

	private readonly List<LightObject> _lights = [];

	private float _totalTime;

	public unsafe ArenaScene(
		Glfw glfw,
		WindowHandle* window,
		GlfwInput glfwInput,
		Func<SpawnsetBinary> getSpawnset,
		bool useMenuCamera,
		bool isEditor,
		FileStates fileStates,
		SpawnsetSaver spawnsetSaver,
		UserSettings userSettings)
	{
		_getSpawnset = getSpawnset;
		_useMenuCamera = useMenuCamera;

		Camera = new Camera(glfw, glfwInput, window) { Position = new Vector3(0, 5, 0) };

		InitializeArena();

		if (isEditor)
			EditorContext = new ArenaEditorContext(this, glfwInput, fileStates, spawnsetSaver, userSettings);
	}

	public Camera Camera { get; }
	public Tile[,] Tiles { get; } = new Tile[SpawnsetBinary.ArenaDimensionMax, SpawnsetBinary.ArenaDimensionMax];
	public int CurrentTick { get; set; }
	public ReplaySimulation? ReplaySimulation { get; private set; }

	public ArenaEditorContext? EditorContext { get; }
	public RaceDagger RaceDagger { get; } = new();

	public Player? Player { get; private set; }

	public IReadOnlyList<LightObject> Lights => _lights;
	public bool ShowSkull4 { get; private set; }

	private void InitializeArena()
	{
		const int halfSize = SpawnsetBinary.ArenaDimensionMax / 2;
		for (int i = 0; i < SpawnsetBinary.ArenaDimensionMax; i++)
		{
			for (int j = 0; j < SpawnsetBinary.ArenaDimensionMax; j++)
			{
				float x = (i - halfSize) * 4;
				float z = (j - halfSize) * 4;
				Tiles[i, j] = new Tile(x, z, i, j, Camera);
			}
		}

		_lights.Add(new LightObject(64, default, new Vector3(1, 0.5f, 0)));
	}

	public void AddSkull4()
	{
		ShowSkull4 = true;
	}

	public void SetPlayerMovement(ReplaySimulation replaySimulation)
	{
		ReplaySimulation = replaySimulation;

		if (Player != null)
			_lights.Remove(Player.Light);

		Player = new Player(ReplaySimulation);
		_lights.Add(Player.Light);
	}

	public void Update(bool activateMouse, bool activateKeyboard, float dt, Vector2 framebufferOffset, Vector2 canvasSize)
	{
		SpawnsetBinary spawnset = _getSpawnset();

		if (_useMenuCamera)
		{
			_totalTime += dt;
			Camera.SetMenuCamera(_totalTime);
		}
		else
		{
			Camera.Update(dt, activateMouse || activateKeyboard);
		}

		RaceDagger.Update(spawnset, CurrentTick);
		Player?.Update(CurrentTick);

		for (int i = 0; i < Tiles.GetLength(0); i++)
		{
			for (int j = 0; j < Tiles.GetLength(1); j++)
			{
				Tile tile = Tiles[i, j];
				tile.SetDisplayHeight(spawnset.GetActualTileHeight(tile.ArenaX, tile.ArenaY, CurrentTick / 60f));
			}
		}

		EditorContext?.Update(activateMouse, CurrentTick, framebufferOffset, canvasSize);
	}

	/// <summary>
	/// Fills a scratch buffer with the tiles whose hitboxes are worth drawing, sorted nearest first, and returns how many
	/// were collected. The renderer walks the range backwards to draw them back to front.
	/// </summary>
	public int CollectVisibleHitboxTiles(out Tile[] tiles)
	{
		tiles = _hitboxTiles;

		int count = 0;
		for (int i = 0; i < Tiles.GetLength(0); i++)
		{
			for (int j = 0; j < Tiles.GetLength(1); j++)
			{
				Tile tile = Tiles[i, j];
				if (!tile.IsHitboxVisible)
					continue;

				_hitboxTiles[count] = tile;
				_hitboxSquaredDistances[count] = tile.SquaredDistanceToCamera();
				count++;
			}
		}

		if (count > 0)
			Array.Sort(_hitboxSquaredDistances, _hitboxTiles, 0, count);

		return count;
	}
}
