using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using Hexa.NET.ImGui;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Timeline.EventTypes;

internal static class SpiderEggSpawn
{
	public static void RenderEdit(int uniqueId, SpiderEggSpawnEventData e, EditorReplayModel replay)
	{
		const float leftColumnWidth = 120;
		const float rightColumnWidth = 160;
		const float tableWidth = leftColumnWidth + rightColumnWidth;

		if (ImGui.BeginChild(Inline.Utf8($"SpiderEggSpawnEdit{uniqueId}"), default, ImGuiChildFlags.AutoResizeY))
		{
			if (ImGui.BeginTable("Left", 2, ImGuiTableFlags.None, new Vector2(tableWidth, 0)))
			{
				ImGui.TableSetupColumn("LeftText", ImGuiTableColumnFlags.WidthFixed, leftColumnWidth);
				ImGui.TableSetupColumn("LeftInput", ImGuiTableColumnFlags.None, rightColumnWidth);

				ImGui.TableNextRow();

				ImGui.TableNextColumn();
				ImGui.Text("Spawner Entity Id");
				ImGui.TableNextColumn();
				UtilsRendering.EditableEntityId(uniqueId, "SpawnerEntityId"u8, replay, ref e.SpawnerEntityId);

				ImGui.TableNextColumn();
				ImGui.Text("Position");
				ImGui.TableNextColumn();
				UtilsRendering.InputVector3(uniqueId, "Position"u8, ref e.Position, "%.2f"u8);

				ImGui.TableNextColumn();
				ImGui.Text("Target Position");
				ImGui.TableNextColumn();
				UtilsRendering.InputVector3(uniqueId, "TargetPosition"u8, ref e.TargetPosition, "%.2f"u8);

				ImGui.EndTable();
			}
		}

		ImGui.EndChild();
	}
}
