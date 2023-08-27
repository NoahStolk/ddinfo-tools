using DevilDaggersInfo.App.ZeroAllocation;
using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Core.Wiki;
using ImGuiNET;

namespace DevilDaggersInfo.App.Ui.ReplayEditor.Events.EventTypes;

public sealed class SpiderEggSpawnEvents : IEventTypeRenderer<SpiderEggSpawnEvent>
{
	public static void Render(IReadOnlyList<(int Index, SpiderEggSpawnEvent Event)> events, IReadOnlyList<EntityType> entityTypes, IReadOnlyList<EventColumn> columns)
	{
		ImGui.TextColored(EnemiesV3_2.SpiderEgg1.Color, EventTypeRendererUtils.EventTypeNames[EventType.SpiderEggSpawn]);

		if (ImGui.BeginTable(EventTypeRendererUtils.EventTypeNames[EventType.SpiderEggSpawn], columns.Count, EventTypeRendererUtils.EventTableFlags))
		{
			EventTypeRendererUtils.SetupColumns(columns);

			for (int i = 0; i < events.Count; i++)
			{
				ImGui.TableNextRow();

				(int index, SpiderEggSpawnEvent e) = events[i];
				EventTypeRendererUtils.NextColumnText(UnsafeSpan.Get(index));
				EventTypeRendererUtils.EntityColumn(entityTypes, e.EntityId);
				EventTypeRendererUtils.EntityColumn(entityTypes, e.SpawnerEntityId);
				EventTypeRendererUtils.NextColumnText(UnsafeSpan.Get(e.Position, "0.00"));
				EventTypeRendererUtils.NextColumnText(UnsafeSpan.Get(e.TargetPosition, "0.00"));
			}

			ImGui.EndTable();
		}
	}
}
