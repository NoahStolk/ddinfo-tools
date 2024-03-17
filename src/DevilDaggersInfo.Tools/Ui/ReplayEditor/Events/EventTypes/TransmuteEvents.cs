using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class TransmuteEvents : IEventTypeRenderer<TransmuteEventData>
{
	public static int ColumnCount => 6;
	public static int ColumnCountData => 5;

	public static void SetupColumns()
	{
		EventTypeRendererUtils.SetupColumnIndex();
		SetupColumnsData();
	}

	public static void SetupColumnsData()
	{
		EventTypeRendererUtils.SetupColumnEntityId();
		ImGui.TableSetupColumn("?", ImGuiTableColumnFlags.WidthFixed, 128);
		ImGui.TableSetupColumn("?", ImGuiTableColumnFlags.WidthFixed, 128);
		ImGui.TableSetupColumn("?", ImGuiTableColumnFlags.WidthFixed, 128);
		ImGui.TableSetupColumn("?", ImGuiTableColumnFlags.WidthFixed, 128);
	}

	public static void Render(int eventIndex, int entityId, TransmuteEventData e, EditorReplayModel replay)
	{
		EventTypeRendererUtils.NextColumnEventIndex(eventIndex);
		RenderData(eventIndex, e, replay);
	}

	public static void RenderData(int eventIndex, TransmuteEventData e, EditorReplayModel replay)
	{
		EventTypeRendererUtils.NextColumnEditableEntityId(eventIndex, nameof(TransmuteEventData.EntityId), replay, ref e.EntityId);
		EventTypeRendererUtils.NextColumnInputInt16Vec3(eventIndex, nameof(TransmuteEventData.A), ref e.A);
		EventTypeRendererUtils.NextColumnInputInt16Vec3(eventIndex, nameof(TransmuteEventData.B), ref e.B);
		EventTypeRendererUtils.NextColumnInputInt16Vec3(eventIndex, nameof(TransmuteEventData.C), ref e.C);
		EventTypeRendererUtils.NextColumnInputInt16Vec3(eventIndex, nameof(TransmuteEventData.D), ref e.D);
	}

	public static void RenderEdit(int uniqueId, TransmuteEventData e, EditorReplayModel replay)
	{
		const float leftColumnWidth = 120;
		const float rightColumnWidth = 160;
		const float tableWidth = leftColumnWidth + rightColumnWidth;

		if (ImGui.BeginChild(Inline.Span($"TransmuteEdit{uniqueId}"), default, ImGuiChildFlags.AutoResizeY))
		{
			if (ImGui.BeginTable("Left", 2, ImGuiTableFlags.None, new(tableWidth, 0)))
			{
				ImGui.TableSetupColumn("LeftText", ImGuiTableColumnFlags.WidthFixed, leftColumnWidth);
				ImGui.TableSetupColumn("LeftInput", ImGuiTableColumnFlags.None, rightColumnWidth);

				ImGui.TableNextRow();

				ImGui.TableNextColumn();
				ImGui.Text("Entity Id");
				ImGui.TableNextColumn();
				EventTypeRendererUtils.EditableEntityId(uniqueId, nameof(TransmuteEventData.EntityId), replay, ref e.EntityId);

				ImGui.TableNextColumn();
				ImGui.Text("?");
				ImGui.TableNextColumn();
				EventTypeRendererUtils.InputInt16Vec3(uniqueId, nameof(TransmuteEventData.A), ref e.A);

				ImGui.TableNextColumn();
				ImGui.Text("?");
				ImGui.TableNextColumn();
				EventTypeRendererUtils.InputInt16Vec3(uniqueId, nameof(TransmuteEventData.B), ref e.B);

				ImGui.TableNextColumn();
				ImGui.Text("?");
				ImGui.TableNextColumn();
				EventTypeRendererUtils.InputInt16Vec3(uniqueId, nameof(TransmuteEventData.C), ref e.C);

				ImGui.TableNextColumn();
				ImGui.Text("?");
				ImGui.TableNextColumn();
				EventTypeRendererUtils.InputInt16Vec3(uniqueId, nameof(TransmuteEventData.D), ref e.D);

				ImGui.EndTable();
			}

			ImGui.SameLine();
		}

		ImGui.EndChild();
	}
}
