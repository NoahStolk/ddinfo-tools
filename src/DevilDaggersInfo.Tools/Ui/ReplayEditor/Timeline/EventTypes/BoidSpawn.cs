using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Timeline.EventTypes;

public static class BoidSpawn
{
	private static readonly string[] _boidTypeNamesArray = EnumUtils.BoidTypeNames.Values.ToArray();

	public static void RenderEdit(int uniqueId, BoidSpawnEventData e, EditorReplayModel replay)
	{
		const float leftColumnWidth = 120;
		const float rightColumnWidth = 160;
		const float tableWidth = leftColumnWidth + rightColumnWidth;

		if (ImGui.BeginChild(Inline.Span($"BoidSpawnEdit{uniqueId}"), default, ImGuiChildFlags.AutoResizeY))
		{
			if (ImGui.BeginTable("Left", 2, ImGuiTableFlags.None, new(tableWidth, 0)))
			{
				ImGui.TableSetupColumn("LeftText", ImGuiTableColumnFlags.WidthFixed, leftColumnWidth);
				ImGui.TableSetupColumn("LeftInput", ImGuiTableColumnFlags.None, rightColumnWidth);

				ImGui.TableNextRow();

				ImGui.TableNextColumn();
				ImGui.Text("Spawner Entity Id");
				ImGui.TableNextColumn();
				UtilsRendering.EditableEntityId(uniqueId, nameof(BoidSpawnEventData.SpawnerEntityId), replay, ref e.SpawnerEntityId);

				ImGui.TableNextColumn();
				ImGui.Text("Type");
				ImGui.TableNextColumn();
				UtilsRendering.InputByteEnum(uniqueId, nameof(BoidSpawnEventData.BoidType), ref e.BoidType, EnumUtils.BoidTypes, _boidTypeNamesArray);

				ImGui.TableNextColumn();
				ImGui.Text("Position");
				ImGui.TableNextColumn();
				UtilsRendering.InputInt16Vec3(uniqueId, nameof(BoidSpawnEventData.Position), ref e.Position);

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
				UtilsRendering.InputInt16Mat3x3Square(uniqueId, nameof(BoidSpawnEventData.Orientation), ref e.Orientation);

				ImGui.TableNextColumn();
				ImGui.Text("Velocity");
				ImGui.TableNextColumn();
				UtilsRendering.InputVector3(uniqueId, nameof(BoidSpawnEventData.Velocity), ref e.Velocity, "%.2f");

				ImGui.TableNextColumn();
				ImGui.Text("Speed");
				ImGui.TableNextColumn();
				UtilsRendering.InputFloat(uniqueId, nameof(BoidSpawnEventData.Speed), ref e.Speed, "%.2f");

				ImGui.EndTable();
			}
		}

		ImGui.EndChild();
	}
}
