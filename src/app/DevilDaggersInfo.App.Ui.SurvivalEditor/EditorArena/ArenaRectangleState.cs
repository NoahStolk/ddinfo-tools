using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Data;
using DevilDaggersInfo.App.Ui.SurvivalEditor.Components.SpawnsetArena;
using DevilDaggersInfo.App.Ui.SurvivalEditor.EditorArena.Data;
using DevilDaggersInfo.App.Ui.SurvivalEditor.Utils;
using Silk.NET.GLFW;
using Warp.NET;

namespace DevilDaggersInfo.App.Ui.SurvivalEditor.EditorArena;

public class ArenaRectangleState : IArenaState
{
	private Vector2i<int>? _rectangleStart;
	private float[,]? _newArena;

	public void Handle(ArenaMousePosition mousePosition)
	{
		if (Input.IsButtonPressed(MouseButton.Left))
		{
			_rectangleStart = mousePosition.Tile;
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
		if (!_rectangleStart.HasValue || _newArena == null)
			return;

		Loop(mousePosition, (i, j) => _newArena[i, j] = StateManager.ArenaEditorState.SelectedHeight);

		Arena.UpdateArena(_newArena, SpawnsetEditType.ArenaRectangle);

		Reset();
	}

	private void Reset()
	{
		_rectangleStart = null;
		_newArena = null;
	}

	public void Render(ArenaMousePosition mousePosition, Vector2i<int> origin, float depth)
	{
		Loop(mousePosition, (i, j) => Root.Game.RectangleRenderer.Schedule(new(Arena.TileSize), origin + new Vector2i<int>(i, j) * Arena.TileSize + Arena.HalfTile, depth, Color.HalfTransparentWhite));
	}

	private void Loop(ArenaMousePosition mousePosition, Action<int, int> action)
	{
		if (!_rectangleStart.HasValue)
			return;

		ArenaEditingUtils.Rectangle rectangle = ArenaEditingUtils.GetRectangle(_rectangleStart.Value, mousePosition.Tile);

		int startXa = Math.Max(0, rectangle.X1);
		int startYa = Math.Max(0, rectangle.Y1);
		int endXa = Math.Min(StateManager.SpawnsetState.Spawnset.ArenaDimension - 1, rectangle.X2);
		int endYa = Math.Min(StateManager.SpawnsetState.Spawnset.ArenaDimension - 1, rectangle.Y2);

		if (StateManager.ArenaRectangleState.Filled)
		{
			for (int i = startXa; i <= endXa; i++)
			{
				for (int j = startYa; j <= endYa; j++)
					action(i, j);
			}
		}
		else
		{
			int addedSize = StateManager.ArenaRectangleState.Thickness;
			int startXb = Math.Max(0, rectangle.X1 + addedSize);
			int startYb = Math.Max(0, rectangle.Y1 + addedSize);
			int endXb = Math.Min(StateManager.SpawnsetState.Spawnset.ArenaDimension - 1, rectangle.X2 - addedSize);
			int endYb = Math.Min(StateManager.SpawnsetState.Spawnset.ArenaDimension - 1, rectangle.Y2 - addedSize);

			for (int i = startXa; i <= endXa; i++)
			{
				for (int j = startYa; j <= endYa; j++)
				{
					if (i >= startXb && i <= endXb && j >= startYb && j <= endYb)
						continue;

					action(i, j);
				}
			}
		}
	}
}
