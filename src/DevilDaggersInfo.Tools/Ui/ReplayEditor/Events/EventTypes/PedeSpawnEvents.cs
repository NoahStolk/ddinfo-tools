using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class PedeSpawnEvents : IEventTypeRenderer<PedeSpawnEventData>
{
	private static readonly string[] _pedeTypeNamesArray = EnumUtils.PedeTypeNames.Values.ToArray();

	public static int ColumnCount => 7;
	public static int ColumnCountData => 5;

	public static void SetupColumns()
	{
		EventTypeRendererUtils.SetupColumnIndex();
		EventTypeRendererUtils.SetupColumnEntityId();
		SetupColumnsData();
	}

	public static void SetupColumnsData()
	{
		ImGui.TableSetupColumn("Type", ImGuiTableColumnFlags.WidthFixed, 96);
		ImGui.TableSetupColumn("?", ImGuiTableColumnFlags.WidthFixed, 32);
		ImGui.TableSetupColumn("Position", ImGuiTableColumnFlags.WidthFixed, 128);
		ImGui.TableSetupColumn("?", ImGuiTableColumnFlags.WidthFixed, 80);
		ImGui.TableSetupColumn("Orientation", ImGuiTableColumnFlags.None, 128);
	}

	public static void Render(int eventIndex, int entityId, PedeSpawnEventData e, EditorReplayModel replay)
	{
		EventTypeRendererUtils.NextColumnEventIndex(eventIndex);
		EventTypeRendererUtils.NextColumnEntityId(replay, entityId);
		RenderData(eventIndex, e, replay);
	}

	public static void RenderData(int eventIndex, PedeSpawnEventData e, EditorReplayModel replay)
	{
		EventTypeRendererUtils.NextColumnInputByteEnum(eventIndex, nameof(PedeSpawnEventData.PedeType), ref e.PedeType, EnumUtils.PedeTypes, _pedeTypeNamesArray);
		EventTypeRendererUtils.NextColumnInputInt(eventIndex, nameof(PedeSpawnEventData.A), ref e.A);
		EventTypeRendererUtils.NextColumnInputVector3(eventIndex, nameof(PedeSpawnEventData.Position), ref e.Position, "%.2f");
		EventTypeRendererUtils.NextColumnInputVector3(eventIndex, nameof(PedeSpawnEventData.B), ref e.B, "%.0f");
		EventTypeRendererUtils.NextColumnInputMatrix3x3(eventIndex, nameof(PedeSpawnEventData.Orientation), ref e.Orientation, "%.2f");
	}

	public static void RenderEdit(int uniqueId, PedeSpawnEventData e, EditorReplayModel replay)
	{
		const float leftColumnWidth = 120;
		const float rightColumnWidth = 160;
		const float tableWidth = leftColumnWidth + rightColumnWidth;

		if (ImGui.BeginChild(Inline.Span($"PedeSpawnEdit{uniqueId}"), default, ImGuiChildFlags.AutoResizeY))
		{
			if (ImGui.BeginTable("Left", 2, ImGuiTableFlags.None, new(tableWidth, 0)))
			{
				ImGui.TableSetupColumn("LeftText", ImGuiTableColumnFlags.WidthFixed, leftColumnWidth);
				ImGui.TableSetupColumn("LeftInput", ImGuiTableColumnFlags.None, rightColumnWidth);

				ImGui.TableNextRow();

				ImGui.TableNextColumn();
				ImGui.Text("Type");
				ImGui.TableNextColumn();
				EventTypeRendererUtils.InputByteEnum(uniqueId, nameof(PedeSpawnEventData.PedeType), ref e.PedeType, EnumUtils.PedeTypes, _pedeTypeNamesArray);

				ImGui.TableNextColumn();
				ImGui.Text("?");
				ImGui.TableNextColumn();
				EventTypeRendererUtils.InputInt(uniqueId, nameof(PedeSpawnEventData.A), ref e.A);

				ImGui.TableNextColumn();
				ImGui.Text("?");
				ImGui.TableNextColumn();
				EventTypeRendererUtils.InputVector3(uniqueId, nameof(PedeSpawnEventData.B), ref e.B, "%.0f");

				ImGui.EndTable();
			}

			ImGui.SameLine();

			if (ImGui.BeginTable("Right", 2, ImGuiTableFlags.None, new(tableWidth, 0)))
			{
				ImGui.TableSetupColumn("RightText", ImGuiTableColumnFlags.WidthFixed, leftColumnWidth);
				ImGui.TableSetupColumn("RightInput", ImGuiTableColumnFlags.None, rightColumnWidth);

				ImGui.TableNextRow();

				ImGui.TableNextColumn();
				ImGui.Text("Position");
				ImGui.TableNextColumn();
				EventTypeRendererUtils.InputVector3(uniqueId, nameof(PedeSpawnEventData.Position), ref e.Position, "%.2f");

				ImGui.TableNextColumn();
				ImGui.Text("Orientation");
				ImGui.TableNextColumn();
				EventTypeRendererUtils.InputMatrix3x3Square(uniqueId, nameof(PedeSpawnEventData.Orientation), ref e.Orientation, "%.2f");

				ImGui.EndTable();
			}
		}

		ImGui.EndChild();
	}
}
