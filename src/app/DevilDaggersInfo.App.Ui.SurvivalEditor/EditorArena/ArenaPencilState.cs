using DevilDaggersInfo.App.Engine;
using DevilDaggersInfo.App.Engine.Extensions;
using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Data;
using DevilDaggersInfo.App.Ui.SurvivalEditor.Components.SpawnsetArena;
using DevilDaggersInfo.App.Ui.SurvivalEditor.EditorArena.Data;
using DevilDaggersInfo.App.Ui.SurvivalEditor.Utils;
using DevilDaggersInfo.Core.Spawnset;
using Silk.NET.GLFW;

namespace DevilDaggersInfo.App.Ui.SurvivalEditor.EditorArena;

public class ArenaPencilState : IArenaState
{
	private Vector2i<int>? _pencilStart;
	private HashSet<Vector2i<int>>? _modifiedCoords;

	public void Handle(ArenaMousePosition mousePosition)
	{
		if (Input.IsButtonPressed(MouseButton.Left))
		{
			_pencilStart = mousePosition.Real;
			_modifiedCoords = new();
		}
		else if (Input.IsButtonHeld(MouseButton.Left))
		{
			if (!_pencilStart.HasValue || _modifiedCoords == null)
				return;

			Vector2i<int> pencilEnd = mousePosition.Real;
			Vector2 start = ArenaEditingUtils.Snap(_pencilStart.Value.ToVector2(), Arena.TileSize) + Arena.HalfTileAsVector2;
			Vector2 end = ArenaEditingUtils.Snap(pencilEnd.ToVector2(), Arena.TileSize) + Arena.HalfTileAsVector2;
			ArenaEditingUtils.Stadium stadium = new(start, end, StateManager.ArenaPencilState.Size / 2 * Arena.TileSize);
			for (int i = 0; i < SpawnsetBinary.ArenaDimensionMax; i++)
			{
				for (int j = 0; j < SpawnsetBinary.ArenaDimensionMax; j++)
				{
					Vector2i<int> target = new(i, j);
					if (_modifiedCoords.Contains(target)) // Early rejection, even though we're using a HashSet.
						continue;

					Vector2 visualTileCenter = new Vector2(i, j) * Arena.TileSize + Arena.HalfTile.ToVector2();

					ArenaEditingUtils.Square square = ArenaEditingUtils.Square.FromCenter(visualTileCenter, Arena.TileSize);
					if (square.IntersectsStadium(stadium))
						_modifiedCoords.Add(target);
				}
			}

			_modifiedCoords.Add(new(mousePosition.Tile.X, mousePosition.Tile.Y));
			_pencilStart = mousePosition.Real;
		}
		else if (Input.IsButtonReleased(MouseButton.Left))
		{
			Emit();
		}
	}

	public void HandleOutOfRange(ArenaMousePosition mousePosition)
	{
		if (Input.IsButtonHeld(MouseButton.Left))
			_pencilStart = mousePosition.Real;
		else if (Input.IsButtonReleased(MouseButton.Left))
			Emit();
	}

	private void Emit()
	{
		if (!_pencilStart.HasValue || _modifiedCoords == null)
			return;

		float[,] newArena = StateManager.SpawnsetState.Spawnset.ArenaTiles.GetMutableClone();

		foreach (Vector2i<int> position in _modifiedCoords)
			newArena[position.X, position.Y] = StateManager.ArenaEditorState.SelectedHeight;

		Arena.UpdateArena(newArena, SpawnsetEditType.ArenaPencil);

		Reset();
	}

	private void Reset()
	{
		_pencilStart = null;
		_modifiedCoords = null;
	}

	public void Render(ArenaMousePosition mousePosition, Vector2i<int> origin, float depth)
	{
		Root.Game.EllipseRenderer.Schedule(origin + GetSnappedPosition(mousePosition.Real), GetDisplayRadius(), depth + 1, _pencilStart.HasValue ? Color.White : Color.HalfTransparentWhite);

		if (_modifiedCoords == null)
			return;

		for (int i = 0; i < SpawnsetBinary.ArenaDimensionMax; i++)
		{
			for (int j = 0; j < SpawnsetBinary.ArenaDimensionMax; j++)
			{
				if (_modifiedCoords.Contains(new(i, j)))
					Root.Game.RectangleRenderer.Schedule(new(Arena.TileSize), origin + new Vector2i<int>(i, j) * Arena.TileSize + Arena.HalfTile, depth, Color.HalfTransparentWhite);
			}
		}
	}

	private static float GetDisplayRadius()
	{
		return StateManager.ArenaPencilState.Size / 2 * Arena.TileSize;
	}

	private static Vector2i<int> GetSnappedPosition(Vector2i<int> position)
	{
		return ArenaEditingUtils.Snap(position, Arena.TileSize) + Arena.HalfTile;
	}
}
