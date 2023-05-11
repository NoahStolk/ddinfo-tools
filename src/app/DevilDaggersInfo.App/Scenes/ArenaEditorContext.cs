// ReSharper disable ForCanBeConvertedToForeach
using DevilDaggersInfo.App.Engine.Intersections;
using DevilDaggersInfo.App.Scenes.GameObjects;
using DevilDaggersInfo.App.Ui.SpawnsetEditor.State;
using DevilDaggersInfo.App.Ui.SpawnsetEditor.Utils;
using Silk.NET.Input;
using System.Numerics;

namespace DevilDaggersInfo.App.Scenes;

public sealed class ArenaEditorContext
{
	private readonly ArenaScene _arenaScene;
	private readonly List<(Tile Tile, float Distance)> _hitTiles = new();

	private Tile? _closestHitTile;

	public ArenaEditorContext(ArenaScene arenaScene)
	{
		_arenaScene = arenaScene;
	}

	public void Update(bool isActive, int currentTick)
	{
		if (!isActive)
			return;

		ScrollWheel scrollWheel = Root.Mouse?.ScrollWheels.Count > 0 ? Root.Mouse.ScrollWheels[0] : default;
		float scroll = scrollWheel.Y;
		if (currentTick > 0 || scroll == 0 || _closestHitTile == null)
			return;

		float height = SpawnsetState.Spawnset.ArenaTiles[_closestHitTile.ArenaX, _closestHitTile.ArenaY] - scroll;
		_closestHitTile.SetDisplayHeight(height);

		float[,] newArena = SpawnsetState.Spawnset.ArenaTiles.GetMutableClone();
		newArena[_closestHitTile.ArenaX, _closestHitTile.ArenaY] = height;
		SpawnsetState.Spawnset = SpawnsetState.Spawnset with { ArenaTiles = new(SpawnsetState.Spawnset.ArenaDimension, newArena) };
		SpawnsetHistoryUtils.Save(SpawnsetEditType.ArenaTileHeight);
	}

	public void RenderTiles(Shader shader)
	{
		_hitTiles.Clear();
		Ray ray = _arenaScene.Camera.ScreenToWorldPoint();
		for (int i = 0; i < _arenaScene.Tiles.GetLength(0); i++)
		{
			for (int j = 0; j < _arenaScene.Tiles.GetLength(1); j++)
			{
				Tile tile = _arenaScene.Tiles[i, j];
				Vector3 min = new(tile.PositionX - 2, -2, tile.PositionZ - 2);
				Vector3 max = new(tile.PositionX + 2, tile.Height + 2, tile.PositionZ + 2);
				RayVsAabbIntersection? intersects = ray.Intersects(min, max);
				if (intersects.HasValue)
					_hitTiles.Add((tile, intersects.Value.Distance));
			}
		}

		_closestHitTile = _hitTiles.Count == 0 ? null : _hitTiles.MinBy(ht => ht.Distance).Tile;

		// Temporarily use LutScale to highlight the target tile.
		Root.GameResources.TileTexture.Bind();

		for (int i = 0; i < _arenaScene.Tiles.GetLength(0); i++)
		{
			for (int j = 0; j < _arenaScene.Tiles.GetLength(1); j++)
			{
				Tile tile = _arenaScene.Tiles[i, j];

				if (_closestHitTile == tile)
					shader.SetUniform("lutScale", 2.5f);

				tile.RenderTop();

				if (_closestHitTile == tile)
					shader.SetUniform("lutScale", 1f);
			}
		}

		Root.GameResources.PillarTexture.Bind();

		for (int i = 0; i < _arenaScene.Tiles.GetLength(0); i++)
		{
			for (int j = 0; j < _arenaScene.Tiles.GetLength(1); j++)
			{
				Tile tile = _arenaScene.Tiles[i, j];

				if (_closestHitTile == tile)
					shader.SetUniform("lutScale", 2.5f);

				tile.RenderPillar();

				if (_closestHitTile == tile)
					shader.SetUniform("lutScale", 1f);
			}
		}
	}
}
