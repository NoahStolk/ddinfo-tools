using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Arena.EditorChildren;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.State;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Utils;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Arena.EditorStates;

public class ArenaDaggerState : IArenaState
{
	private Vector2? _position;
	private bool _dragging;

	public void InitializeSession(ArenaMousePosition mousePosition)
	{
		_dragging = true;
	}

	public void Handle(ArenaMousePosition mousePosition)
	{
		if (FileStates.Spawnset.Object.GameMode != GameMode.Race)
			return;

		if (_dragging && ImGui.IsMouseDown(ImGuiMouseButton.Left))
		{
			_position = GetSnappedDaggerPosition();
		}
		else if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
		{
			if (!_position.HasValue)
				return;

			Vector2 tileCoordinate = _position.Value / ArenaChild.TileSize;
			Vector2 daggerPosition = new(FileStates.Spawnset.Object.TileToWorldCoordinate(tileCoordinate.X), FileStates.Spawnset.Object.TileToWorldCoordinate(tileCoordinate.Y));

			FileStates.Spawnset.Update(FileStates.Spawnset.Object with { RaceDaggerPosition = daggerPosition });
			SpawnsetHistoryUtils.Save(SpawnsetEditType.RaceDagger);

			Reset();
		}

		Vector2 GetSnappedDaggerPosition()
		{
			return ArenaEditingUtils.Snap(mousePosition.Real, DaggerChild.Snap * ArenaChild.TileSize);
		}
	}

	public void HandleOutOfRange(ArenaMousePosition mousePosition)
	{
		Reset();
	}

	private void Reset()
	{
		_position = null;
		_dragging = false;
	}

	public void Render(ArenaMousePosition mousePosition)
	{
		if (!_position.HasValue)
			return;

		ImDrawListPtr drawList = ImGui.GetWindowDrawList();
		Vector2 origin = ImGui.GetCursorScreenPos();
		Vector2 center = origin + _position.Value + ArenaChild.HalfTileSizeAsVector2;
		drawList.AddImage(Root.GameResources.IconMaskDaggerTexture.Handle, center - new Vector2(8), center + new Vector2(8), Color.HalfTransparentWhite);
	}
}
