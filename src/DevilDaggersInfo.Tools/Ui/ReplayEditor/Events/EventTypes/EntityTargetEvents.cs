using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class EntityTargetEvents : IEventTypeRenderer<EntityTargetEventData>
{
	public static int ColumnCount => 3;
	public static int ColumnCountData => 2;

	public static void SetupColumns()
	{
		EventTypeRendererUtils.SetupColumnIndex();
		SetupColumnsData();
	}

	public static void SetupColumnsData()
	{
		EventTypeRendererUtils.SetupColumnEntityId();
		ImGui.TableSetupColumn("Target Position", ImGuiTableColumnFlags.WidthFixed, 128);
	}

	public static void Render(int eventIndex, int entityId, EntityTargetEventData e, EditorReplayModel replay)
	{
		EventTypeRendererUtils.NextColumnEventIndex(eventIndex);
		RenderData(eventIndex, e, replay);
	}

	public static void RenderData(int eventIndex, EntityTargetEventData e, EditorReplayModel replay)
	{
		EventTypeRendererUtils.NextColumnEditableEntityId(eventIndex, nameof(EntityTargetEventData.EntityId), replay, ref e.EntityId);
		EventTypeRendererUtils.NextColumnInputInt16Vec3(eventIndex, nameof(EntityTargetEventData.TargetPosition), ref e.TargetPosition);
	}

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
				EventTypeRendererUtils.EditableEntityId(uniqueId, nameof(EntityTargetEventData.EntityId), replay, ref e.EntityId);

				ImGui.TableNextColumn();
				ImGui.Text("Target Position");
				ImGui.TableNextColumn();
				EventTypeRendererUtils.InputInt16Vec3(uniqueId, nameof(EntityTargetEventData.TargetPosition), ref e.TargetPosition);

				ImGui.EndTable();
			}
		}

		ImGui.EndChild();
	}
}
