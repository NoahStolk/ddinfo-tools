using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Arena.EditorChildren;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Utils;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Arena.EditorStates;

public class ArenaEllipseState : IArenaState
{
	private Session? _session;

	public void InitializeSession(ArenaMousePosition mousePosition)
	{
		_session ??= new Session(mousePosition.Real, FileStates.Spawnset.Object.ArenaTiles.GetMutableClone());
	}

	public void Handle(ArenaMousePosition mousePosition)
	{
		if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
			Emit(mousePosition);
	}

	public void HandleOutOfRange(ArenaMousePosition mousePosition)
	{
		if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
			Emit(mousePosition);
	}

	private void Emit(ArenaMousePosition mousePosition)
	{
		if (!_session.HasValue)
			return;

		Loop(mousePosition, (i, j) => _session.Value.NewArena[i, j] = ArenaWindow.SelectedHeight);

		FileStates.Spawnset.Update(FileStates.Spawnset.Object with { ArenaTiles = new ImmutableArena(FileStates.Spawnset.Object.ArenaDimension, _session.Value.NewArena) });
		SpawnsetHistoryUtils.Save(SpawnsetEditType.ArenaEllipse);

		Reset();
	}

	private void Reset()
	{
		_session = null;
	}

	public void Render(ArenaMousePosition mousePosition)
	{
		ImDrawListPtr drawList = ImGui.GetWindowDrawList();

		Vector2 origin = ImGui.GetCursorScreenPos();
		Loop(mousePosition, (i, j) =>
		{
			Vector2 topLeft = origin + new Vector2(i, j) * ArenaWindow.TileSize;
			drawList.AddRectFilled(topLeft, topLeft + new Vector2(ArenaWindow.TileSize), ImGui.GetColorU32(Color.HalfTransparentWhite));
		});

		if (!_session.HasValue)
			return;

		ArenaEditingUtils.AlignedEllipse ellipse = GetEllipse(_session.Value.StartPosition, mousePosition);

		drawList.AddEllipse(origin + ellipse.Center, ellipse.Radius, ImGui.GetColorU32(Color.HalfTransparentWhite), 40, 1);

		if (!EllipseChild.Filled)
		{
			ArenaEditingUtils.AlignedEllipse innerEllipse = GetEllipse(_session.Value.StartPosition, mousePosition, (EllipseChild.Thickness - 1) * ArenaWindow.TileSize);
			drawList.AddEllipse(origin + innerEllipse.Center, innerEllipse.Radius, ImGui.GetColorU32(Color.White), 40, 1);
		}
	}

	private void Loop(ArenaMousePosition mousePosition, Action<int, int> action)
	{
		if (!_session.HasValue)
			return;

		ArenaEditingUtils.AlignedEllipse ellipse = GetEllipse(_session.Value.StartPosition, mousePosition);

		if (EllipseChild.Filled)
		{
			for (int i = 0; i < FileStates.Spawnset.Object.ArenaDimension; i++)
			{
				for (int j = 0; j < FileStates.Spawnset.Object.ArenaDimension; j++)
				{
					Vector2 visualTileCenter = new Vector2(i, j) * ArenaWindow.TileSize + ArenaWindow.HalfTileSizeAsVector2;

					ArenaEditingUtils.Square square = ArenaEditingUtils.Square.FromCenter(visualTileCenter, ArenaWindow.TileSize);
					if (ellipse.Contains(square))
						action(i, j);
				}
			}
		}
		else
		{
			ArenaEditingUtils.AlignedEllipse innerEllipse = GetEllipse(_session.Value.StartPosition, mousePosition, (EllipseChild.Thickness - 1) * ArenaWindow.TileSize);
			for (int i = 0; i < FileStates.Spawnset.Object.ArenaDimension; i++)
			{
				for (int j = 0; j < FileStates.Spawnset.Object.ArenaDimension; j++)
				{
					Vector2 visualTileCenter = new Vector2(i, j) * ArenaWindow.TileSize + ArenaWindow.HalfTileSizeAsVector2;
					ArenaEditingUtils.Square square = ArenaEditingUtils.Square.FromCenter(visualTileCenter, ArenaWindow.TileSize);

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

		return new ArenaEditingUtils.AlignedEllipse(center, radius);
	}

	private static Vector2 GetSnappedPosition(Vector2 position)
	{
		return ArenaEditingUtils.Snap(position, ArenaWindow.TileSize) + ArenaWindow.HalfTileSizeAsVector2;
	}

	private struct Session
	{
		public Session(Vector2 startPosition, float[,] newArena)
		{
			StartPosition = startPosition;
			NewArena = newArena;
		}

		public Vector2 StartPosition { get; }

		public float[,] NewArena { get; }
	}
}
