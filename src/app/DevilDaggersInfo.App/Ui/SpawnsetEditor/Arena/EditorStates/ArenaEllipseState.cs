using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Extensions;
using DevilDaggersInfo.App.Ui.SpawnsetEditor.Arena.EditorChildren;
using DevilDaggersInfo.App.Ui.SpawnsetEditor.State;
using DevilDaggersInfo.App.Ui.SpawnsetEditor.Utils;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui.SpawnsetEditor.Arena.EditorStates;

public class ArenaEllipseState : IArenaState
{
	private Vector2? _ellipseStart;
	private float[,]? _newArena;

	public void Handle(ArenaMousePosition mousePosition)
	{
		if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
		{
			_ellipseStart = mousePosition.Real;
			_newArena = SpawnsetState.Spawnset.ArenaTiles.GetMutableClone();
		}
		else if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
		{
			Emit(mousePosition);
		}
	}

	public void HandleOutOfRange(ArenaMousePosition mousePosition)
	{
		if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
			Emit(mousePosition);
	}

	private void Emit(ArenaMousePosition mousePosition)
	{
		if (!_ellipseStart.HasValue || _newArena == null)
			return;

		Loop(mousePosition, (i, j) => _newArena[i, j] = ArenaChild.SelectedHeight);

		SpawnsetState.Spawnset = SpawnsetState.Spawnset with { ArenaTiles = new(SpawnsetState.Spawnset.ArenaDimension, _newArena) };
		SpawnsetHistoryUtils.Save(SpawnsetEditType.ArenaEllipse);

		Reset();
	}

	private void Reset()
	{
		_ellipseStart = null;
		_newArena = null;
	}

	public void Render(ArenaMousePosition mousePosition)
	{
		ImDrawListPtr drawList = ImGui.GetWindowDrawList();

		Vector2 origin = ImGui.GetCursorScreenPos();
		Loop(mousePosition, (i, j) =>
		{
			Vector2 topLeft = origin + new Vector2(i, j) * ArenaChild.TileSize;
			drawList.AddRectFilled(topLeft, topLeft + new Vector2(ArenaChild.TileSize), ImGui.GetColorU32(Color.HalfTransparentWhite));
		});

		if (!_ellipseStart.HasValue)
			return;

		ArenaEditingUtils.AlignedEllipse ellipse = GetEllipse(_ellipseStart.Value, mousePosition);

		drawList.AddEllipse(origin + ellipse.Center, ellipse.Radius, ImGui.GetColorU32(Color.HalfTransparentWhite), 40, 1);

		if (!EllipseChild.Filled)
		{
			ArenaEditingUtils.AlignedEllipse innerEllipse = GetEllipse(_ellipseStart.Value, mousePosition, (EllipseChild.Thickness - 1) * ArenaChild.TileSize);
			drawList.AddEllipse(origin + innerEllipse.Center, innerEllipse.Radius, ImGui.GetColorU32(Color.White), 40, 1);
		}
	}

	private void Loop(ArenaMousePosition mousePosition, Action<int, int> action)
	{
		if (!_ellipseStart.HasValue)
			return;

		ArenaEditingUtils.AlignedEllipse ellipse = GetEllipse(_ellipseStart.Value, mousePosition);

		if (EllipseChild.Filled)
		{
			for (int i = 0; i < SpawnsetState.Spawnset.ArenaDimension; i++)
			{
				for (int j = 0; j < SpawnsetState.Spawnset.ArenaDimension; j++)
				{
					Vector2 visualTileCenter = new Vector2(i, j) * ArenaChild.TileSize + ArenaChild.HalfTileSizeAsVector2;

					ArenaEditingUtils.Square square = ArenaEditingUtils.Square.FromCenter(visualTileCenter, ArenaChild.TileSize);
					if (ellipse.Contains(square))
						action(i, j);
				}
			}
		}
		else
		{
			ArenaEditingUtils.AlignedEllipse innerEllipse = GetEllipse(_ellipseStart.Value, mousePosition, (EllipseChild.Thickness - 1) * ArenaChild.TileSize);
			for (int i = 0; i < SpawnsetState.Spawnset.ArenaDimension; i++)
			{
				for (int j = 0; j < SpawnsetState.Spawnset.ArenaDimension; j++)
				{
					Vector2 visualTileCenter = new Vector2(i, j) * ArenaChild.TileSize + ArenaChild.HalfTileSizeAsVector2;
					ArenaEditingUtils.Square square = ArenaEditingUtils.Square.FromCenter(visualTileCenter, ArenaChild.TileSize);

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

	private static ArenaEditingUtils.AlignedEllipse GetEllipse(Vector2 center, ArenaMousePosition mousePosition, float radiusSubtraction = 0)
	{
		return GetEllipse(GetSnappedPosition(center), GetSnappedPosition(mousePosition.Real), radiusSubtraction);
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

	private static Vector2 GetSnappedPosition(Vector2 position)
	{
		return ArenaEditingUtils.Snap(position, ArenaChild.TileSize) + ArenaChild.HalfTileSizeAsVector2;
	}
}
