using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class EntityOrientationEvents : IEventTypeRenderer<EntityOrientationEventData>
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
		ImGui.TableSetupColumn("Orientation", ImGuiTableColumnFlags.WidthFixed, 384);
	}

	public static void Render(int eventIndex, int entityId, EntityOrientationEventData e, EditorReplayModel replay)
	{
		EventTypeRendererUtils.NextColumnEventIndex(eventIndex);
		RenderData(eventIndex, e, replay);
	}

	public static void RenderData(int eventIndex, EntityOrientationEventData e, EditorReplayModel replay)
	{
		EventTypeRendererUtils.NextColumnEditableEntityId(eventIndex, nameof(EntityOrientationEventData.EntityId), replay, ref e.EntityId);
		EventTypeRendererUtils.NextColumnInputInt16Mat3x3(eventIndex, nameof(EntityOrientationEventData.Orientation), ref e.Orientation);
	}

	public static void RenderEdit(int uniqueId, EntityOrientationEventData e, EditorReplayModel replay)
	{
		const float leftColumnWidth = 120;
		const float rightColumnWidth = 160;
		const float tableWidth = leftColumnWidth + rightColumnWidth;

		if (ImGui.BeginChild(Inline.Span($"EntityOrientationEdit{uniqueId}"), default, ImGuiChildFlags.AutoResizeY))
		{
			if (ImGui.BeginTable("Left", 2, ImGuiTableFlags.None, new(tableWidth, 0)))
			{
				ImGui.TableSetupColumn("LeftText", ImGuiTableColumnFlags.WidthFixed, leftColumnWidth);
				ImGui.TableSetupColumn("LeftInput", ImGuiTableColumnFlags.None, rightColumnWidth);

				ImGui.TableNextRow();

				ImGui.TableNextColumn();
				ImGui.Text("Entity Id");
				ImGui.TableNextColumn();
				EventTypeRendererUtils.EditableEntityId(uniqueId, nameof(EntityOrientationEventData.EntityId), replay, ref e.EntityId);

				ImGui.TableNextColumn();
				ImGui.Text("Orientation");
				ImGui.TableNextColumn();
				EventTypeRendererUtils.InputInt16Mat3x3Square(uniqueId, nameof(EntityOrientationEventData.Orientation), ref e.Orientation);

				ImGui.EndTable();
			}
		}

		ImGui.EndChild();
	}
}
