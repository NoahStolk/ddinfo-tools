// ReSharper disable ForCanBeConvertedToForeach
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Engine.Intersections;
using DevilDaggersInfo.Tools.Scenes.GameObjects;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Utils;
using ImGuiNET;
using Silk.NET.Input;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Scenes;

public sealed class ArenaEditorContext
{
	private readonly ArenaScene _arenaScene;
	private readonly List<(Tile Tile, float Distance)> _hitTiles = [];
	private readonly List<Tile> _selectedTiles = [];

	private Tile? _closestHitTile;

	public ArenaEditorContext(ArenaScene arenaScene)
	{
		_arenaScene = arenaScene;
	}

	public void Update(bool isActive, int currentTick)
	{
		if (!isActive || currentTick > 0 || Root.Mouse == null)
			return;

		bool ctrl = Root.Keyboard != null && (Root.Keyboard.IsKeyPressed(Key.ControlLeft) || Root.Keyboard.IsKeyPressed(Key.ControlRight));
		if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
		{
			if (_closestHitTile != null)
			{
				if (ctrl)
				{
					if (!_selectedTiles.Contains(_closestHitTile))
						_selectedTiles.Add(_closestHitTile);
					else
						_selectedTiles.Remove(_closestHitTile);
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
		else if (ImGui.IsMouseDown(ImGuiMouseButton.Left) && !ctrl && _closestHitTile != null && !_selectedTiles.Contains(_closestHitTile))
		{
			_selectedTiles.Add(_closestHitTile);
		}

		ScrollWheel scrollWheel = Root.Mouse.ScrollWheels.Count > 0 ? Root.Mouse.ScrollWheels[0] : default;
		float scroll = scrollWheel.Y;
		if (scroll == 0 || _selectedTiles.Count == 0)
			return;

		float[,] newArena = FileStates.Spawnset.Object.ArenaTiles.GetMutableClone();
		for (int i = 0; i < _selectedTiles.Count; i++)
		{
			Tile tile = _selectedTiles[i];
			float height = FileStates.Spawnset.Object.ArenaTiles[tile.ArenaX, tile.ArenaY] - scroll;
			tile.SetDisplayHeight(height);
			newArena[tile.ArenaX, tile.ArenaY] = height;
		}

		FileStates.Spawnset.Update(FileStates.Spawnset.Object with { ArenaTiles = new(FileStates.Spawnset.Object.ArenaDimension, newArena) });
		SpawnsetHistoryUtils.Save(SpawnsetEditType.ArenaTileHeight);
	}

	public void RenderTiles(bool renderEditorContext, Shader shader)
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
				Vector3 highlightColor = GetHighlightColor(tile);
				bool highlight = highlightColor != default;

				if (highlight)
					shader.SetUniform("highlightColor", highlightColor);

				tile.RenderTop();

				if (highlight)
					shader.SetUniform("highlightColor", Vector3.Zero);
			}
		}

		Root.GameResources.PillarTexture.Bind();

		for (int i = 0; i < _arenaScene.Tiles.GetLength(0); i++)
		{
			for (int j = 0; j < _arenaScene.Tiles.GetLength(1); j++)
			{
				Tile tile = _arenaScene.Tiles[i, j];
				Vector3 highlightColor = GetHighlightColor(tile);
				bool highlight = highlightColor != default;

				if (highlight)
					shader.SetUniform("highlightColor", highlightColor);

				tile.RenderPillar();

				if (highlight)
					shader.SetUniform("highlightColor", Vector3.Zero);
			}
		}

		Vector3 GetHighlightColor(Tile tile)
		{
			bool isSelected = _selectedTiles.Contains(tile);

			if (_closestHitTile == tile && renderEditorContext)
				return isSelected ? new(0.55f, 0.4f, 0.3f) : new(0.3f, 0.3f, 0.3f);

			return isSelected ? new(0.25f, 0.1f, 0) : default;
		}
	}
}
