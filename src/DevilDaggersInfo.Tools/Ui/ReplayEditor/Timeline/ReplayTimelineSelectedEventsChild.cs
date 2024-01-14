using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Events;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Timeline;

public static class ReplayTimelineSelectedEventsChild
{
	private static readonly List<ReplayEvent> _checkedEvents = [];

	public static void Render(ReplayEventsData replayEventsData, IReadOnlyList<ReplayEvent> selectedEvents, EventCache selectedEventDataCache)
	{
		if (selectedEvents.Count == 0)
		{
			ImGui.Text("No events selected");
			return;
		}

		ImGui.SeparatorText("Inputs");

		if (selectedEventDataCache.InitialInputsEvents.Count == 1)
			EventTypeRendererUtils.RenderInputsTable<InitialInputsEventData, InitialInputsEvents>("InputsTable", selectedEventDataCache.InitialInputsEvents, replayEventsData);
		else if (selectedEventDataCache.InputsEvents.Count == 1)
			EventTypeRendererUtils.RenderInputsTable<InputsEventData, InputsEvents>("InputsTable", selectedEventDataCache.InputsEvents, replayEventsData);
		else if (selectedEventDataCache.EndEvents.Count == 1)
			ImGui.Text("End of replay / inputs");
		else
			ImGui.TextColored(Color.Red, "Invalid replay data");

		ImGui.SeparatorText("Events");

		if (ImGui.BeginTable("EventsTable", 2, ImGuiTableFlags.Borders | ImGuiTableFlags.NoPadOuterX))
		{
			ImGui.TableSetupColumn("EventsTableColumnEventType", ImGuiTableColumnFlags.WidthFixed, 160);

			for (int i = 0; i < selectedEvents.Count; i++)
			{
				ReplayEvent replayEvent = selectedEvents[i];

				ImGui.TableSetBgColor(ImGuiTableBgTarget.RowBg0, i % 2 == 0 ? 0xff0f0f0fU : 0x00000000U);

				ImGui.TableNextRow();
				ImGui.TableNextColumn();

				EventType eventType = replayEvent.GetEventType();
				bool temp = _checkedEvents.Contains(replayEvent);
				if (ImGui.Checkbox(Inline.Span($"##EventCheckbox{i}"), ref temp))
				{
					if (temp)
						_checkedEvents.Remove(replayEvent);
					else
						_checkedEvents.Add(replayEvent);
				}

				ImGui.SameLine();
				ImGui.TextColored(EventTypeRendererUtils.GetEventTypeColor(eventType), EnumUtils.EventTypeFriendlyNames[eventType]);

				ImGui.TableNextColumn();

				if (ImGui.CollapsingHeader(Inline.Span($"Event data##{i}")))
				{
					int eventIndex = replayEventsData.Events.IndexOf(replayEvent);
					if (replayEvent.Data is BoidSpawnEventData boidSpawn)
						BoidSpawnEvents.RenderEdit(eventIndex, boidSpawn, replayEventsData);
					else
						ImGui.Text("Not implemented");
				}
			}

			ImGui.EndTable();
		}

		if (ImGui.Button("Delete selected events"))
		{
			foreach (ReplayEvent replayEvent in _checkedEvents.OrderByDescending(e => e))
			{
				int eventIndex = replayEventsData.Events.IndexOf(replayEvent);
				replayEventsData.RemoveEvent(eventIndex);
			}

			_checkedEvents.Clear();
		}

		if (ImGui.Button("Duplicate selected events"))
		{
			int indexToInsertAt = _checkedEvents.Max(e => replayEventsData.Events.IndexOf(e));
			foreach (ReplayEvent replayEvent in _checkedEvents.OrderByDescending(e => e))
			{
				replayEventsData.InsertEvent(indexToInsertAt, replayEvent.Data); // TODO: Use with { }
			}

			_checkedEvents.Clear();
		}
	}
}
