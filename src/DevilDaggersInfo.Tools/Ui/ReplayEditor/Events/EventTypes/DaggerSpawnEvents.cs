using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

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
				EventTypeRendererUtils.NextColumnText(EnumUtils.DaggerTypeNames[e.DaggerType]); // TODO: Make this a dropdown.
				EventTypeRendererUtils.NextColumnInputInt(index, nameof(DaggerSpawnEvent.A), ref e.A);
				EventTypeRendererUtils.NextColumnInputInt16Vec3(index, nameof(DaggerSpawnEvent.Position), ref e.Position);
				EventTypeRendererUtils.NextColumnInputInt16Mat3x3(index, nameof(DaggerSpawnEvent.Orientation), ref e.Orientation);
				EventTypeRendererUtils.NextColumnCheckbox(index, nameof(DaggerSpawnEvent.IsShot), ref e.IsShot);
			}

			ImGui.EndTable();
		}
	}
}
