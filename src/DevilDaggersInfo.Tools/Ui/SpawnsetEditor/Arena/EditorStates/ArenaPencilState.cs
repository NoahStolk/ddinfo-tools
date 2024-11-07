using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Arena.EditorChildren;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Utils;
using ImGuiNET;
using Silk.NET.Maths;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Arena.EditorStates;

public sealed class ArenaPencilState : IArenaState
{
	private readonly ArenaWindow _arenaWindow;

	private Session? _session;

	public ArenaPencilState(ArenaWindow arenaWindow)
	{
		_arenaWindow = arenaWindow;
	}

	public void InitializeSession(ArenaMousePosition mousePosition)
	{
		_session ??= new Session(mousePosition.Real);
	}

	public void Handle(ArenaMousePosition mousePosition)
	{
		if (_session != null && ImGui.IsMouseDown(ImGuiMouseButton.Left))
		{
			Vector2 pencilEnd = mousePosition.Real;
			Vector2 start = ArenaEditingUtils.Snap(_session.StartPosition, ArenaWindow.TileSize) + ArenaWindow.HalfTileSizeAsVector2;
			Vector2 end = ArenaEditingUtils.Snap(pencilEnd, ArenaWindow.TileSize) + ArenaWindow.HalfTileSizeAsVector2;
			ArenaEditingUtils.Stadium stadium = new(start, end, PencilChild.Size / 2 * ArenaWindow.TileSize);
			for (int i = 0; i < SpawnsetBinary.ArenaDimensionMax; i++)
			{
				for (int j = 0; j < SpawnsetBinary.ArenaDimensionMax; j++)
				{
					Vector2D<int> target = new(i, j);
					if (_session.ModifiedCoords.Contains(target)) // Early rejection, even though we're using a HashSet.
						continue;

					Vector2 visualTileCenter = new Vector2(i, j) * ArenaWindow.TileSize + ArenaWindow.HalfTileSizeAsVector2;

					ArenaEditingUtils.Square square = ArenaEditingUtils.Square.FromCenter(visualTileCenter, ArenaWindow.TileSize);
					if (square.IntersectsStadium(stadium))
						_session.ModifiedCoords.Add(target);
				}
			}

			_session.ModifiedCoords.Add(new Vector2D<int>(mousePosition.Tile.X, mousePosition.Tile.Y));
			_session.StartPosition = mousePosition.Real;
		}
		else if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
		{
			Emit();
		}
	}

	public void HandleOutOfRange(ArenaMousePosition mousePosition)
	{
		// When out of range, continue setting the position of the pencil if the user is still using it.
		if (_session != null && ImGui.IsMouseDown(ImGuiMouseButton.Left))
			_session.StartPosition = mousePosition.Real;
		else if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
			Emit();
	}

	private void Emit()
	{
		if (_session == null)
			return;

		float[,] newArena = FileStates.Spawnset.Object.ArenaTiles.GetMutableClone();

		foreach (Vector2D<int> position in _session.ModifiedCoords)
			newArena[position.X, position.Y] = _arenaWindow.SelectedHeight;

		FileStates.Spawnset.Update(FileStates.Spawnset.Object with { ArenaTiles = new ImmutableArena(FileStates.Spawnset.Object.ArenaDimension, newArena) });
		SpawnsetHistoryUtils.Save(SpawnsetEditType.ArenaPencil);

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
		drawList.AddCircleFilled(origin + GetSnappedPosition(mousePosition.Real), GetDisplayRadius(), ImGui.GetColorU32(_session != null ? Color.White : Color.HalfTransparentWhite));

		if (_session == null)
			return;

		for (int i = 0; i < SpawnsetBinary.ArenaDimensionMax; i++)
		{
			for (int j = 0; j < SpawnsetBinary.ArenaDimensionMax; j++)
			{
				if (_session.ModifiedCoords.Contains(new Vector2D<int>(i, j)))
				{
					Vector2 topLeft = origin + new Vector2(i, j) * ArenaWindow.TileSize;
					drawList.AddRectFilled(topLeft, topLeft + new Vector2(ArenaWindow.TileSize), ImGui.GetColorU32(Color.HalfTransparentWhite));
				}
			}
		}
	}

	private static float GetDisplayRadius()
	{
		return PencilChild.Size / 2 * ArenaWindow.TileSize;
	}

	private static Vector2 GetSnappedPosition(Vector2 position)
	{
		return ArenaEditingUtils.Snap(position, ArenaWindow.TileSize) + ArenaWindow.HalfTileSizeAsVector2;
	}

	// Must be a class so we can modify the properties.
	private sealed class Session
	{
		public Session(Vector2 startPosition)
		{
			StartPosition = startPosition;
		}

		public Vector2 StartPosition { get; set; }

		public HashSet<Vector2D<int>> ModifiedCoords { get; } = [];
	}
}
