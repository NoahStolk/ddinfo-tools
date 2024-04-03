using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Timeline.EventTypes;

public static class EntityTarget
{
	public static void RenderEdit(int uniqueId, EntityTargetEventData e, EditorReplayModel replay)
	{
		const float leftColumnWidth = 120;
		const float rightColumnWidth = 160;
		const float tableWidth = leftColumnWidth + rightColumnWidth;

		if (ImGui.BeginChild(Inline.Span($"EntityTargetEdit{uniqueId}"), default, ImGuiChildFlags.AutoResizeY))
		{
			if (ImGui.BeginTable("Left", 2, ImGuiTableFlags.None, new(tableWidth, 0)))
			{
				ImGui.TableSetupColumn("LeftText", ImGuiTableColumnFlags.WidthFixed, leftColumnWidth);
				ImGui.TableSetupColumn("LeftInput", ImGuiTableColumnFlags.None, rightColumnWidth);

				ImGui.TableNextRow();

				ImGui.TableNextColumn();
				ImGui.Text("Entity Id");
				ImGui.TableNextColumn();
				UtilsRendering.EditableEntityId(uniqueId, nameof(EntityTargetEventData.EntityId), replay, ref e.EntityId);

				ImGui.TableNextColumn();
				ImGui.Text("Target Position");
				ImGui.TableNextColumn();
				UtilsRendering.InputInt16Vec3(uniqueId, nameof(EntityTargetEventData.TargetPosition), ref e.TargetPosition);

				ImGui.EndTable();
			}
		}

		ImGui.EndChild();
	}
}
