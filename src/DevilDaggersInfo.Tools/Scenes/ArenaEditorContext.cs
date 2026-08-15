// ReSharper disable ForCanBeConvertedToForeach
using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Engine.Intersections;
using DevilDaggersInfo.Tools.Scenes.GameObjects;
using DevilDaggersInfo.Tools.Ui;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Utils;
using Hexa.NET.ImGui;
using Silk.NET.GLFW;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Scenes;

internal sealed class ArenaEditorContext(ArenaScene arenaScene, GlfwInput glfwInput, FileStates fileStates, SpawnsetSaver spawnsetSaver)
{
	private readonly List<(Tile Tile, float Distance)> _hitTiles = [];
	private readonly List<Tile> _selectedTiles = [];

	private Tile? _closestHitTile;

	public void Update(bool isActive, int currentTick)
	{
		if (!isActive || currentTick > 0)
		{
			// Clear the hover, otherwise the highlight stays stuck on whichever tile was last under the cursor.
			_closestHitTile = null;
			return;
		}

		UpdateHoveredTile();

		bool ctrl = glfwInput.IsKeyDown(Keys.ControlLeft) || glfwInput.IsKeyDown(Keys.ControlRight);
		if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
		{
			if (_closestHitTile is { Height: > -3 })
			{
				if (ctrl)
				{
					if (!_selectedTiles.Remove(_closestHitTile))
						_selectedTiles.Add(_closestHitTile);
				}
				else
				{
					_selectedTiles.Clear();
					_selectedTiles.Add(_closestHitTile);
				}
			}
			else
			{
				_selectedTiles.Clear();
			}
		}
		else if (ImGui.IsMouseDown(ImGuiMouseButton.Left) && !ctrl && _closestHitTile is { Height: > -3 } && !_selectedTiles.Contains(_closestHitTile))
		{
			_selectedTiles.Add(_closestHitTile);
		}

		float scroll = glfwInput.MouseWheelY;
		if (scroll is > -float.Epsilon and < float.Epsilon || _selectedTiles.Count == 0)
			return;

		float[,] newArena = fileStates.Spawnset.Object.ArenaTiles.GetMutableClone();
		for (int i = 0; i < _selectedTiles.Count; i++)
		{
			Tile tile = _selectedTiles[i];
			float height = fileStates.Spawnset.Object.ArenaTiles[tile.ArenaX, tile.ArenaY] - scroll;
			tile.SetDisplayHeight(height);
			newArena[tile.ArenaX, tile.ArenaY] = height;
		}

		fileStates.Spawnset.Update(fileStates.Spawnset.Object with { ArenaTiles = new ImmutableArena(fileStates.Spawnset.Object.ArenaDimension, newArena) });
		spawnsetSaver.Save(SpawnsetEditType.ArenaTileHeight);
	}

	/// <summary>
	/// Raycasts the cursor against the tile hitboxes to determine which tile is under it.
	/// </summary>
	private void UpdateHoveredTile()
	{
		_hitTiles.Clear();
		Ray ray = arenaScene.Camera.ScreenToWorldPoint();
		for (int i = 0; i < arenaScene.Tiles.GetLength(0); i++)
		{
			for (int j = 0; j < arenaScene.Tiles.GetLength(1); j++)
			{
				Tile tile = arenaScene.Tiles[i, j];
				Vector3 min = new(tile.PositionX - 2, -2, tile.PositionZ - 2);
				Vector3 max = new(tile.PositionX + 2, tile.Height + 2, tile.PositionZ + 2);
				RayVsAabbIntersection? intersects = ray.Intersects(min, max);
				if (intersects.HasValue)
					_hitTiles.Add((tile, intersects.Value.Distance));
			}
		}

		_closestHitTile = _hitTiles.Count == 0 ? null : _hitTiles.MinBy(ht => ht.Distance).Tile;
	}

	/// <summary>
	/// The additive colour the given tile should be tinted with, or <see cref="Vector3.Zero"/> for no tint.
	/// </summary>
	public Vector3 GetHighlightColor(Tile tile, bool isHovering)
	{
		bool isSelected = _selectedTiles.Contains(tile);

		if (_closestHitTile == tile && isHovering)
			return isSelected ? new Vector3(0.55f, 0.4f, 0.3f) : new Vector3(0.3f, 0.3f, 0.3f);

		return isSelected ? new Vector3(0.25f, 0.1f, 0) : default;
	}
}
