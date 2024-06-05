using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Timeline.EventTypes;

public static class Hit
{
	public static void RenderEdit(int uniqueId, HitEventData e, EditorReplayModel replay)
	{
		const float leftColumnWidth = 120;
		const float rightColumnWidth = 160;
		const float tableWidth = leftColumnWidth + rightColumnWidth;

		if (ImGui.BeginChild(Inline.Span($"HitEdit{uniqueId}"), default, ImGuiChildFlags.AutoResizeY))
		{
			if (ImGui.BeginTable("Left", 2, ImGuiTableFlags.None, new Vector2(tableWidth, 0)))
			{
				ImGui.TableSetupColumn("LeftText", ImGuiTableColumnFlags.WidthFixed, leftColumnWidth);
				ImGui.TableSetupColumn("LeftInput", ImGuiTableColumnFlags.None, rightColumnWidth);

				ImGui.TableNextRow();

				ImGui.TableNextColumn();
				ImGui.Text("Entity Id A");
				ImGui.TableNextColumn();
				UtilsRendering.EditableEntityId(uniqueId, nameof(HitEventData.EntityIdA), replay, ref e.EntityIdA);

				ImGui.TableNextColumn();
				ImGui.Text("Entity Id B");
				ImGui.TableNextColumn();
				UtilsRendering.EditableEntityId(uniqueId, nameof(HitEventData.EntityIdB), replay, ref e.EntityIdB);

				ImGui.TableNextColumn();
				ImGui.Text("User Data");
				ImGui.TableNextColumn();
				UtilsRendering.InputInt(uniqueId, nameof(HitEventData.UserData), ref e.UserData);

				ImGui.EndTable();
			}

			ImGui.SameLine();

			ImGui.Text("Explanation");
			HitEventExplanation.Render(e, replay);
		}

		ImGui.EndChild();
	}

}
