using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.ZeroAllocation;
using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using ImGuiNET;

namespace DevilDaggersInfo.App.Ui.ReplayEditor.Events.EventTypes;

public sealed class EntityPositionEvents : IEventTypeRenderer<EntityPositionEvent>
{
	public static void Render(IReadOnlyList<(int Index, EntityPositionEvent Event)> events, IReadOnlyList<EntityType> entityTypes, IReadOnlyList<EventColumn> columns)
	{
		ImGui.TextColored(Color.Yellow, EventTypeRendererUtils.EventTypeNames[EventType.EntityPosition]);

		if (ImGui.BeginTable(EventTypeRendererUtils.EventTypeNames[EventType.EntityPosition], columns.Count, EventTypeRendererUtils.EventTableFlags))
		{
			EventTypeRendererUtils.SetupColumns(columns);

			for (int i = 0; i < events.Count; i++)
			{
				ImGui.TableNextRow();

				(int index, EntityPositionEvent e) = events[i];
				EventTypeRendererUtils.NextColumnText(UnsafeSpan.Get(index));
				EventTypeRendererUtils.EntityColumn(entityTypes, e.EntityId);
				EventTypeRendererUtils.NextColumnText(UnsafeSpan.Get(e.Position));
			}

			ImGui.EndTable();
		}
	}
}
