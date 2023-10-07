using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Core.Wiki;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public sealed class DeathEvents : IEventTypeRenderer<DeathEvent>
{
	public static void Render(IReadOnlyList<(int Index, DeathEvent Event)> events, IReadOnlyList<EntityType> entityTypes, IReadOnlyList<EventColumn> columns)
	{
		ImGui.TextColored(Color.Red, "Death");

		if (ImGui.BeginTable("Death", columns.Count, EventTypeRendererUtils.EventTableFlags))
		{
			EventTypeRendererUtils.SetupColumns(columns);

			for (int i = 0; i < events.Count; i++)
			{
				ImGui.TableNextRow();

				(int index, DeathEvent e) = events[i];
				EventTypeRendererUtils.NextColumnText(Inline.Span(index));
				EventTypeRendererUtils.NextColumnText(Inline.Span(Deaths.GetDeathByType(GameConstants.CurrentVersion, (byte)e.DeathType)?.Name ?? "???"));
			}

			ImGui.EndTable();
		}
	}
}
