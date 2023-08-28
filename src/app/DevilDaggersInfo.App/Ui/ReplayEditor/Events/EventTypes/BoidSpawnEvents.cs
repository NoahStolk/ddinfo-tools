using DevilDaggersInfo.App.ZeroAllocation;
using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Core.Wiki;
using ImGuiNET;
using System.Diagnostics;

namespace DevilDaggersInfo.App.Ui.ReplayEditor.Events.EventTypes;

public sealed class BoidSpawnEvents : IEventTypeRenderer<BoidSpawnEvent>
{
	public static void Render(IReadOnlyList<(int Index, BoidSpawnEvent Event)> events, IReadOnlyList<EntityType> entityTypes, IReadOnlyList<EventColumn> columns)
	{
		ImGui.TextColored(EnemiesV3_2.Skull4.Color, EventTypeRendererUtils.EventTypeNames[EventType.BoidSpawn]);

		if (ImGui.BeginTable(EventTypeRendererUtils.EventTypeNames[EventType.BoidSpawn], columns.Count, EventTypeRendererUtils.EventTableFlags))
		{
			EventTypeRendererUtils.SetupColumns(columns);

			for (int i = 0; i < events.Count; i++)
			{
				ImGui.TableNextRow();

				(int index, BoidSpawnEvent e) = events[i];
				EventTypeRendererUtils.NextColumnText(UnsafeSpan.Get(index));
				EventTypeRendererUtils.EntityColumn(entityTypes, e.EntityId);
				EventTypeRendererUtils.EntityColumn(entityTypes, e.SpawnerEntityId);
				EventTypeRendererUtils.NextColumnText(GetBoidTypeText(e.BoidType));
				EventTypeRendererUtils.NextColumnText(UnsafeSpan.Get(e.Position));
				EventTypeRendererUtils.NextColumnText(UnsafeSpan.Get(e.Orientation));
				EventTypeRendererUtils.NextColumnText(UnsafeSpan.Get(e.Velocity));
				EventTypeRendererUtils.NextColumnText(UnsafeSpan.Get(e.Speed, "0.00"));
			}

			ImGui.EndTable();
		}
	}

	private static ReadOnlySpan<char> GetBoidTypeText(BoidType boidType) => boidType switch
	{
		BoidType.Skull1 => "Skull1",
		BoidType.Skull2 => "Skull2",
		BoidType.Skull3 => "Skull3",
		BoidType.Skull4 => "Skull4",
		BoidType.Spiderling => "Spiderling",
		_ => throw new UnreachableException(),
	};
}
