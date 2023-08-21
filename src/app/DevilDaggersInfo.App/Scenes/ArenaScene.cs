// ReSharper disable ForCanBeConvertedToForeach
using DevilDaggersInfo.App.Scenes.GameObjects;
using DevilDaggersInfo.Core.Replay.PostProcessing.ReplaySimulation;
using DevilDaggersInfo.Core.Spawnset;
using Silk.NET.OpenGL;
using System.Numerics;

namespace DevilDaggersInfo.App.Scenes;

public sealed class ArenaScene
{
	public const float MinRenderTileHeight = -3;

	private readonly Tile[] _sortedTiles = new Tile[SpawnsetBinary.ArenaDimensionMax * SpawnsetBinary.ArenaDimensionMax];

	private readonly RaceDagger _raceDagger = new();
	private readonly List<LightObject> _lights = new();

	private readonly ArenaEditorContext? _editorContext;
	private readonly Func<SpawnsetBinary> _getSpawnset;
	private Player? _player;
	private Skull4? _skull4;

	public ArenaScene(Func<SpawnsetBinary> getSpawnset, bool useMenuCamera, bool isEditor)
	{
		_getSpawnset = getSpawnset;

		Camera = new(useMenuCamera) { Position = new(0, 5, 0) };

		InitializeArena();

		if (isEditor)
			_editorContext = new(this);
	}

	public Camera Camera { get; }
	public Tile[,] Tiles { get; } = new Tile[SpawnsetBinary.ArenaDimensionMax, SpawnsetBinary.ArenaDimensionMax];
	public int CurrentTick { get; set; }
	public ReplaySimulation? ReplaySimulation { get; private set; }

	private void InitializeArena()
	{
		const int halfSize = SpawnsetBinary.ArenaDimensionMax / 2;
		for (int i = 0; i < SpawnsetBinary.ArenaDimensionMax; i++)
		{
			for (int j = 0; j < SpawnsetBinary.ArenaDimensionMax; j++)
			{
				float x = (i - halfSize) * 4;
				float z = (j - halfSize) * 4;
				Tiles[i, j] = new(x, z, i, j, Camera);
				_sortedTiles[i * SpawnsetBinary.ArenaDimensionMax + j] = Tiles[i, j];
			}
		}

		_lights.Add(new(64, default, new(1, 0.5f, 0)));
	}

	private void FillArena(SpawnsetBinary spawnset)
	{
		for (int i = 0; i < spawnset.ArenaDimension; i++)
		{
			for (int j = 0; j < spawnset.ArenaDimension; j++)
				Tiles[i, j].SetDisplayHeight(spawnset.ArenaTiles[i, j]);
		}
	}

	public void AddSkull4()
	{
		_skull4 = new();
	}

	public void SetPlayerMovement(ReplaySimulation replaySimulation)
	{
		ReplaySimulation = replaySimulation;

		if (_player != null)
			_lights.Remove(_player.Light);

		_player = new(ReplaySimulation);
		_lights.Add(_player.Light);
	}

	public void Update(bool activateMouse, bool activateKeyboard, float delta)
	{
		SpawnsetBinary spawnset = _getSpawnset();
		FillArena(spawnset);

		Camera.Update(activateMouse, activateKeyboard, delta);
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

		_editorContext?.Update(activateMouse, CurrentTick);
	}

	public void Render(int windowWidth, int windowHeight)
	{
		Camera.PreRender(windowWidth, windowHeight);

		Shader shader = Root.InternalResources.MeshShader;
		shader.Use();
		shader.SetUniform("view", Camera.ViewMatrix);
		shader.SetUniform("projection", Camera.Projection);
		shader.SetUniform("textureDiffuse", 0);
		shader.SetUniform("textureLut", 1);
		shader.SetUniform("lutScale", 1f);

		Span<Vector3> lightPositions = stackalloc Vector3[_lights.Count];
		Span<Vector3> lightColors = stackalloc Vector3[_lights.Count];
		Span<float> lightRadii = stackalloc float[_lights.Count];
		for (int i = 0; i < _lights.Count; i++)
		{
			lightPositions[i] = _lights[i].Position;
			lightColors[i] = _lights[i].Color;
			lightRadii[i] = _lights[i].Radius;
		}

		shader.SetUniform("lightCount", lightPositions.Length);
		shader.SetUniform("lightPosition", lightPositions);
		shader.SetUniform("lightColor", lightColors);
		shader.SetUniform("lightRadius", lightRadii);

		Root.GameResources.PostLut.Bind(TextureUnit.Texture1);

		if (_editorContext != null && CurrentTick == 0)
			_editorContext.RenderTiles(shader);
		else
			RenderTilesDefault();

		_raceDagger.Render();
		_player?.Render();
		_skull4?.Render();

		Root.InternalResources.TileHitboxTexture.Bind();

		Array.Sort(_sortedTiles, static (a, b) => a.SquaredDistanceToCamera().CompareTo(b.SquaredDistanceToCamera()));
		for (int i = 0; i < _sortedTiles.Length; i++)
		{
			Tile tile = _sortedTiles[i];
			tile.RenderHitbox();
		}
	}

	private void RenderTilesDefault()
	{
		Root.GameResources.TileTexture.Bind();

		for (int i = 0; i < Tiles.GetLength(0); i++)
		{
			for (int j = 0; j < Tiles.GetLength(1); j++)
				Tiles[i, j].RenderTop();
		}

		Root.GameResources.PillarTexture.Bind();

		for (int i = 0; i < Tiles.GetLength(0); i++)
		{
			for (int j = 0; j < Tiles.GetLength(1); j++)
				Tiles[i, j].RenderPillar();
		}
	}
}
