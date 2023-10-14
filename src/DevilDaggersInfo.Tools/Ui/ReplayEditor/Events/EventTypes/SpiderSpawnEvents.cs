using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Core.Wiki;
using ImGuiNET;
using System.Diagnostics;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class SpiderSpawnEvents : IEventTypeRenderer<SpiderSpawnEvent>
{
	public static void Render(IReadOnlyList<(int Index, SpiderSpawnEvent Event)> events, IReadOnlyList<EntityType> entityTypes, IReadOnlyList<EventColumn> columns)
	{
		ImGui.TextColored(EnemiesV3_2.Spider2.Color, EventTypeRendererUtils.EventTypeNames[EventType.SpiderSpawn]);

		if (ImGui.BeginTable(EventTypeRendererUtils.EventTypeNames[EventType.SpiderSpawn], columns.Count, EventTypeRendererUtils.EventTableFlags))
		{
			EventTypeRendererUtils.SetupColumns(columns);

			for (int i = 0; i < events.Count; i++)
			{
				ImGui.TableNextRow();

				(int index, SpiderSpawnEvent e) = events[i];
				EventTypeRendererUtils.NextColumnText(Inline.Span(index));
				EventTypeRendererUtils.EntityColumn(entityTypes, e.EntityId);
				EventTypeRendererUtils.NextColumnText(GetSpiderTypeText(e.SpiderType)); // TODO: Make this a dropdown.
				EventTypeRendererUtils.NextColumnInputInt(index, nameof(SpiderSpawnEvent.A), ref e.A);
				EventTypeRendererUtils.NextColumnInputVector3(index, nameof(SpiderSpawnEvent.Position), ref e.Position, "%.2f");
			}

			ImGui.EndTable();
		}
	}

	private static ReadOnlySpan<char> GetSpiderTypeText(SpiderType spiderType) => spiderType switch
	{
		SpiderType.Spider1 => "Spider1",
		SpiderType.Spider2 => "Spider2",
		_ => throw new UnreachableException(),
	};
}
