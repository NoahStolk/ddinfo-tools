using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using Hexa.NET.ImGui;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Timeline.EventTypes;

internal static class LeviathanSpawn
{
	public static void RenderEdit(int uniqueId, LeviathanSpawnEventData e, EditorReplayModel replay)
	{
		const float leftColumnWidth = 120;
		const float rightColumnWidth = 160;
		const float tableWidth = leftColumnWidth + rightColumnWidth;

		if (ImGui.BeginChild(Inline.Utf8($"LeviathanSpawnEdit{uniqueId}"), default, ImGuiChildFlags.AutoResizeY))
		{
			RenderTable(uniqueId, e, replay, tableWidth, leftColumnWidth, rightColumnWidth);
		}

		ImGui.EndChild();
	}

	private static void RenderTable(int uniqueId, LeviathanSpawnEventData e, EditorReplayModel replay, float tableWidth, float leftColumnWidth, float rightColumnWidth)
	{
		if (!ImGui.BeginTable("Left", 2, ImGuiTableFlags.None, new Vector2(tableWidth, 0)))
			return;

		ImGui.TableSetupColumn("LeftText", ImGuiTableColumnFlags.WidthFixed, leftColumnWidth);
		ImGui.TableSetupColumn("LeftInput", ImGuiTableColumnFlags.None, rightColumnWidth);

		ImGui.TableNextRow();

		ImGui.TableNextColumn();
		ImGui.Text("?");
		ImGui.TableNextColumn();
		UtilsRendering.EditableEntityId(uniqueId, "EntityId"u8, replay, ref e.A);

		ImGui.EndTable();
	}
}
