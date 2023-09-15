using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using ImGuiNET;

namespace DevilDaggersInfo.App.Ui.ReplayEditor.Events.EventTypes;

public sealed class HitEvents : IEventTypeRenderer<HitEvent>
{
	public static void Render(IReadOnlyList<(int Index, HitEvent Event)> events, IReadOnlyList<EntityType> entityTypes, IReadOnlyList<EventColumn> columns)
	{
		ImGui.TextColored(Color.Orange, EventTypeRendererUtils.EventTypeNames[EventType.Hit]);

		if (ImGui.BeginTable(EventTypeRendererUtils.EventTypeNames[EventType.Hit], columns.Count, EventTypeRendererUtils.EventTableFlags))
		{
			EventTypeRendererUtils.SetupColumns(columns);

			for (int i = 0; i < events.Count; i++)
			{
				ImGui.TableNextRow();

				(int index, HitEvent e) = events[i];
				EventTypeRendererUtils.NextColumnText(Inline.Span(index));
				EventTypeRendererUtils.EntityColumn(entityTypes, e.EntityIdA);
				EventTypeRendererUtils.EntityColumn(entityTypes, e.EntityIdB);
				EventTypeRendererUtils.NextColumnText(Inline.Span(e.UserData));
			}

			ImGui.EndTable();
		}
	}
}
