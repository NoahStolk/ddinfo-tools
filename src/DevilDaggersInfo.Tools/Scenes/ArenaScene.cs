// ReSharper disable ForCanBeConvertedToForeach
using DevilDaggersInfo.Core.Replay.PostProcessing.ReplaySimulation;
using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Scenes.GameObjects;
using ImGuiGlfw;
using Silk.NET.GLFW;
using Silk.NET.OpenGL;
using System.Diagnostics;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Scenes;

public sealed class ArenaScene
{
	public const float MinRenderTileHeight = -3;

	private readonly GL _gl;
	private readonly ResourceManager _resourceManager;

	private readonly Func<SpawnsetBinary> _getSpawnset;

	private readonly Tile[] _sortedTiles = new Tile[SpawnsetBinary.ArenaDimensionMax * SpawnsetBinary.ArenaDimensionMax];
	private readonly RaceDagger _raceDagger = new();
	private readonly List<LightObject> _lights = [];
	private readonly ArenaEditorContext? _editorContext;

	private Player? _player;
	// private Skull4? _skull4;

	public unsafe ArenaScene(Glfw glfw, GL gl, WindowHandle* window, GlfwInput glfwInput, ResourceManager resourceManager, Func<SpawnsetBinary> getSpawnset, bool useMenuCamera, bool isEditor)
	{
		_gl = gl;
		_resourceManager = resourceManager;

		_getSpawnset = getSpawnset;

		Camera = new Camera(glfw, glfwInput, window, useMenuCamera) { Position = new Vector3(0, 5, 0) };

		InitializeArena();

		if (isEditor)
			_editorContext = new ArenaEditorContext(this, glfwInput, gl, resourceManager);
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
				Tiles[i, j] = new Tile(x, z, i, j, Camera, _resourceManager);
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
		//_skull4 = new Skull4();
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

	public unsafe void Render(bool renderEditorContext, int windowWidth, int windowHeight)
	{
		Debug.Assert(_resourceManager.GameResources != null, $"{nameof(_resourceManager.GameResources)} is null, which should never happen here.");

		Camera.PreRender(windowWidth, windowHeight);

		Shader shader = _resourceManager.InternalResources.MeshShader;
		_gl.UseProgram(shader.Id);
		_gl.UniformMatrix4x4(shader.GetUniformLocation("view"), Camera.ViewMatrix);
		_gl.UniformMatrix4x4(shader.GetUniformLocation("projection"), Camera.Projection);
		_gl.Uniform1(shader.GetUniformLocation("textureDiffuse"), 0);
		_gl.Uniform1(shader.GetUniformLocation("textureLut"), 1);

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

		_gl.Uniform1(shader.GetUniformLocation("lightCount"), lightPositions.Length);
		_gl.Uniform3(shader.GetUniformLocation("lightPosition"), lightPositions);
		_gl.Uniform3(shader.GetUniformLocation("lightColor"), lightColors);
		_gl.Uniform1(shader.GetUniformLocation("lightRadius"), lightRadii);

		_resourceManager.GameResources.PostLut.Bind(TextureUnit.Texture1);

		if (_editorContext != null && CurrentTick == 0)
			_editorContext.RenderTiles(renderEditorContext, shader);
		else
			RenderTilesDefault();

		// Render dagger.
		_resourceManager.GameResources.DaggerSilverTexture.Bind();
		_gl.UniformMatrix4x4(_resourceManager.InternalResources.MeshShader.GetUniformLocation("model"), Matrix4x4.CreateScale(8) * Matrix4x4.CreateFromQuaternion(_raceDagger.MeshRotation) * Matrix4x4.CreateTranslation(_raceDagger.MeshPosition));

		_gl.BindVertexArray(RaceDagger.Vao);
		fixed (uint* i = &ContentManager.Content.DaggerMesh.Indices[0])
			_gl.DrawElements(PrimitiveType.Triangles, (uint)ContentManager.Content.DaggerMesh.Indices.Length, DrawElementsType.UnsignedInt, i);

		_gl.BindVertexArray(0);

		// Render player.
		if (_player != null)
		{
			_resourceManager.GameResources.Hand4Texture.Bind();
			_gl.UniformMatrix4x4(_resourceManager.InternalResources.MeshShader.GetUniformLocation("model"), Matrix4x4.CreateScale(4) * Matrix4x4.CreateFromQuaternion(_player.Mesh.Rotation) * Matrix4x4.CreateTranslation(_player.Mesh.Position));

			_gl.BindVertexArray(_player.Mesh.Vao);
			fixed (uint* i = &_player.Mesh.Mesh.Indices[0])
				_gl.DrawElements(PrimitiveType.Triangles, (uint)_player.Mesh.Mesh.Indices.Length, DrawElementsType.UnsignedInt, i);

			_gl.BindVertexArray(0);
		}

		// _skull4?.Render();

		_resourceManager.InternalResources.TileHitboxTexture.Bind();

		Array.Sort(_sortedTiles, static (a, b) => a.SquaredDistanceToCamera().CompareTo(b.SquaredDistanceToCamera()));
		for (int i = 0; i < _sortedTiles.Length; i++)
		{
			Tile tile = _sortedTiles[i];
			tile.RenderHitbox(_gl, _resourceManager);
		}
	}

	private void RenderTilesDefault()
	{
		Debug.Assert(_resourceManager.GameResources != null, $"{nameof(_resourceManager.GameResources)} is null, which should never happen here.");

		_resourceManager.GameResources.TileTexture.Bind();

		for (int i = 0; i < Tiles.GetLength(0); i++)
		{
			for (int j = 0; j < Tiles.GetLength(1); j++)
				Tiles[i, j].RenderTop(_gl, _resourceManager);
		}

		_resourceManager.GameResources.PillarTexture.Bind();

		for (int i = 0; i < Tiles.GetLength(0); i++)
		{
			for (int j = 0; j < Tiles.GetLength(1); j++)
				Tiles[i, j].RenderPillar(_gl, _resourceManager);
		}
	}
}
