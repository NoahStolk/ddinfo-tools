using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class SpiderEggSpawnEvents : IEventTypeRenderer<SpiderEggSpawnEventData>
{
	public static int ColumnCount => 5;
	public static int ColumnCountData => 3;

	public static void SetupColumns()
	{
		EventTypeRendererUtils.SetupColumnIndex();
		EventTypeRendererUtils.SetupColumnEntityId();
		SetupColumnsData();
	}

	public static void SetupColumnsData()
	{
		ImGui.TableSetupColumn("Spawner Entity Id", ImGuiTableColumnFlags.WidthFixed, 160);
		ImGui.TableSetupColumn("Position", ImGuiTableColumnFlags.WidthFixed, 192);
		ImGui.TableSetupColumn("Target Position", ImGuiTableColumnFlags.WidthFixed, 192);
	}

	public static void Render(int eventIndex, int entityId, SpiderEggSpawnEventData e, EditorReplayModel replay)
	{
		EventTypeRendererUtils.NextColumnEventIndex(eventIndex);
		EventTypeRendererUtils.NextColumnEntityId(replay, entityId);
		RenderData(eventIndex, e, replay);
	}

	public static void RenderData(int eventIndex, SpiderEggSpawnEventData e, EditorReplayModel replay)
	{
		EventTypeRendererUtils.NextColumnEditableEntityId(eventIndex, nameof(SpiderEggSpawnEventData.SpawnerEntityId), replay, ref e.SpawnerEntityId);
		EventTypeRendererUtils.NextColumnInputVector3(eventIndex, nameof(SpiderEggSpawnEventData.Position), ref e.Position, "%.2f");
		EventTypeRendererUtils.NextColumnInputVector3(eventIndex, nameof(SpiderEggSpawnEventData.TargetPosition), ref e.TargetPosition, "%.2f");
	}

	public static void RenderEdit(int uniqueId, SpiderEggSpawnEventData e, EditorReplayModel replay)
	{
		const float leftColumnWidth = 120;
		const float rightColumnWidth = 160;
		const float tableWidth = leftColumnWidth + rightColumnWidth;

		if (ImGui.BeginChild(Inline.Span($"SpiderEggSpawnEdit{uniqueId}"), default, ImGuiChildFlags.AutoResizeY))
		{
			if (ImGui.BeginTable("Left", 2, ImGuiTableFlags.None, new(tableWidth, 0)))
			{
				ImGui.TableSetupColumn("LeftText", ImGuiTableColumnFlags.WidthFixed, leftColumnWidth);
				ImGui.TableSetupColumn("LeftInput", ImGuiTableColumnFlags.None, rightColumnWidth);

				ImGui.TableNextRow();

				ImGui.TableNextColumn();
				ImGui.Text("Spawner Entity Id");
				ImGui.TableNextColumn();
				EventTypeRendererUtils.EditableEntityId(uniqueId, nameof(SpiderEggSpawnEventData.SpawnerEntityId), replay, ref e.SpawnerEntityId);

				ImGui.TableNextColumn();
				ImGui.Text("Position");
				ImGui.TableNextColumn();
				EventTypeRendererUtils.InputVector3(uniqueId, nameof(SpiderEggSpawnEventData.Position), ref e.Position, "%.2f");

				ImGui.TableNextColumn();
				ImGui.Text("Target Position");
				ImGui.TableNextColumn();
				EventTypeRendererUtils.InputVector3(uniqueId, nameof(SpiderEggSpawnEventData.TargetPosition), ref e.TargetPosition, "%.2f");

				ImGui.EndTable();
			}
		}

		ImGui.EndChild();
	}
}
