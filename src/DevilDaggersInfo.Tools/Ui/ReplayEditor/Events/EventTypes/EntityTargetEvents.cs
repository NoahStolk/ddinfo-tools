using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class EntityTargetEvents : IEventTypeRenderer<EntityTargetEvent>
{
	public static void Render(IReadOnlyList<(int Index, EntityTargetEvent Event)> events, IReadOnlyList<EntityType> entityTypes, IReadOnlyList<EventColumn> columns)
	{
		ImGui.TextColored(Color.Yellow, EventTypeRendererUtils.EventTypeNames[EventType.EntityTarget]);

		if (ImGui.BeginTable(EventTypeRendererUtils.EventTypeNames[EventType.EntityTarget], columns.Count, EventTypeRendererUtils.EventTableFlags))
		{
			EventTypeRendererUtils.SetupColumns(columns);

			for (int i = 0; i < events.Count; i++)
			{
				ImGui.TableNextRow();

				(int index, EntityTargetEvent e) = events[i];
				EventTypeRendererUtils.NextColumnText(Inline.Span(index));
				EventTypeRendererUtils.EntityColumn(entityTypes, e.EntityId);
				EventTypeRendererUtils.NextColumnInputInt16Vec3(index, nameof(EntityTargetEvent.TargetPosition), ref e.TargetPosition);
			}

			ImGui.EndTable();
		}
	}
}
