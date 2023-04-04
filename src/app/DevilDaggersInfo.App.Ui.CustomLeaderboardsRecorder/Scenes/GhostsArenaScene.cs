// ReSharper disable ForCanBeConvertedToForeach
using DevilDaggersInfo.App.Ui.Base;
using DevilDaggersInfo.App.Ui.Scene;
using DevilDaggersInfo.App.Ui.Scene.GameObjects;
using DevilDaggersInfo.Core.Replay.PostProcessing.ReplaySimulation;
using DevilDaggersInfo.Core.Spawnset;
using Silk.NET.OpenGL;

namespace DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Scenes;

public sealed class GhostsArenaScene : IArenaScene
{
	private SpawnsetBinary? _spawnset;
#if SOUNDS
	private ReplaySimulation? _replaySimulation;
#endif
	private Player? _player;

	public Camera Camera { get; } = new();
	public List<Tile> Tiles { get; } = new();
	public List<LightObject> Lights { get; } = new();
	public RaceDagger? RaceDagger { get; set; }

	private void Clear()
	{
		_spawnset = null;
#if SOUNDS
		_replaySimulation = null;
#endif
		_player = null;

		Tiles.Clear();
		Lights.Clear();

		RaceDagger = null;
	}

	public void BuildSpawnset(SpawnsetBinary spawnset)
	{
		Clear();

		_spawnset = spawnset;

		IArenaScene scene = this;
		scene.AddArena(spawnset);

		int halfSize = spawnset.ArenaDimension / 2;
		float cameraHeight = Math.Max(4, spawnset.ArenaTiles[halfSize, halfSize] + 8);
		Camera.Reset(new(0, cameraHeight, 0));
		Camera.IsMenuCamera = false;
	}

	public void BuildPlayerMovement(ReplaySimulation replaySimulation)
	{
#if SOUNDS
		_replaySimulation = replaySimulation;
#endif
		_player = new(replaySimulation);
		Lights.Add(_player.Light);
	}

	public void Update(int currentTick)
	{
		if (_spawnset == null)
			return;

		for (int i = 0; i < Lights.Count; i++)
			Lights[i].PrepareUpdate();

		Camera.Update();
		RaceDagger?.Update(currentTick);
		_player?.Update(currentTick);

		for (int i = 0; i < Tiles.Count; i++)
		{
			Tile tile = Tiles[i];
			tile.SetDisplayHeight(_spawnset.GetActualTileHeight(tile.ArenaX, tile.ArenaY, currentTick / 60f));
		}

#if SOUNDS
		SoundSnapshot[] soundSnapshots = _replaySimulation?.GetSoundSnapshots(currentTick) ?? Array.Empty<SoundSnapshot>();
		foreach (SoundSnapshot soundSnapshot in soundSnapshots)
		{
			Sound3dObject sound = new(soundSnapshot.Sound switch
			{
				ReplaySound.Jump1 => ContentManager.Content.SoundJump1,
				ReplaySound.Jump2 => ContentManager.Content.SoundJump2,
				ReplaySound.Jump3 => ContentManager.Content.SoundJump3,
				_ => throw new UnreachableException(),
			})
			{
				Position = soundSnapshot.Position,
			};
			sound.Add();
		}
#endif
	}

	public void Render()
	{
		for (int i = 0; i < Lights.Count; i++)
			Lights[i].PrepareRender();

		Camera.PreRender();

		WarpShaders.Mesh.Use();
		Shader.SetMatrix4x4(MeshUniforms.View, Camera.ViewMatrix);
		Shader.SetMatrix4x4(MeshUniforms.Projection, Camera.Projection);
		Shader.SetInt(MeshUniforms.TextureDiffuse, 0);
		Shader.SetInt(MeshUniforms.TextureLut, 1);
		Shader.SetFloat(MeshUniforms.LutScale, 1f);

		Span<Vector3> lightPositions = Lights.Select(lo => lo.PositionState.Render).ToArray();
		Span<Vector3> lightColors = Lights.Select(lo => lo.ColorState.Render).ToArray();
		Span<float> lightRadii = Lights.Select(lo => lo.RadiusState.Render).ToArray();

		Shader.SetInt(MeshUniforms.LightCount, lightPositions.Length);
		Shader.SetVector3Array(MeshUniforms.LightPosition, lightPositions);
		Shader.SetVector3Array(MeshUniforms.LightColor, lightColors);
		Shader.SetFloatArray(MeshUniforms.LightRadius, lightRadii);

		ContentManager.Content.PostLut.Use(TextureUnit.Texture1);

		ContentManager.Content.TileTexture.Use();

		for (int i = 0; i < Tiles.Count; i++)
			Tiles[i].RenderTop();

		ContentManager.Content.PillarTexture.Use();
		for (int i = 0; i < Tiles.Count; i++)
			Tiles[i].RenderPillar();

		RaceDagger?.Render();
		_player?.Render();

		WarpTextures.TileHitbox.Use();

		Tiles.Sort(static (a, b) => a.SquaredDistanceToCamera().CompareTo(b.SquaredDistanceToCamera()));
		foreach (Tile tile in Tiles)
			tile.RenderHitbox();
	}
}
