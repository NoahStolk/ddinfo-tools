using DevilDaggersInfo.App.ZeroAllocation;
using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using ImGuiNET;

namespace DevilDaggersInfo.App.Ui.ReplayEditor.Events.EventTypes;

public sealed class TransmuteEvents : IEventTypeRenderer<TransmuteEvent>
{
	public static void Render(IReadOnlyList<(int Index, TransmuteEvent Event)> events, IReadOnlyList<EntityType> entityTypes, IReadOnlyList<EventColumn> columns)
	{
		ImGui.TextColored(new(0.75f, 0, 0, 1), EventTypeRendererUtils.EventTypeNames[EventType.Transmute]);

		if (ImGui.BeginTable(EventTypeRendererUtils.EventTypeNames[EventType.Transmute], columns.Count, EventTypeRendererUtils.EventTableFlags))
		{
			EventTypeRendererUtils.SetupColumns(columns);

			for (int i = 0; i < events.Count; i++)
			{
				ImGui.TableNextRow();

				(int index, TransmuteEvent e) = events[i];
				EventTypeRendererUtils.NextColumnText(UnsafeSpan.Get(index));
				EventTypeRendererUtils.EntityColumn(entityTypes, e.EntityId);
				EventTypeRendererUtils.NextColumnText(UnsafeSpan.Get(e.A));
				EventTypeRendererUtils.NextColumnText(UnsafeSpan.Get(e.B));
				EventTypeRendererUtils.NextColumnText(UnsafeSpan.Get(e.C));
				EventTypeRendererUtils.NextColumnText(UnsafeSpan.Get(e.D));
			}

			ImGui.EndTable();
		}
	}
}
