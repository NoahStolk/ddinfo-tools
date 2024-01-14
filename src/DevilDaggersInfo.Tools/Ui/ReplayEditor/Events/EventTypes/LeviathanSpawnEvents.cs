using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events.Data;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class LeviathanSpawnEvents : IEventTypeRenderer<LeviathanSpawnEventData>
{
	public static int ColumnCount => 4;
	public static int ColumnCountData => 1;

	public static void SetupColumns()
	{
		EventTypeRendererUtils.SetupColumnActions();
		EventTypeRendererUtils.SetupColumnIndex();
		EventTypeRendererUtils.SetupColumnEntityId();
		SetupColumnsData();
	}

	public static void SetupColumnsData()
	{
		ImGui.TableSetupColumn("?", ImGuiTableColumnFlags.WidthFixed, 32);
	}

	public static void Render(int eventIndex, int entityId, LeviathanSpawnEventData e, ReplayEventsData replayEventsData)
	{
		EventTypeRendererUtils.NextColumnActions(eventIndex);
		EventTypeRendererUtils.NextColumnEventIndex(eventIndex);
		EventTypeRendererUtils.NextColumnEntityId(replayEventsData, entityId);
		RenderData(eventIndex, e, replayEventsData);
	}

	public static void RenderData(int eventIndex, LeviathanSpawnEventData e, ReplayEventsData replayEventsData)
	{
		EventTypeRendererUtils.NextColumnInputInt(eventIndex, nameof(LeviathanSpawnEventData.A), ref e.A);
	}

	public static void RenderEdit(int eventIndex, LeviathanSpawnEventData e, ReplayEventsData replayEventsData)
	{
		const float leftColumnWidth = 120;
		const float rightColumnWidth = 160;
		const float tableWidth = leftColumnWidth + rightColumnWidth;

		if (ImGui.BeginChild(Inline.Span($"LeviathanSpawnEdit{eventIndex}"), default, ImGuiChildFlags.AutoResizeY))
		{
			if (ImGui.BeginTable("Left", 2, ImGuiTableFlags.None, new(tableWidth, 0)))
			{
				ImGui.TableSetupColumn("LeftText", ImGuiTableColumnFlags.WidthFixed, leftColumnWidth);
				ImGui.TableSetupColumn("LeftInput", ImGuiTableColumnFlags.None, rightColumnWidth);

				ImGui.TableNextRow();

				ImGui.TableNextColumn();
				ImGui.Text("?");
				ImGui.TableNextColumn();
				EventTypeRendererUtils.EditableEntityId(eventIndex, nameof(EntityTargetEventData.EntityId), replayEventsData, ref e.A);

				ImGui.EndTable();
			}
		}

		ImGui.EndChild();
	}
}
