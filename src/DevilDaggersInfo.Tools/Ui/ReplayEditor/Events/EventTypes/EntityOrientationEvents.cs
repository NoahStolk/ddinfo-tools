using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class EntityOrientationEvents : IEventTypeRenderer<EntityOrientationEvent>
{
	public static void Render(IReadOnlyList<(int Index, EntityOrientationEvent Event)> events, IReadOnlyList<EntityType> entityTypes, IReadOnlyList<EventColumn> columns)
	{
		ImGui.TextColored(Color.Yellow, EventTypeRendererUtils.EventTypeNames[EventType.EntityOrientation]);

		if (ImGui.BeginTable(EventTypeRendererUtils.EventTypeNames[EventType.EntityOrientation], columns.Count, EventTypeRendererUtils.EventTableFlags))
		{
			EventTypeRendererUtils.SetupColumns(columns);

			for (int i = 0; i < events.Count; i++)
			{
				ImGui.TableNextRow();

				(int index, EntityOrientationEvent e) = events[i];
				EventTypeRendererUtils.NextColumnText(Inline.Span(index));
				EventTypeRendererUtils.EntityColumn(entityTypes, e.EntityId);
				EventTypeRendererUtils.NextColumnText(Inline.Span(e.Orientation));
			}

			ImGui.EndTable();
		}
	}
}