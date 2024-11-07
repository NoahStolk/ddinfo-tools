using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Arena.EditorChildren;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Utils;
using ImGuiNET;
using Silk.NET.Maths;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Arena.EditorStates;

public sealed class ArenaRectangleState : IArenaState
{
	private readonly ArenaWindow _arenaWindow;

	private Session? _session;

	public ArenaRectangleState(ArenaWindow arenaWindow)
	{
		_arenaWindow = arenaWindow;
	}

	public void InitializeSession(ArenaMousePosition mousePosition)
	{
		_session ??= new Session(mousePosition.Tile, FileStates.Spawnset.Object.ArenaTiles.GetMutableClone());
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

		Loop(mousePosition, (i, j) => _session.Value.NewArena[i, j] = _arenaWindow.SelectedHeight);

		FileStates.Spawnset.Update(FileStates.Spawnset.Object with { ArenaTiles = new ImmutableArena(FileStates.Spawnset.Object.ArenaDimension, _session.Value.NewArena) });
		SpawnsetHistoryUtils.Save(SpawnsetEditType.ArenaRectangle);

		Reset();
	}

	private void Reset()
	{
		_session = null;
	}

	public void Render(ArenaMousePosition mousePosition)
	{
		ImDrawListPtr drawList = ImGui.GetWindowDrawList();

		Loop(mousePosition, (i, j) =>
		{
			Vector2 origin = ImGui.GetCursorScreenPos();
			Vector2 min = origin + new Vector2(i, j) * ArenaWindow.TileSize;
			drawList.AddRectFilled(min, min + new Vector2(ArenaWindow.TileSize), ImGui.GetColorU32(Color.HalfTransparentWhite));
		});
	}

	private void Loop(ArenaMousePosition mousePosition, Action<int, int> action)
	{
		if (!_session.HasValue)
			return;

		ArenaEditingUtils.Rectangle rectangle = ArenaEditingUtils.GetRectangle(_session.Value.StartPosition, mousePosition.Tile);

		int startXa = Math.Max(0, rectangle.X1);
		int startYa = Math.Max(0, rectangle.Y1);
		int endXa = Math.Min(FileStates.Spawnset.Object.ArenaDimension - 1, rectangle.X2);
		int endYa = Math.Min(FileStates.Spawnset.Object.ArenaDimension - 1, rectangle.Y2);

		if (RectangleChild.Filled)
		{
			for (int i = startXa; i <= endXa; i++)
			{
				for (int j = startYa; j <= endYa; j++)
					action(i, j);
			}
		}
		else
		{
			int addedSize = RectangleChild.Thickness;
			int startXb = Math.Max(0, rectangle.X1 + addedSize);
			int startYb = Math.Max(0, rectangle.Y1 + addedSize);
			int endXb = Math.Min(FileStates.Spawnset.Object.ArenaDimension - 1, rectangle.X2 - addedSize);
			int endYb = Math.Min(FileStates.Spawnset.Object.ArenaDimension - 1, rectangle.Y2 - addedSize);

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

	private struct Session
	{
		public Session(Vector2D<int> startPosition, float[,] newArena)
		{
			StartPosition = startPosition;
			NewArena = newArena;
		}

		public Vector2D<int> StartPosition { get; }

		public float[,] NewArena { get; }
	}
}
