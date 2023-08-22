using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Ui.SpawnsetEditor.Arena.EditorChildren;
using DevilDaggersInfo.App.Ui.SpawnsetEditor.State;
using DevilDaggersInfo.App.Ui.SpawnsetEditor.Utils;
using DevilDaggersInfo.Core.Spawnset;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui.SpawnsetEditor.Arena.EditorStates;

public class ArenaDaggerState : IArenaState
{
	private Vector2? _position;

	public void Handle(ArenaMousePosition mousePosition)
	{
		if (SpawnsetState.Spawnset.GameMode != GameMode.Race)
			return;

		if (ImGui.IsMouseDown(ImGuiMouseButton.Left))
		{
			_position = GetSnappedDaggerPosition();
		}
		else if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
		{
			if (!_position.HasValue)
				return;

			Vector2 tileCoordinate = _position.Value / ArenaChild.TileSize;
			Vector2 daggerPosition = new(SpawnsetState.Spawnset.TileToWorldCoordinate(tileCoordinate.X), SpawnsetState.Spawnset.TileToWorldCoordinate(tileCoordinate.Y));

			SpawnsetState.Spawnset = SpawnsetState.Spawnset with { RaceDaggerPosition = daggerPosition };
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
