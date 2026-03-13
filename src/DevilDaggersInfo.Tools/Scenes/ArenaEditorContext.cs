// ReSharper disable ForCanBeConvertedToForeach
using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Engine.Intersections;
using DevilDaggersInfo.Tools.Scenes.GameObjects;
using DevilDaggersInfo.Tools.Ui;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Utils;
using ImGuiNET;
using Silk.NET.GLFW;
using Silk.NET.OpenGL;
using System.Diagnostics;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Scenes;

internal sealed class ArenaEditorContext(ArenaScene arenaScene, GlfwInput glfwInput, GL gl, ResourceManager resourceManager)
{
	private readonly List<(Tile Tile, float Distance)> _hitTiles = [];
	private readonly List<Tile> _selectedTiles = [];

	private Tile? _closestHitTile;

	public void Update(bool isActive, int currentTick)
	{
		if (!isActive || currentTick > 0)
			return;

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

		float[,] newArena = FileStates.Spawnset.Object.ArenaTiles.GetMutableClone();
		for (int i = 0; i < _selectedTiles.Count; i++)
		{
			Tile tile = _selectedTiles[i];
			float height = FileStates.Spawnset.Object.ArenaTiles[tile.ArenaX, tile.ArenaY] - scroll;
			tile.SetDisplayHeight(height);
			newArena[tile.ArenaX, tile.ArenaY] = height;
		}

		FileStates.Spawnset.Update(FileStates.Spawnset.Object with { ArenaTiles = new ImmutableArena(FileStates.Spawnset.Object.ArenaDimension, newArena) });
		SpawnsetHistoryUtils.Save(SpawnsetEditType.ArenaTileHeight);
	}

	public void RenderTiles(bool renderEditorContext, Shader shader)
	{
		Debug.Assert(resourceManager.GameResources != null, $"{nameof(resourceManager.GameResources)} is null, which should never happen here.");

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

		// Temporarily use LutScale to highlight the target tile.
		resourceManager.GameResources.TileTexture.Bind();

		for (int i = 0; i < arenaScene.Tiles.GetLength(0); i++)
		{
			for (int j = 0; j < arenaScene.Tiles.GetLength(1); j++)
			{
				Tile tile = arenaScene.Tiles[i, j];
				Vector3 highlightColor = GetHighlightColor(tile);
				bool highlight = highlightColor != default;

				if (highlight)
					gl.Uniform3(shader.GetUniformLocation("highlightColor"), highlightColor);

				tile.RenderTop(gl, resourceManager);

				if (highlight)
					gl.Uniform3(shader.GetUniformLocation("highlightColor"), Vector3.Zero);
			}
		}

		resourceManager.GameResources.PillarTexture.Bind();

		for (int i = 0; i < arenaScene.Tiles.GetLength(0); i++)
		{
			for (int j = 0; j < arenaScene.Tiles.GetLength(1); j++)
			{
				Tile tile = arenaScene.Tiles[i, j];
				Vector3 highlightColor = GetHighlightColor(tile);
				bool highlight = highlightColor != default;

				if (highlight)
					gl.Uniform3(shader.GetUniformLocation("highlightColor"), highlightColor);

				tile.RenderPillar(gl, resourceManager);

				if (highlight)
					gl.Uniform3(shader.GetUniformLocation("highlightColor"), Vector3.Zero);
			}
		}

		Vector3 GetHighlightColor(Tile tile)
		{
			bool isSelected = _selectedTiles.Contains(tile);

			if (_closestHitTile == tile && renderEditorContext)
				return isSelected ? new Vector3(0.55f, 0.4f, 0.3f) : new Vector3(0.3f, 0.3f, 0.3f);

			return isSelected ? new Vector3(0.25f, 0.1f, 0) : default;
		}
	}
}
