using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Tools.Extensions;
using Hexa.NET.ImGui;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Timeline.EventTypes;

internal static class DaggerSpawn
{
	public static void RenderEdit(int uniqueId, DaggerSpawnEventData e)
	{
		const float leftColumnWidth = 120;
		const float rightColumnWidth = 160;
		const float tableWidth = leftColumnWidth + rightColumnWidth;

		if (ImGui.BeginChild(Inline.Utf8($"DaggerSpawnEdit{uniqueId}"), default, ImGuiChildFlags.AutoResizeY))
		{
			if (ImGui.BeginTable("Left", 2, ImGuiTableFlags.None, new Vector2(tableWidth, 0)))
			{
				ImGui.TableSetupColumn("LeftText", ImGuiTableColumnFlags.WidthFixed, leftColumnWidth);
				ImGui.TableSetupColumn("LeftInput", ImGuiTableColumnFlags.None, rightColumnWidth);

				ImGui.TableNextRow();

				ImGui.TableNextColumn();
				ImGui.Text("Type");
				ImGui.TableNextColumn();
				UtilsRendering.InputByteEnum(uniqueId, "DaggerType"u8, ref e.DaggerType, DaggerTypeGen.Values, DaggerType.DaggerTypeNullTerminatedDisplayNames);

				ImGui.TableNextColumn();
				ImGui.Text("?");
				ImGui.TableNextColumn();
				UtilsRendering.InputInt(uniqueId, "A"u8, ref e.A);

				ImGui.TableNextColumn();
				ImGui.Text("Shot/Rapid");
				ImGui.TableNextColumn();
				UtilsRendering.Checkbox(uniqueId, "IsShot"u8, ref e.IsShot, "Shot"u8, "Rapid"u8);

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
				UtilsRendering.InputInt16Vec3(uniqueId, "Position"u8, ref e.Position);

				ImGui.TableNextColumn();
				ImGui.Text("Orientation");
				ImGui.TableNextColumn();
				UtilsRendering.InputInt16Mat3x3Square(uniqueId, "Orientation"u8, ref e.Orientation);

				ImGui.EndTable();
			}
		}

		ImGui.EndChild();
	}
}
