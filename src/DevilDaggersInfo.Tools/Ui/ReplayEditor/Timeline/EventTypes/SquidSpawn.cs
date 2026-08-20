using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using Hexa.NET.ImGui;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Timeline.EventTypes;

internal static class SquidSpawn
{
	public static void RenderEdit(int uniqueId, SquidSpawnEventData e)
	{
		const float leftColumnWidth = 120;
		const float rightColumnWidth = 160;
		const float tableWidth = leftColumnWidth + rightColumnWidth;

		if (ImGui.BeginChild(Inline.Utf8($"SquidSpawnEdit{uniqueId}"), default, ImGuiChildFlags.AutoResizeY))
		{
			if (ImGui.BeginTable("Left", 2, ImGuiTableFlags.None, new Vector2(tableWidth, 0)))
			{
				ImGui.TableSetupColumn("LeftText", ImGuiTableColumnFlags.WidthFixed, leftColumnWidth);
				ImGui.TableSetupColumn("LeftInput", ImGuiTableColumnFlags.None, rightColumnWidth);

				ImGui.TableNextRow();

				ImGui.TableNextColumn();
				ImGui.Text("Type");
				ImGui.TableNextColumn();
				UtilsRendering.InputByteEnum(uniqueId, "SquidType"u8, ref e.SquidType, SquidTypeGen.Values, SquidTypeGen.NullTerminatedMemberNames);

				ImGui.TableNextColumn();
				ImGui.Text("?");
				ImGui.TableNextColumn();
				UtilsRendering.InputInt(uniqueId, "A"u8, ref e.A);

				ImGui.EndTable();
			}

			ImGui.SameLine();

			if (ImGui.BeginTable("Right", 2, ImGuiTableFlags.None, new Vector2(tableWidth, 0)))
			{
				ImGui.TableSetupColumn("RightText", ImGuiTableColumnFlags.WidthFixed, leftColumnWidth);
				ImGui.TableSetupColumn("RightInput", ImGuiTableColumnFlags.None, rightColumnWidth);

				ImGui.TableNextRow();

				ImGui.TableNextColumn();
				ImGui.Text("Position");
				ImGui.TableNextColumn();
				UtilsRendering.InputVector3(uniqueId, "Position"u8, ref e.Position, "%.2f"u8);

				ImGui.TableNextColumn();
				ImGui.Text("Direction");
				ImGui.TableNextColumn();
				UtilsRendering.InputVector3(uniqueId, "Direction"u8, ref e.Direction, "%.2f"u8);

				ImGui.TableNextColumn();
				ImGui.Text("Rotation");
				ImGui.TableNextColumn();
				UtilsRendering.InputFloat(uniqueId, "RotationInRadians"u8, ref e.RotationInRadians, "%.2f"u8);

				ImGui.EndTable();
			}
		}

		ImGui.EndChild();
	}
}
