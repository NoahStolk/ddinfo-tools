using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Core.Wiki;
using ImGuiNET;

namespace DevilDaggersInfo.App.Ui.ReplayEditor.Events.EventTypes;

public sealed class LeviathanSpawnEvents : IEventTypeRenderer<LeviathanSpawnEvent>
{
	public static void Render(IReadOnlyList<(int Index, LeviathanSpawnEvent Event)> events, IReadOnlyList<EntityType> entityTypes, IReadOnlyList<EventColumn> columns)
	{
		ImGui.TextColored(EnemiesV3_2.Leviathan.Color, EventTypeRendererUtils.EventTypeNames[EventType.LeviathanSpawn]);

		if (ImGui.BeginTable(EventTypeRendererUtils.EventTypeNames[EventType.LeviathanSpawn], columns.Count, EventTypeRendererUtils.EventTableFlags))
		{
			EventTypeRendererUtils.SetupColumns(columns);

			for (int i = 0; i < events.Count; i++)
			{
				ImGui.TableNextRow();

				(int index, LeviathanSpawnEvent e) = events[i];
				EventTypeRendererUtils.NextColumnText(Inline.Span(index));
				EventTypeRendererUtils.EntityColumn(entityTypes, e.EntityId);
				EventTypeRendererUtils.NextColumnText(Inline.Span(e.A));
			}

			ImGui.EndTable();
		}
	}
}
