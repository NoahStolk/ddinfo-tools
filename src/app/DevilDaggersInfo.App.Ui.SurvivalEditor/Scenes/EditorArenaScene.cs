// ReSharper disable ForCanBeConvertedToForeach
using DevilDaggersInfo.App.Ui.Base;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Actions;
using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Data;
using DevilDaggersInfo.App.Ui.Scene;
using DevilDaggersInfo.App.Ui.Scene.GameObjects;
using DevilDaggersInfo.Core.Spawnset;
using Silk.NET.OpenGL;
using Warp.NET;
using Warp.NET.Intersections;
using Shader = Warp.NET.Content.Shader;

namespace DevilDaggersInfo.App.Ui.SurvivalEditor.Scenes;

public sealed class EditorArenaScene : IArenaScene
{
	private readonly List<(Tile Tile, float Distance)> _hitTiles = new();
	private Tile? _closestHitTile;

	public Camera Camera { get; } = new();
	public List<Tile> Tiles { get; } = new();
	public List<LightObject> Lights { get; } = new();
	public RaceDagger? RaceDagger { get; set; }

	private void Clear()
	{
		Tiles.Clear();
		Lights.Clear();

		RaceDagger = null;
	}

	public void BuildSpawnset(SpawnsetBinary spawnset)
	{
		Clear();

		IArenaScene scene = this;
		scene.AddArena(spawnset);

		int halfSize = spawnset.ArenaDimension / 2;
		float cameraHeight = Math.Max(4, spawnset.ArenaTiles[halfSize, halfSize] + 8);
		Camera.Reset(new(0, cameraHeight, 0));
		Camera.IsMenuCamera = false;
	}

	public void Update(int currentTick)
	{
		for (int i = 0; i < Lights.Count; i++)
			Lights[i].PrepareUpdate();

		Camera.Update();
		RaceDagger?.Update(currentTick);

		for (int i = 0; i < Tiles.Count; i++)
		{
			Tile tile = Tiles[i];
			tile.SetDisplayHeight(StateManager.SpawnsetState.Spawnset.GetActualTileHeight(tile.ArenaX, tile.ArenaY, currentTick / 60f));
		}

		int scroll = Input.GetScroll();
		if (currentTick > 0 || scroll == 0 || _closestHitTile == null)
			return;

		float height = StateManager.SpawnsetState.Spawnset.ArenaTiles[_closestHitTile.ArenaX, _closestHitTile.ArenaY] - scroll;
		_closestHitTile.SetDisplayHeight(height);

		float[,] newArena = StateManager.SpawnsetState.Spawnset.ArenaTiles.GetMutableClone();
		newArena[_closestHitTile.ArenaX, _closestHitTile.ArenaY] = height;
		StateManager.Dispatch(new UpdateArena(newArena, SpawnsetEditType.ArenaTileHeight));

		int dimension = StateManager.SpawnsetState.Spawnset.ArenaDimension;
		RaceDagger?.UpdatePosition(dimension, new(dimension, newArena), StateManager.SpawnsetState.Spawnset.RaceDaggerPosition);
	}

	public void Render(int currentTick)
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

		RenderTiles(currentTick);

		RaceDagger?.Render();

		WarpTextures.TileHitbox.Use();

		Tiles.Sort(static (a, b) => a.SquaredDistanceToCamera().CompareTo(b.SquaredDistanceToCamera()));
		foreach (Tile tile in Tiles)
			tile.RenderHitbox();
	}

	private void RenderTiles(int currentTick)
	{
		if (currentTick == 0)
		{
			_hitTiles.Clear();
			Ray ray = Camera.ScreenToWorldPoint();
			for (int i = 0; i < Tiles.Count; i++)
			{
				Tile tile = Tiles[i];
				Vector3 min = new(tile.PositionX - 2, -2, tile.PositionZ - 2);
				Vector3 max = new(tile.PositionX + 2, tile.Height + 2, tile.PositionZ + 2);
				RayVsAabbIntersection? intersects = ray.Intersects(min, max);
				if (intersects.HasValue)
					_hitTiles.Add((tile, intersects.Value.Distance));
			}

			_closestHitTile = _hitTiles.Count == 0 ? null : _hitTiles.MinBy(ht => ht.Distance).Tile;

			// Temporarily use LutScale to highlight the target tile.
			ContentManager.Content.TileTexture.Use();
			for (int i = 0; i < Tiles.Count; i++)
			{
				Tile tile = Tiles[i];

				if (_closestHitTile == tile)
					Shader.SetFloat(MeshUniforms.LutScale, 2.5f);

				tile.RenderTop();

				if (_closestHitTile == tile)
					Shader.SetFloat(MeshUniforms.LutScale, 1f);
			}

			ContentManager.Content.PillarTexture.Use();
			for (int i = 0; i < Tiles.Count; i++)
			{
				Tile tile = Tiles[i];

				if (_closestHitTile == tile)
					Shader.SetFloat(MeshUniforms.LutScale, 2.5f);

				tile.RenderPillar();

				if (_closestHitTile == tile)
					Shader.SetFloat(MeshUniforms.LutScale, 1f);
			}
		}
		else
		{
			ContentManager.Content.TileTexture.Use();
			for (int i = 0; i < Tiles.Count; i++)
				Tiles[i].RenderTop();

			ContentManager.Content.PillarTexture.Use();
			for (int i = 0; i < Tiles.Count; i++)
				Tiles[i].RenderPillar();
		}
	}
}
