using DevilDaggersInfo.App.ZeroAllocation;
using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Core.Wiki;
using ImGuiNET;
using System.Diagnostics;

namespace DevilDaggersInfo.App.Ui.ReplayEditor.Events.EventTypes;

public sealed class SquidSpawnEvents : IEventTypeRenderer<SquidSpawnEvent>
{
	public static void Render(IReadOnlyList<(int Index, SquidSpawnEvent Event)> events, IReadOnlyList<EntityType> entityTypes, IReadOnlyList<EventColumn> columns)
	{
		ImGui.TextColored(EnemiesV3_2.Squid3.Color, EventTypeRendererUtils.EventTypeNames[EventType.SquidSpawn]);

		if (ImGui.BeginTable(EventTypeRendererUtils.EventTypeNames[EventType.SquidSpawn], columns.Count, EventTypeRendererUtils.EventTableFlags))
		{
			EventTypeRendererUtils.SetupColumns(columns);

			for (int i = 0; i < events.Count; i++)
			{
				ImGui.TableNextRow();

				(int index, SquidSpawnEvent e) = events[i];
				EventTypeRendererUtils.NextColumnText(UnsafeSpan.Get(index));
				EventTypeRendererUtils.EntityColumn(entityTypes, e.EntityId);
				EventTypeRendererUtils.NextColumnText(GetSquidTypeText(e.SquidType));
				EventTypeRendererUtils.EntityColumn(entityTypes, e.A);
				EventTypeRendererUtils.NextColumnText(UnsafeSpan.Get(e.Position, "0.00"));
				EventTypeRendererUtils.NextColumnText(UnsafeSpan.Get(e.Direction, "0.00"));
				EventTypeRendererUtils.NextColumnText(UnsafeSpan.Get(e.RotationInRadians, "0.00"));
			}

			ImGui.EndTable();
		}
	}

	private static ReadOnlySpan<char> GetSquidTypeText(SquidType squidType) => squidType switch
	{
		SquidType.Squid1 => "Squid1",
		SquidType.Squid2 => "Squid2",
		SquidType.Squid3 => "Squid3",
		_ => throw new UnreachableException(),
	};
}
