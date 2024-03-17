using DevilDaggersInfo.Core.Replay.Events.Data;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Timeline.EventTypes;

public static class ThornSpawn
{
	public static void RenderEdit(int uniqueId, ThornSpawnEventData e)
	{
		const float leftColumnWidth = 120;
		const float rightColumnWidth = 160;
		const float tableWidth = leftColumnWidth + rightColumnWidth;

		if (ImGui.BeginChild(Inline.Span($"ThornSpawnEdit{uniqueId}"), default, ImGuiChildFlags.AutoResizeY))
		{
			if (ImGui.BeginTable("Left", 2, ImGuiTableFlags.None, new(tableWidth, 0)))
			{
				ImGui.TableSetupColumn("LeftText", ImGuiTableColumnFlags.WidthFixed, leftColumnWidth);
				ImGui.TableSetupColumn("LeftInput", ImGuiTableColumnFlags.None, rightColumnWidth);

				ImGui.TableNextRow();

				ImGui.TableNextColumn();
				ImGui.Text("Position");
				ImGui.TableNextColumn();
				UtilsRendering.InputVector3(uniqueId, nameof(ThornSpawnEventData.Position), ref e.Position, "%.2f");

				ImGui.TableNextColumn();
				ImGui.Text("Rotation");
				ImGui.TableNextColumn();
				UtilsRendering.InputFloat(uniqueId, nameof(ThornSpawnEventData.RotationInRadians), ref e.RotationInRadians, "%.2f");

				ImGui.TableNextColumn();
				ImGui.Text("?");
				ImGui.TableNextColumn();
				UtilsRendering.InputInt(uniqueId, nameof(ThornSpawnEventData.A), ref e.A);

				ImGui.EndTable();
			}
		}

		ImGui.EndChild();
	}
}
