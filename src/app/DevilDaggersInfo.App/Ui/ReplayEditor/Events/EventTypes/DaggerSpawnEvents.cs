using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Utils;
using DevilDaggersInfo.App.ZeroAllocation;
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
				EventTypeRendererUtils.NextColumnText(UnsafeSpan.Get(index));
				EventTypeRendererUtils.EntityColumn(entityTypes, e.EntityId);
				EventTypeRendererUtils.NextColumnText(EnumUtils.DaggerTypeNames[e.DaggerType]);
				EventTypeRendererUtils.NextColumnText(UnsafeSpan.Get(e.A));
				EventTypeRendererUtils.NextColumnText(UnsafeSpan.Get(e.Position));
				EventTypeRendererUtils.NextColumnText(UnsafeSpan.Get(e.Orientation));
				EventTypeRendererUtils.NextColumnText(e.IsShot ? "Shot" : "Rapid");
			}

			ImGui.EndTable();
		}
	}
}
