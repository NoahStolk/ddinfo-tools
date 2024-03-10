using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Arena.EditorChildren;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Utils;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Arena.EditorStates;

public class ArenaLineState : IArenaState
{
	private Session? _session;

	public void InitializeSession(ArenaMousePosition mousePosition)
	{
		_session ??= new(mousePosition.Real, FileStates.Spawnset.Object.ArenaTiles.GetMutableClone());
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
		if (_session == null)
			return;

		Loop(mousePosition, (i, j) => _session.Value.NewArena[i, j] = ArenaWindow.SelectedHeight);

		FileStates.Spawnset.Update(FileStates.Spawnset.Object with { ArenaTiles = new(FileStates.Spawnset.Object.ArenaDimension, _session.Value.NewArena) });
		SpawnsetHistoryUtils.Save(SpawnsetEditType.ArenaLine);

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

		if (_session.HasValue)
		{
			ArenaEditingUtils.Stadium stadium = GetStadium(_session.Value.StartPosition, mousePosition);

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
		if (!_session.HasValue)
			return;

		ArenaEditingUtils.Stadium stadium = GetStadium(_session.Value.StartPosition, mousePosition);
		for (int i = 0; i < SpawnsetBinary.ArenaDimensionMax; i++)
		{
			for (int j = 0; j < SpawnsetBinary.ArenaDimensionMax; j++)
			{
				Vector2 visualTileCenter = new Vector2(i, j) * ArenaWindow.TileSize + ArenaWindow.HalfTileSizeAsVector2;

				ArenaEditingUtils.Square square = ArenaEditingUtils.Square.FromCenter(visualTileCenter, ArenaWindow.TileSize);
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
		return LineChild.Thickness / 2 * ArenaWindow.TileSize;
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
