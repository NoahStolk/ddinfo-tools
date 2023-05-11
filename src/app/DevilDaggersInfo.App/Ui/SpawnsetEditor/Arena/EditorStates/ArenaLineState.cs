using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Ui.SpawnsetEditor.Arena.EditorChildren;
using DevilDaggersInfo.App.Ui.SpawnsetEditor.State;
using DevilDaggersInfo.App.Ui.SpawnsetEditor.Utils;
using DevilDaggersInfo.Core.Spawnset;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui.SpawnsetEditor.Arena.EditorStates;

public class ArenaLineState : IArenaState
{
	private Vector2? _lineStart;
	private float[,]? _newArena;

	public void Handle(ArenaMousePosition mousePosition)
	{
		if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
		{
			_lineStart = mousePosition.Real;
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
		if (!_lineStart.HasValue || _newArena == null)
			return;

		Loop(mousePosition, (i, j) => _newArena[i, j] = ArenaChild.SelectedHeight);

		SpawnsetState.Spawnset = SpawnsetState.Spawnset with { ArenaTiles = new(SpawnsetState.Spawnset.ArenaDimension, _newArena) };
		SpawnsetHistoryUtils.Save(SpawnsetEditType.ArenaLine);

		Reset();
	}

	private void Reset()
	{
		_lineStart = null;
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

		if (_lineStart.HasValue)
		{
			ArenaEditingUtils.Stadium stadium = GetStadium(_lineStart.Value, mousePosition);

			drawList.AddCircle(origin + stadium.Start, 2, ImGui.GetColorU32(Color.White));
			drawList.AddCircle(origin + stadium.End, 2, ImGui.GetColorU32(Color.White));

			drawList.AddCircle(origin + stadium.Start, stadium.Radius, ImGui.GetColorU32(Color.White));
			drawList.AddCircle(origin + stadium.End, stadium.Radius, ImGui.GetColorU32(Color.White));

			Vector2 delta = stadium.End - stadium.Start;
			drawList.AddLine(origin + stadium.Start, origin + stadium.End, ImGui.GetColorU32(Color.White));
			drawList.AddLine(origin + stadium.Edge1Point, origin + (stadium.Edge1Point + delta), ImGui.GetColorU32(Color.White));
			drawList.AddLine(origin + stadium.Edge2Point, origin + (stadium.Edge2Point + delta), ImGui.GetColorU32(Color.White));
		}
		else
		{
			drawList.AddCircle(origin + GetSnappedPosition(mousePosition.Real), GetDisplayRadius(), ImGui.GetColorU32(Color.HalfTransparentWhite));
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
				Vector2 visualTileCenter = new Vector2(i, j) * ArenaChild.TileSize + ArenaChild.HalfTileSizeAsVector2;

				ArenaEditingUtils.Square square = ArenaEditingUtils.Square.FromCenter(visualTileCenter, ArenaChild.TileSize);
				if (square.IntersectsStadium(stadium))
					action(i, j);
			}
		}
	}

	private static ArenaEditingUtils.Stadium GetStadium(Vector2 lineStart, ArenaMousePosition mousePosition)
	{
		return new(GetSnappedPosition(lineStart), GetSnappedPosition(mousePosition.Real), GetDisplayRadius());
	}

	private static float GetDisplayRadius()
	{
		return LineChild.Thickness / 2 * ArenaChild.TileSize;
	}

	private static Vector2 GetSnappedPosition(Vector2 position)
	{
		return ArenaEditingUtils.Snap(position, ArenaChild.TileSize) + ArenaChild.HalfTileSizeAsVector2;
	}
}
