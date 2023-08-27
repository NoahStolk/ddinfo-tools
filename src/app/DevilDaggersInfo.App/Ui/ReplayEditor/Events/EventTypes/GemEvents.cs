using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.ZeroAllocation;
using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using ImGuiNET;

namespace DevilDaggersInfo.App.Ui.ReplayEditor.Events.EventTypes;

public sealed class GemEvents : IEventTypeRenderer<GemEvent>
{
	public static void Render(IReadOnlyList<(int Index, GemEvent Event)> events, IReadOnlyList<EntityType> entityTypes, IReadOnlyList<EventColumn> columns)
	{
		ImGui.TextColored(Color.Red, EventTypeRendererUtils.EventTypeNames[EventType.Gem]);

		if (ImGui.BeginTable(EventTypeRendererUtils.EventTypeNames[EventType.Gem], columns.Count, EventTypeRendererUtils.EventTableFlags))
		{
			EventTypeRendererUtils.SetupColumns(columns);

			for (int i = 0; i < events.Count; i++)
			{
				ImGui.TableNextRow();

				(int index, _) = events[i];
				EventTypeRendererUtils.NextColumnText(UnsafeSpan.Get(index));
			}

			ImGui.EndTable();
		}
	}
}
