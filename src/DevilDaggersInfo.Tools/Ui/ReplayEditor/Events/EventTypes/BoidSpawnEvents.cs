using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class BoidSpawnEvents : IEventTypeRenderer<BoidSpawnEventData>
{
	private static readonly string[] _boidTypeNamesArray = EnumUtils.BoidTypeNames.Values.ToArray();

	public static int ColumnCount => 9;
	public static int ColumnCountData => 6;

	public static void SetupColumns()
	{
		EventTypeRendererUtils.SetupColumnActions();
		EventTypeRendererUtils.SetupColumnIndex();
		EventTypeRendererUtils.SetupColumnEntityId();
		SetupColumnsData();
	}

	public static void SetupColumnsData()
	{
		ImGui.TableSetupColumn("Spawner Entity Id", ImGuiTableColumnFlags.WidthFixed, 160);
		ImGui.TableSetupColumn("Type", ImGuiTableColumnFlags.WidthFixed, 96);
		ImGui.TableSetupColumn("Position", ImGuiTableColumnFlags.WidthFixed, 96);
		ImGui.TableSetupColumn("Orientation", ImGuiTableColumnFlags.None, 196);
		ImGui.TableSetupColumn("Velocity", ImGuiTableColumnFlags.WidthFixed, 128);
		ImGui.TableSetupColumn("Speed", ImGuiTableColumnFlags.WidthFixed, 64);
	}

	public static void Render(int eventIndex, int entityId, BoidSpawnEventData e, ReplayEventsData replayEventsData)
	{
		EventTypeRendererUtils.NextColumnActions(eventIndex);
		EventTypeRendererUtils.NextColumnEventIndex(eventIndex);
		EventTypeRendererUtils.NextColumnEntityId(replayEventsData, entityId);
		RenderData(eventIndex, e, replayEventsData);
	}

	public static void RenderData(int eventIndex, BoidSpawnEventData e, ReplayEventsData replayEventsData)
	{
		EventTypeRendererUtils.NextColumnEditableEntityId(eventIndex, nameof(BoidSpawnEventData.SpawnerEntityId), replayEventsData, ref e.SpawnerEntityId);
		EventTypeRendererUtils.NextColumnInputByteEnum(eventIndex, nameof(BoidSpawnEventData.BoidType), ref e.BoidType, EnumUtils.BoidTypes, _boidTypeNamesArray);
		EventTypeRendererUtils.NextColumnInputInt16Vec3(eventIndex, nameof(BoidSpawnEventData.Position), ref e.Position);
		EventTypeRendererUtils.NextColumnInputInt16Mat3x3(eventIndex, nameof(BoidSpawnEventData.Orientation), ref e.Orientation);
		EventTypeRendererUtils.NextColumnInputVector3(eventIndex, nameof(BoidSpawnEventData.Velocity), ref e.Velocity, "%.2f");
		EventTypeRendererUtils.NextColumnInputFloat(eventIndex, nameof(BoidSpawnEventData.Speed), ref e.Speed, "%.2f");
	}

	public static void RenderEdit(int eventIndex, BoidSpawnEventData e, ReplayEventsData replayEventsData)
	{
		const float leftColumnWidth = 120;
		const float rightColumnWidth = 160;
		const float tableWidth = leftColumnWidth + rightColumnWidth;

		if (ImGui.BeginChild(Inline.Span($"BoidSpawnEdit{eventIndex}"), default, ImGuiChildFlags.AutoResizeY))
		{
			if (ImGui.BeginTable("Left", 2, ImGuiTableFlags.None, new(tableWidth, 0)))
			{
				ImGui.TableSetupColumn("LeftText", ImGuiTableColumnFlags.WidthFixed, leftColumnWidth);
				ImGui.TableSetupColumn("LeftInput", ImGuiTableColumnFlags.None, rightColumnWidth);

				ImGui.TableNextRow();

				ImGui.TableNextColumn();
				ImGui.Text("Spawner Entity Id");
				ImGui.TableNextColumn();
				EventTypeRendererUtils.EditableEntityId(eventIndex, nameof(BoidSpawnEventData.SpawnerEntityId), replayEventsData, ref e.SpawnerEntityId);

				ImGui.TableNextColumn();
				ImGui.Text("Type");
				ImGui.TableNextColumn();
				EventTypeRendererUtils.InputByteEnum(eventIndex, nameof(BoidSpawnEventData.BoidType), ref e.BoidType, EnumUtils.BoidTypes, _boidTypeNamesArray);

				ImGui.TableNextColumn();
				ImGui.Text("Position");
				ImGui.TableNextColumn();
				EventTypeRendererUtils.InputInt16Vec3(eventIndex, nameof(BoidSpawnEventData.Position), ref e.Position);

				ImGui.EndTable();
			}

			ImGui.SameLine();

			if (ImGui.BeginTable("Right", 2, ImGuiTableFlags.None, new(tableWidth, 0)))
			{
				ImGui.TableSetupColumn("RightText", ImGuiTableColumnFlags.WidthFixed, leftColumnWidth);
				ImGui.TableSetupColumn("RightInput", ImGuiTableColumnFlags.None, rightColumnWidth);

				ImGui.TableNextRow();

				ImGui.TableNextColumn();
				ImGui.Text("Orientation");
				ImGui.TableNextColumn();
				EventTypeRendererUtils.InputInt16Mat3x3Square(eventIndex, nameof(BoidSpawnEventData.Orientation), ref e.Orientation);

				ImGui.TableNextColumn();
				ImGui.Text("Velocity");
				ImGui.TableNextColumn();
				EventTypeRendererUtils.InputVector3(eventIndex, nameof(BoidSpawnEventData.Velocity), ref e.Velocity, "%.2f");

				ImGui.TableNextColumn();
				ImGui.Text("Speed");
				ImGui.TableNextColumn();
				EventTypeRendererUtils.InputFloat(eventIndex, nameof(BoidSpawnEventData.Speed), ref e.Speed, "%.2f");

				ImGui.EndTable();
			}
		}

		ImGui.EndChild();
	}
}
