using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Utils;
using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using ImGuiNET;

namespace DevilDaggersInfo.App.Ui.ReplayEditor.Events.EventTypes;

public sealed class DaggerSpawnEvents : IEventTypeRenderer<DaggerSpawnEvent>
{
	public static void Render(IReadOnlyList<(int Index, DaggerSpawnEvent Event)> events, IReadOnlyList<EntityType> entityTypes, IReadOnlyList<EventColumn> columns)
	{
		ImGui.TextColored(Color.Purple, EventTypeRendererUtils.EventTypeNames[EventType.DaggerSpawn]);

		if (ImGui.BeginTable(EventTypeRendererUtils.EventTypeNames[EventType.DaggerSpawn], columns.Count, EventTypeRendererUtils.EventTableFlags))
		{
			EventTypeRendererUtils.SetupColumns(columns);

			for (int i = 0; i < events.Count; i++)
			{
				ImGui.TableNextRow();

				(int index, DaggerSpawnEvent e) = events[i];
				EventTypeRendererUtils.NextColumnText(Inline.Span(index));
				EventTypeRendererUtils.EntityColumn(entityTypes, e.EntityId);
				EventTypeRendererUtils.NextColumnText(EnumUtils.DaggerTypeNames[e.DaggerType]);
				EventTypeRendererUtils.NextColumnText(Inline.Span(e.A));
				EventTypeRendererUtils.NextColumnText(Inline.Span(e.Position));
				EventTypeRendererUtils.NextColumnText(Inline.Span(e.Orientation));
				EventTypeRendererUtils.NextColumnText(e.IsShot ? "Shot" : "Rapid");
			}

			ImGui.EndTable();
		}
	}
}
