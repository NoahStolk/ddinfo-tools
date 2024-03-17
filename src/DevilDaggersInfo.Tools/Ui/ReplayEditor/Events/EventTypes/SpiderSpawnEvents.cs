using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class SpiderSpawnEvents : IEventTypeRenderer<SpiderSpawnEventData>
{
	private static readonly string[] _spiderTypeNamesArray = EnumUtils.SpiderTypeNames.Values.ToArray();

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
		ImGui.TableSetupColumn("Type", ImGuiTableColumnFlags.WidthFixed, 80);
		ImGui.TableSetupColumn("?", ImGuiTableColumnFlags.WidthFixed, 32);
		ImGui.TableSetupColumn("Position", ImGuiTableColumnFlags.WidthFixed, 192);
	}

	public static void Render(int eventIndex, int entityId, SpiderSpawnEventData e, EditorReplayModel replay)
	{
		EventTypeRendererUtils.NextColumnEventIndex(eventIndex);
		EventTypeRendererUtils.NextColumnEntityId(replay, entityId);
		RenderData(eventIndex, e, replay);
	}

	public static void RenderData(int eventIndex, SpiderSpawnEventData e, EditorReplayModel replay)
	{
		EventTypeRendererUtils.NextColumnInputByteEnum(eventIndex, nameof(SpiderSpawnEventData.SpiderType), ref e.SpiderType, EnumUtils.SpiderTypes, _spiderTypeNamesArray);
		EventTypeRendererUtils.NextColumnInputInt(eventIndex, nameof(SpiderSpawnEventData.A), ref e.A);
		EventTypeRendererUtils.NextColumnInputVector3(eventIndex, nameof(SpiderSpawnEventData.Position), ref e.Position, "%.2f");
	}

	public static void RenderEdit(int uniqueId, SpiderSpawnEventData e, EditorReplayModel replay)
	{
		const float leftColumnWidth = 120;
		const float rightColumnWidth = 160;
		const float tableWidth = leftColumnWidth + rightColumnWidth;

		if (ImGui.BeginChild(Inline.Span($"SpiderSpawnEdit{uniqueId}"), default, ImGuiChildFlags.AutoResizeY))
		{
			if (ImGui.BeginTable("Left", 2, ImGuiTableFlags.None, new(tableWidth, 0)))
			{
				ImGui.TableSetupColumn("LeftText", ImGuiTableColumnFlags.WidthFixed, leftColumnWidth);
				ImGui.TableSetupColumn("LeftInput", ImGuiTableColumnFlags.None, rightColumnWidth);

				ImGui.TableNextRow();

				ImGui.TableNextColumn();
				ImGui.Text("Type");
				ImGui.TableNextColumn();
				EventTypeRendererUtils.InputByteEnum(uniqueId, nameof(SpiderSpawnEventData.SpiderType), ref e.SpiderType, EnumUtils.SpiderTypes, _spiderTypeNamesArray);

				ImGui.TableNextColumn();
				ImGui.Text("Position");
				ImGui.TableNextColumn();
				EventTypeRendererUtils.InputVector3(uniqueId, nameof(SpiderSpawnEventData.Position), ref e.Position, "%.2f");

				ImGui.TableNextColumn();
				ImGui.Text("?");
				ImGui.TableNextColumn();
				EventTypeRendererUtils.InputInt(uniqueId, nameof(SpiderSpawnEventData.A), ref e.A);

				ImGui.EndTable();
			}
		}

		ImGui.EndChild();
	}
}
