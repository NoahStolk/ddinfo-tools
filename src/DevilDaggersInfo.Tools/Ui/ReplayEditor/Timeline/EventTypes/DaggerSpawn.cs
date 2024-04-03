using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Timeline.EventTypes;

public static class DaggerSpawn
{
	private static readonly string[] _daggerTypeNamesArray =
	[
		"Lvl1",
		"Lvl2",
		"Lvl3",
		"Lvl3 Homing",
		"Lvl4",
		"Lvl4 Homing",
		"Lvl4 Splash",
	];

	public static void RenderEdit(int uniqueId, DaggerSpawnEventData e)
	{
		const float leftColumnWidth = 120;
		const float rightColumnWidth = 160;
		const float tableWidth = leftColumnWidth + rightColumnWidth;

		if (ImGui.BeginChild(Inline.Span($"DaggerSpawnEdit{uniqueId}"), default, ImGuiChildFlags.AutoResizeY))
		{
			if (ImGui.BeginTable("Left", 2, ImGuiTableFlags.None, new(tableWidth, 0)))
			{
				ImGui.TableSetupColumn("LeftText", ImGuiTableColumnFlags.WidthFixed, leftColumnWidth);
				ImGui.TableSetupColumn("LeftInput", ImGuiTableColumnFlags.None, rightColumnWidth);

				ImGui.TableNextRow();

				ImGui.TableNextColumn();
				ImGui.Text("Type");
				ImGui.TableNextColumn();
				UtilsRendering.InputByteEnum(uniqueId, nameof(DaggerSpawnEventData.DaggerType), ref e.DaggerType, EnumUtils.DaggerTypes, _daggerTypeNamesArray);

				ImGui.TableNextColumn();
				ImGui.Text("?");
				ImGui.TableNextColumn();
				UtilsRendering.InputInt(uniqueId, nameof(DaggerSpawnEventData.A), ref e.A);

				ImGui.TableNextColumn();
				ImGui.Text("Shot/Rapid");
				ImGui.TableNextColumn();
				UtilsRendering.Checkbox(uniqueId, nameof(DaggerSpawnEventData.IsShot), ref e.IsShot, "Shot", "Rapid");

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
				UtilsRendering.InputInt16Vec3(uniqueId, nameof(DaggerSpawnEventData.Position), ref e.Position);

				ImGui.TableNextColumn();
				ImGui.Text("Orientation");
				ImGui.TableNextColumn();
				UtilsRendering.InputInt16Mat3x3Square(uniqueId, nameof(DaggerSpawnEventData.Orientation), ref e.Orientation);

				ImGui.EndTable();
			}
		}

		ImGui.EndChild();
	}
}
