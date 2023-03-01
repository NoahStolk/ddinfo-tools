using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Data;
using DevilDaggersInfo.App.Ui.SurvivalEditor.Components.SpawnsetArena;
using DevilDaggersInfo.App.Ui.SurvivalEditor.EditorArena.Data;
using DevilDaggersInfo.App.Ui.SurvivalEditor.Utils;
using DevilDaggersInfo.Core.Spawnset;
using Silk.NET.GLFW;
using Warp.NET;
using Warp.NET.Extensions;

namespace DevilDaggersInfo.App.Ui.SurvivalEditor.EditorArena;

public class ArenaLineState : IArenaState
{
	private Vector2i<int>? _lineStart;
	private float[,]? _newArena;

	public void Handle(ArenaMousePosition mousePosition)
	{
		if (Input.IsButtonPressed(MouseButton.Left))
		{
			_lineStart = mousePosition.Real;
			_newArena = StateManager.SpawnsetState.Spawnset.ArenaTiles.GetMutableClone();
		}
		else if (Input.IsButtonReleased(MouseButton.Left))
		{
			Emit(mousePosition);
		}
	}

	public void HandleOutOfRange(ArenaMousePosition mousePosition)
	{
		if (Input.IsButtonReleased(MouseButton.Left))
			Emit(mousePosition);
	}

	private void Emit(ArenaMousePosition mousePosition)
	{
		if (!_lineStart.HasValue || _newArena == null)
			return;

		Loop(mousePosition, (i, j) => _newArena[i, j] = StateManager.ArenaEditorState.SelectedHeight);

		Arena.UpdateArena(_newArena, SpawnsetEditType.ArenaLine);

		Reset();
	}

	private void Reset()
	{
		_lineStart = null;
		_newArena = null;
	}

	public void Render(ArenaMousePosition mousePosition, Vector2i<int> origin, float depth)
	{
		Loop(mousePosition, (i, j) => Root.Game.RectangleRenderer.Schedule(new(Arena.TileSize), origin + new Vector2i<int>(i, j) * Arena.TileSize + Arena.HalfTile, depth, Color.HalfTransparentWhite));

		if (_lineStart.HasValue)
		{
			ArenaEditingUtils.Stadium stadium = GetStadium(_lineStart.Value, mousePosition);

			Root.Game.EllipseRenderer.Schedule(origin + stadium.Start.RoundToVector2Int32(), 2, depth + 1, Color.White);
			Root.Game.EllipseRenderer.Schedule(origin + stadium.End.RoundToVector2Int32(), 2, depth + 1, Color.White);

			Root.Game.EllipseRenderer.Schedule(origin + stadium.Start.RoundToVector2Int32(), stadium.Radius, depth + 1, Color.White);
			Root.Game.EllipseRenderer.Schedule(origin + stadium.End.RoundToVector2Int32(), stadium.Radius, depth + 1, Color.White);

			Vector2 delta = stadium.End - stadium.Start;
			Root.Game.LineRenderer.Schedule(origin + stadium.Start.RoundToVector2Int32(), origin + stadium.End.RoundToVector2Int32(), depth + 1, Color.White);
			Root.Game.LineRenderer.Schedule(origin + stadium.Edge1Point.RoundToVector2Int32(), origin + (stadium.Edge1Point + delta).RoundToVector2Int32(), depth + 1, Color.White);
			Root.Game.LineRenderer.Schedule(origin + stadium.Edge2Point.RoundToVector2Int32(), origin + (stadium.Edge2Point + delta).RoundToVector2Int32(), depth + 1, Color.White);
		}
		else
		{
			Root.Game.EllipseRenderer.Schedule(origin + GetSnappedPosition(mousePosition.Real), GetDisplayRadius(), depth + 1, Color.HalfTransparentWhite);
		}
	}

	private void Loop(ArenaMousePosition mousePosition, Action<int, int> action)
	{
		if (!_lineStart.HasValue)
			return;

		ArenaEditingUtils.Stadium stadium = GetStadium(_lineStart.Value, mousePosition);
		for (int i = 0; i < SpawnsetBinary.ArenaDimensionMax; i++)
		{
			for (int j = 0; j < SpawnsetBinary.ArenaDimensionMax; j++)
			{
				Vector2 visualTileCenter = new Vector2(i, j) * Arena.TileSize + Arena.HalfTileAsVector2;

				ArenaEditingUtils.Square square = ArenaEditingUtils.Square.FromCenter(visualTileCenter, Arena.TileSize);
				if (square.IntersectsStadium(stadium))
					action(i, j);
			}
		}
	}

	private static ArenaEditingUtils.Stadium GetStadium(Vector2i<int> lineStart, ArenaMousePosition mousePosition)
	{
		return new(GetSnappedPosition(lineStart).ToVector2(), GetSnappedPosition(mousePosition.Real).ToVector2(), GetDisplayRadius());
	}

	private static float GetDisplayRadius()
	{
		return StateManager.ArenaLineState.Thickness / 2 * Arena.TileSize;
	}

	private static Vector2i<int> GetSnappedPosition(Vector2i<int> position)
	{
		return ArenaEditingUtils.Snap(position, Arena.TileSize) + Arena.HalfTile;
	}
}
