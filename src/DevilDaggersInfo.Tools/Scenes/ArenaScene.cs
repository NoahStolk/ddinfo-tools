// ReSharper disable ForCanBeConvertedToForeach
using DevilDaggersInfo.Core.Replay.PostProcessing.ReplaySimulation;
using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.Engine;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Scenes.GameObjects;
using Silk.NET.OpenGL;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Scenes;

public sealed class ArenaScene
{
	public const float MinRenderTileHeight = -3;

	private readonly Tile[] _sortedTiles = new Tile[SpawnsetBinary.ArenaDimensionMax * SpawnsetBinary.ArenaDimensionMax];

	private readonly RaceDagger _raceDagger = new();
	private readonly List<LightObject> _lights = [];

	private readonly ArenaEditorContext? _editorContext;
	private readonly Func<SpawnsetBinary> _getSpawnset;
	private Player? _player;
	private Skull4? _skull4;

	public ArenaScene(Func<SpawnsetBinary> getSpawnset, bool useMenuCamera, bool isEditor)
	{
		_getSpawnset = getSpawnset;

		Camera = new Camera(useMenuCamera) { Position = new Vector3(0, 5, 0) };

		InitializeArena();

		if (isEditor)
			_editorContext = new ArenaEditorContext(this);
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
				Tiles[i, j] = new Tile(x, z, i, j, Camera);
				_sortedTiles[i * SpawnsetBinary.ArenaDimensionMax + j] = Tiles[i, j];
			}
		}

		_lights.Add(new LightObject(64, default, new Vector3(1, 0.5f, 0)));
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
		_skull4 = new Skull4();
	}

	public void SetPlayerMovement(ReplaySimulation replaySimulation)
	{
		ReplaySimulation = replaySimulation;

		if (_player != null)
			_lights.Remove(_player.Light);

		_player = new Player(ReplaySimulation);
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

	public void Render(bool renderEditorContext, int windowWidth, int windowHeight)
	{
		Camera.PreRender(windowWidth, windowHeight);

		Shader shader = Root.InternalResources.MeshShader;
		Graphics.Gl.UseProgram(shader.Id);
		Graphics.Gl.UniformMatrix4x4(shader.GetUniformLocation("view"), Camera.ViewMatrix);
		Graphics.Gl.UniformMatrix4x4(shader.GetUniformLocation("projection"), Camera.Projection);
		Graphics.Gl.Uniform1(shader.GetUniformLocation("textureDiffuse"), 0);
		Graphics.Gl.Uniform1(shader.GetUniformLocation("textureLut"), 1);

		Span<float> lightPositions = stackalloc float[_lights.Count * 3];
		Span<float> lightColors = stackalloc float[_lights.Count * 3];
		Span<float> lightRadii = stackalloc float[_lights.Count];
		for (int i = 0; i < _lights.Count; i++)
		{
			LightObject lightObject = _lights[i];

			lightPositions[i * 3] = lightObject.Position.X;
			lightPositions[i * 3 + 1] = lightObject.Position.Y;
			lightPositions[i * 3 + 2] = lightObject.Position.Z;
			lightColors[i * 3] = lightObject.Color.X;
			lightColors[i * 3 + 1] = lightObject.Color.Y;
			lightColors[i * 3 + 2] = lightObject.Color.Z;
			lightRadii[i] = lightObject.Radius;
		}

		Graphics.Gl.Uniform1(shader.GetUniformLocation("lightCount"), lightPositions.Length);
		Graphics.Gl.Uniform3(shader.GetUniformLocation("lightPosition"), lightPositions);
		Graphics.Gl.Uniform3(shader.GetUniformLocation("lightColor"), lightColors);
		Graphics.Gl.Uniform1(shader.GetUniformLocation("lightRadius"), lightRadii);

		Root.GameResources.PostLut.Bind(TextureUnit.Texture1);

		if (_editorContext != null && CurrentTick == 0)
			_editorContext.RenderTiles(renderEditorContext, shader);
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
