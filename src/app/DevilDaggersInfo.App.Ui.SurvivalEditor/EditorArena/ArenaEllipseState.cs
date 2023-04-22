using DevilDaggersInfo.App.Engine;
using DevilDaggersInfo.App.Engine.Extensions;
using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.StateManagement.SurvivalEditor.Data;
using DevilDaggersInfo.App.Ui.SurvivalEditor.Components.SpawnsetArena;
using DevilDaggersInfo.App.Ui.SurvivalEditor.EditorArena.Data;
using DevilDaggersInfo.App.Ui.SurvivalEditor.Utils;
using Silk.NET.GLFW;

namespace DevilDaggersInfo.App.Ui.SurvivalEditor.EditorArena;

public class ArenaEllipseState : IArenaState
{
	private Vector2i<int>? _ellipseStart;
	private float[,]? _newArena;

	public void Handle(ArenaMousePosition mousePosition)
	{
		if (Input.IsButtonPressed(MouseButton.Left))
		{
			_ellipseStart = mousePosition.Real;
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
		if (!_ellipseStart.HasValue || _newArena == null)
			return;

		Loop(mousePosition, (i, j) => _newArena[i, j] = StateManager.ArenaEditorState.SelectedHeight);

		Arena.UpdateArena(_newArena, SpawnsetEditType.ArenaEllipse);

		Reset();
	}

	private void Reset()
	{
		_ellipseStart = null;
		_newArena = null;
	}

	public void Render(ArenaMousePosition mousePosition, Vector2i<int> origin, float depth)
	{
		Loop(mousePosition, (i, j) => Root.Game.RectangleRenderer.Schedule(new(Arena.TileSize), origin + new Vector2i<int>(i, j) * Arena.TileSize + Arena.HalfTile, depth, Color.HalfTransparentWhite));

		if (!_ellipseStart.HasValue)
			return;

		ArenaEditingUtils.AlignedEllipse ellipse = GetEllipse(_ellipseStart.Value, mousePosition);
		Root.Game.EllipseRenderer.Schedule(origin + ellipse.Center.RoundToVector2Int32(), ellipse.Radius, depth + 1, Color.White);

		if (!StateManager.ArenaEllipseState.Filled)
		{
			ArenaEditingUtils.AlignedEllipse innerEllipse = GetEllipse(_ellipseStart.Value, mousePosition, (StateManager.ArenaEllipseState.Thickness - 1) * Arena.TileSize);
			Root.Game.EllipseRenderer.Schedule(origin + innerEllipse.Center.RoundToVector2Int32(), innerEllipse.Radius, depth + 1, Color.White);
		}
	}

	private void Loop(ArenaMousePosition mousePosition, Action<int, int> action)
	{
		if (!_ellipseStart.HasValue)
			return;

		ArenaEditingUtils.AlignedEllipse ellipse = GetEllipse(_ellipseStart.Value, mousePosition);

		if (StateManager.ArenaEllipseState.Filled)
		{
			for (int i = 0; i < StateManager.SpawnsetState.Spawnset.ArenaDimension; i++)
			{
				for (int j = 0; j < StateManager.SpawnsetState.Spawnset.ArenaDimension; j++)
				{
					Vector2 visualTileCenter = new Vector2(i, j) * Arena.TileSize + Arena.HalfTileAsVector2;

					ArenaEditingUtils.Square square = ArenaEditingUtils.Square.FromCenter(visualTileCenter, Arena.TileSize);
					if (ellipse.Contains(square))
						action(i, j);
				}
			}
		}
		else
		{
			ArenaEditingUtils.AlignedEllipse innerEllipse = GetEllipse(_ellipseStart.Value, mousePosition, (StateManager.ArenaEllipseState.Thickness - 1) * Arena.TileSize);
			for (int i = 0; i < StateManager.SpawnsetState.Spawnset.ArenaDimension; i++)
			{
				for (int j = 0; j < StateManager.SpawnsetState.Spawnset.ArenaDimension; j++)
				{
					Vector2 visualTileCenter = new Vector2(i, j) * Arena.TileSize + Arena.HalfTileAsVector2;
					ArenaEditingUtils.Square square = ArenaEditingUtils.Square.FromCenter(visualTileCenter, Arena.TileSize);

					if (IsBetweenEllipses())
						action(i, j);

					bool IsBetweenEllipses()
					{
						if (ellipse.Intersects(square) || innerEllipse.Intersects(square))
							return true;

						return ellipse.Contains(square) && !innerEllipse.Contains(square);
					}
				}
			}
		}
	}

	private static ArenaEditingUtils.AlignedEllipse GetEllipse(Vector2i<int> center, ArenaMousePosition mousePosition, float radiusSubtraction = 0)
	{
		return GetEllipse(GetSnappedPosition(center).ToVector2(), GetSnappedPosition(mousePosition.Real).ToVector2(), radiusSubtraction);
	}

	private static ArenaEditingUtils.AlignedEllipse GetEllipse(Vector2 a, Vector2 b, float radiusSubtraction = 0)
	{
		Vector2 center = (a + b) * 0.5f;
		Vector2 radius = center - b;

		if (MathF.Abs(radius.X) > MathF.Abs(radiusSubtraction))
		{
			if (radius.X > 0)
				radius.X -= radiusSubtraction;
			else
				radius.X += radiusSubtraction;
		}
		else
		{
			radius.X = 0;
		}

		if (MathF.Abs(radius.Y) > MathF.Abs(radiusSubtraction))
		{
			if (radius.Y > 0)
				radius.Y -= radiusSubtraction;
			else
				radius.Y += radiusSubtraction;
		}
		else
		{
			radius.Y = 0;
		}

		return new(center, radius);
	}

	private static Vector2i<int> GetSnappedPosition(Vector2i<int> position)
	{
		return ArenaEditingUtils.Snap(position, Arena.TileSize) + Arena.HalfTile;
	}
}
