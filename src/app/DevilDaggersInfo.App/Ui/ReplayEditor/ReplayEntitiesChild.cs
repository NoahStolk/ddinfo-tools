using DevilDaggersInfo.App.Utils;
using DevilDaggersInfo.App.ZeroAllocation;
using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Core.Replay.Extensions;
using ImGuiNET;

namespace DevilDaggersInfo.App.Ui.ReplayEditor;

public static class ReplayEntitiesChild
{
	public static void Render(ReplayEventsData eventsData)
	{
		if (ImGui.BeginTable("ReplayEventsTable", 4, ImGuiTableFlags.None))
		{
			ImGui.TableSetupColumn("Id", ImGuiTableColumnFlags.WidthFixed, 64);
			ImGui.TableSetupColumn("Type", ImGuiTableColumnFlags.None, 128);
			ImGui.TableHeadersRow();

			for (int i = 0; i < eventsData.EntityTypes.Count; i++)
			{
				ImGui.TableNextRow();

				EntityType entityType = eventsData.EntityTypes[i];

				ImGui.TableNextColumn();
				ImGui.Selectable(UnsafeSpan.Get(i), false, ImGuiSelectableFlags.SpanAllColumns);
				ImGui.TableNextColumn();
				ImGui.TextColored(((EntityType?)entityType).GetColor(), EnumUtils.EntityTypeNames[entityType]);
			}

			ImGui.EndTable();
		}
	}
}
