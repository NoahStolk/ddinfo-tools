using DevilDaggersInfo.App.ZeroAllocation;
using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Core.Wiki;
using ImGuiNET;

namespace DevilDaggersInfo.App.Ui.ReplayEditor.Events.EventTypes;

public sealed class ThornSpawnEvents : IEventTypeRenderer<ThornSpawnEvent>
{
	public static void Render(IReadOnlyList<(int Index, ThornSpawnEvent Event)> events, IReadOnlyList<EntityType> entityTypes, IReadOnlyList<EventColumn> columns)
	{
		ImGui.TextColored(EnemiesV3_2.Thorn.Color, EventTypeRendererUtils.EventTypeNames[EventType.ThornSpawn]);

		if (ImGui.BeginTable(EventTypeRendererUtils.EventTypeNames[EventType.ThornSpawn], columns.Count, EventTypeRendererUtils.EventTableFlags))
		{
			EventTypeRendererUtils.SetupColumns(columns);

			for (int i = 0; i < events.Count; i++)
			{
				ImGui.TableNextRow();

				(int index, ThornSpawnEvent e) = events[i];
				EventTypeRendererUtils.NextColumnText(UnsafeSpan.Get(index));
				EventTypeRendererUtils.EntityColumn(entityTypes, e.EntityId);
				EventTypeRendererUtils.NextColumnText(UnsafeSpan.Get(e.A));
				EventTypeRendererUtils.NextColumnText(UnsafeSpan.Get(e.Position, "0.00"));
				EventTypeRendererUtils.NextColumnText(UnsafeSpan.Get(e.RotationInRadians, "0.00"));
			}

			ImGui.EndTable();
		}
	}
}
