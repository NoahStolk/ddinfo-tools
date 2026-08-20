using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using Hexa.NET.ImGui;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Timeline.EventTypes;

internal static class SpiderSpawn
{
	public static void RenderEdit(int uniqueId, SpiderSpawnEventData e)
	{
		const float leftColumnWidth = 120;
		const float rightColumnWidth = 160;
		const float tableWidth = leftColumnWidth + rightColumnWidth;

		if (ImGui.BeginChild(Inline.Utf8($"SpiderSpawnEdit{uniqueId}"), default, ImGuiChildFlags.AutoResizeY))
		{
			if (ImGui.BeginTable("Left", 2, ImGuiTableFlags.None, new Vector2(tableWidth, 0)))
			{
				ImGui.TableSetupColumn("LeftText", ImGuiTableColumnFlags.WidthFixed, leftColumnWidth);
				ImGui.TableSetupColumn("LeftInput", ImGuiTableColumnFlags.None, rightColumnWidth);

				ImGui.TableNextRow();

				ImGui.TableNextColumn();
				ImGui.Text("Type");
				ImGui.TableNextColumn();
				UtilsRendering.InputByteEnum(uniqueId, "SpiderType"u8, ref e.SpiderType, SpiderTypeGen.Values, SpiderTypeGen.NullTerminatedMemberNames);

				ImGui.TableNextColumn();
				ImGui.Text("Position");
				ImGui.TableNextColumn();
				UtilsRendering.InputVector3(uniqueId, "Position"u8, ref e.Position, "%.2f"u8);

				ImGui.TableNextColumn();
				ImGui.Text("?");
				ImGui.TableNextColumn();
				UtilsRendering.InputInt(uniqueId, "A"u8, ref e.A);

				ImGui.EndTable();
			}
		}

		ImGui.EndChild();
	}
}
