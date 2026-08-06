// ReSharper disable ForCanBeConvertedToForeach
using DevilDaggersInfo.Core.Replay.PostProcessing.ReplaySimulation;
using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Scenes.GameObjects;
using DevilDaggersInfo.Tools.Ui;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Utils;
using Silk.NET.GLFW;
using Silk.NET.OpenGL;
using System.Diagnostics;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Scenes;

internal sealed class ArenaScene
{
	public const float MinRenderTileHeight = -3;

	private readonly GL _gl;
	private readonly ResourceManager _resourceManager;

	private readonly Func<SpawnsetBinary> _getSpawnset;

	// Scratch buffers for the transparent hitbox pass, reused every frame.
	private readonly Tile[] _hitboxTiles = new Tile[SpawnsetBinary.ArenaDimensionMax * SpawnsetBinary.ArenaDimensionMax];
	private readonly float[] _hitboxSquaredDistances = new float[SpawnsetBinary.ArenaDimensionMax * SpawnsetBinary.ArenaDimensionMax];

	private readonly RaceDagger _raceDagger = new();
	private readonly List<LightObject> _lights = [];
	private readonly ArenaEditorContext? _editorContext;

	private Player? _player;
	private Skull4? _skull4;

	public unsafe ArenaScene(
		Glfw glfw,
		GL gl,
		WindowHandle* window,
		GlfwInput glfwInput,
		ResourceManager resourceManager,
		Func<SpawnsetBinary> getSpawnset,
		bool useMenuCamera,
		bool isEditor,
		FileStates fileStates,
		SpawnsetSaver spawnsetSaver)
	{
		_gl = gl;
		_resourceManager = resourceManager;

		_getSpawnset = getSpawnset;

		Camera = new Camera(glfw, glfwInput, window, useMenuCamera) { Position = new Vector3(0, 5, 0) };

		InitializeArena();

		if (isEditor)
			_editorContext = new ArenaEditorContext(this, glfwInput, gl, resourceManager, fileStates, spawnsetSaver);
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
		_skull4 = new Skull4(_gl, _resourceManager);
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

		_gl.Uniform1(shader.GetUniformLocation("lightCount"), _lights.Count);
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

		RaceDagger.Mesh.Draw();

		// Render player.
		if (_player != null)
		{
			_resourceManager.GameResources.Hand4Texture.Bind();
			_gl.UniformMatrix4x4(_resourceManager.InternalResources.MeshShader.GetUniformLocation("model"), Matrix4x4.CreateScale(4) * Matrix4x4.CreateFromQuaternion(_player.Mesh.Rotation) * Matrix4x4.CreateTranslation(_player.Mesh.Position));

			_player.Mesh.Mesh.Draw();
		}

		_skull4?.Render();

		RenderTileHitboxes();
	}

	/// <summary>
	/// Renders the alpha-blended tile hitboxes. These must be drawn back to front with depth writes disabled, otherwise
	/// each hitbox occludes the ones behind it instead of blending with them.
	/// </summary>
	private void RenderTileHitboxes()
	{
		Debug.Assert(_resourceManager.GameResources != null, $"{nameof(_resourceManager.GameResources)} is null, which should never happen here.");

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

		if (count == 0)
			return;

		// Sorts nearest first; the draw loop below then walks the range backwards to get farthest first.
		Array.Sort(_hitboxSquaredDistances, _hitboxTiles, 0, count);

		_resourceManager.InternalResources.TileHitboxTexture.Bind();

		_gl.DepthMask(false);
		for (int i = count - 1; i >= 0; i--)
			_hitboxTiles[i].RenderHitbox(_gl, _resourceManager);

		// Depth writes must be restored, otherwise the next frame's glClear cannot clear the depth buffer.
		_gl.DepthMask(true);
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
