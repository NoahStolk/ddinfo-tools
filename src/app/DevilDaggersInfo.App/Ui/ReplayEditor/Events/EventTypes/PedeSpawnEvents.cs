using DevilDaggersInfo.App.ZeroAllocation;
using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Core.Wiki;
using ImGuiNET;
using System.Diagnostics;

namespace DevilDaggersInfo.App.Ui.ReplayEditor.Events.EventTypes;

public sealed class PedeSpawnEvents : IEventTypeRenderer<PedeSpawnEvent>
{
	public static void Render(IReadOnlyList<(int Index, PedeSpawnEvent Event)> events, IReadOnlyList<EntityType> entityTypes, IReadOnlyList<EventColumn> columns)
	{
		ImGui.TextColored(EnemiesV3_2.Gigapede.Color, EventTypeRendererUtils.EventTypeNames[EventType.PedeSpawn]);

		if (ImGui.BeginTable(EventTypeRendererUtils.EventTypeNames[EventType.PedeSpawn], columns.Count, EventTypeRendererUtils.EventTableFlags))
		{
			EventTypeRendererUtils.SetupColumns(columns);

			for (int i = 0; i < events.Count; i++)
			{
				ImGui.TableNextRow();

				(int index, PedeSpawnEvent e) = events[i];
				EventTypeRendererUtils.NextColumnText(UnsafeSpan.Get(index));
				EventTypeRendererUtils.EntityColumn(entityTypes, e.EntityId);
				EventTypeRendererUtils.NextColumnText(GetPedeTypeText(e.PedeType));
				EventTypeRendererUtils.NextColumnText(UnsafeSpan.Get(e.A));
				EventTypeRendererUtils.NextColumnText(UnsafeSpan.Get(e.Position, "0.00"));
				EventTypeRendererUtils.NextColumnText(UnsafeSpan.Get(e.B));
				EventTypeRendererUtils.NextColumnText(UnsafeSpan.Get(e.Orientation));
			}

			ImGui.EndTable();
		}
	}

	private static ReadOnlySpan<char> GetPedeTypeText(PedeType pedeType) => pedeType switch
	{
		PedeType.Centipede => "Centipede",
		PedeType.Gigapede => "Gigapede",
		PedeType.Ghostpede => "Ghostpede",
		_ => throw new UnreachableException(),
	};
}
