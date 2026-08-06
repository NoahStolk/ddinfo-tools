// ReSharper disable ForCanBeConvertedToForeach
using DevilDaggersInfo.Core.Replay.PostProcessing.ReplaySimulation;
using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Scenes.GameObjects;
using DevilDaggersInfo.Tools.Ui;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Utils;
using Silk.NET.GLFW;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Scenes;

/// <summary>
/// State for one arena view. Holds no GPU resources and issues no draw calls; see
/// <see cref="Rendering.ArenaRenderer"/>.
/// </summary>
internal sealed class ArenaScene
{
	public const float MinRenderTileHeight = -3;

	private readonly Func<SpawnsetBinary> _getSpawnset;

	// Scratch buffers for the transparent hitbox pass, reused every frame.
	private readonly Tile[] _hitboxTiles = new Tile[SpawnsetBinary.ArenaDimensionMax * SpawnsetBinary.ArenaDimensionMax];
	private readonly float[] _hitboxSquaredDistances = new float[SpawnsetBinary.ArenaDimensionMax * SpawnsetBinary.ArenaDimensionMax];

	private readonly RaceDagger _raceDagger = new();
	private readonly List<LightObject> _lights = [];

	private Player? _player;

	public unsafe ArenaScene(
		Glfw glfw,
		WindowHandle* window,
		GlfwInput glfwInput,
		Func<SpawnsetBinary> getSpawnset,
		bool useMenuCamera,
		bool isEditor,
		FileStates fileStates,
		SpawnsetSaver spawnsetSaver)
	{
		_getSpawnset = getSpawnset;

		Camera = new Camera(glfw, glfwInput, window, useMenuCamera) { Position = new Vector3(0, 5, 0) };

		InitializeArena();

		if (isEditor)
			EditorContext = new ArenaEditorContext(this, glfwInput, fileStates, spawnsetSaver);
	}

	public Camera Camera { get; }
	public Tile[,] Tiles { get; } = new Tile[SpawnsetBinary.ArenaDimensionMax, SpawnsetBinary.ArenaDimensionMax];
	public int CurrentTick { get; set; }
	public ReplaySimulation? ReplaySimulation { get; private set; }

	public ArenaEditorContext? EditorContext { get; }
	public RaceDagger RaceDagger => _raceDagger;
	public Player? Player => _player;
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

		if (_player != null)
			_lights.Remove(_player.Light);

		_player = new Player(ReplaySimulation);
		_lights.Add(_player.Light);
	}

	/// <summary>
	/// Advances the scene. <paramref name="viewportWidth"/> and <paramref name="viewportHeight"/> are the size the scene
	/// will be rendered at; the camera needs them to build its matrices, which tile picking then depends on.
	/// </summary>
	public void Update(bool activateMouse, bool activateKeyboard, float delta, int viewportWidth, int viewportHeight)
	{
		SpawnsetBinary spawnset = _getSpawnset();

		Camera.Update(activateMouse, activateKeyboard, delta);

		// Must run after Camera.Update and before the editor context picks a tile: it builds the view and projection
		// matrices from the rotation Camera.Update just set, and is the only thing that gives the camera its viewport size.
		Camera.PreRender(viewportWidth, viewportHeight);

		_raceDagger.Update(spawnset, CurrentTick);
		_player?.Update(CurrentTick);

		for (int i = 0; i < Tiles.GetLength(0); i++)
		{
			for (int j = 0; j < Tiles.GetLength(1); j++)
			{
				Tile tile = Tiles[i, j];
				tile.SetDisplayHeight(spawnset.GetActualTileHeight(tile.ArenaX, tile.ArenaY, CurrentTick / 60f));
			}
		}

		EditorContext?.Update(activateMouse, CurrentTick);
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
