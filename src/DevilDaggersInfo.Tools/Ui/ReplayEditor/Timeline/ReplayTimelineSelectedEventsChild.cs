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

	public static void Render(ReplayEventsData replayEventsData, List<ReplayEvent> selectedEvents, EventCache selectedEventDataCache)
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

			int index = 0;
			for (int i = 0; i < selectedEvents.Count; i++)
			{
				ReplayEvent replayEvent = selectedEvents[i];
				EventType eventType = replayEvent.GetEventType();
				if (eventType is EventType.InitialInputs or EventType.Inputs or EventType.End)
					continue;

				ImGui.TableSetBgColor(ImGuiTableBgTarget.RowBg0, index++ % 2 == 0 ? 0xff0f0f0fU : 0x00000000U);

				ImGui.TableNextRow();
				ImGui.TableNextColumn();

				bool temp = _checkedEvents.Contains(replayEvent);
				ImGui.PushStyleColor(ImGuiCol.Text, EventTypeRendererUtils.GetEventTypeColor(eventType));
				if (ImGui.Checkbox(Inline.Span($"{EnumUtils.EventTypeFriendlyNames[eventType]}##EventCheckbox{i}"), ref temp))
				{
					if (temp)
						_checkedEvents.Add(replayEvent);
					else
						_checkedEvents.Remove(replayEvent);
				}

				ImGui.PopStyleColor();
				ImGui.TableNextColumn();

				if (eventType is EventType.Gem)
				{
					ImGui.Text("No data");
				}
				else if (ImGui.CollapsingHeader(Inline.Span($"Event data##{i}")))
				{
					int eventIndex = replayEventsData.Events.IndexOf(replayEvent);
					if (replayEvent.Data is BoidSpawnEventData boidSpawn)
						BoidSpawnEvents.RenderEdit(eventIndex, boidSpawn, replayEventsData);
					else if (replayEvent.Data is DaggerSpawnEventData daggerSpawn)
						DaggerSpawnEvents.RenderEdit(eventIndex, daggerSpawn, replayEventsData);
					else if (replayEvent.Data is EntityOrientationEventData entityOrientation)
						EntityOrientationEvents.RenderEdit(eventIndex, entityOrientation, replayEventsData);
					else if (replayEvent.Data is EntityPositionEventData entityPosition)
						EntityPositionEvents.RenderEdit(eventIndex, entityPosition, replayEventsData);
					else if (replayEvent.Data is EntityTargetEventData entityTarget)
						EntityTargetEvents.RenderEdit(eventIndex, entityTarget, replayEventsData);
					else if (replayEvent.Data is HitEventData hitEvent)
						HitEvents.RenderEdit(eventIndex, hitEvent, replayEventsData);
					else if (replayEvent.Data is LeviathanSpawnEventData leviathanSpawn)
						LeviathanSpawnEvents.RenderEdit(eventIndex, leviathanSpawn, replayEventsData);
					else if (replayEvent.Data is PedeSpawnEventData pedeSpawn)
						PedeSpawnEvents.RenderEdit(eventIndex, pedeSpawn, replayEventsData);
					else if (replayEvent.Data is SpiderEggSpawnEventData spiderEggSpawn)
						SpiderEggSpawnEvents.RenderEdit(eventIndex, spiderEggSpawn, replayEventsData);
					else if (replayEvent.Data is SpiderSpawnEventData spiderSpawn)
						SpiderSpawnEvents.RenderEdit(eventIndex, spiderSpawn, replayEventsData);
					else if (replayEvent.Data is SquidSpawnEventData squidSpawn)
						SquidSpawnEvents.RenderEdit(eventIndex, squidSpawn, replayEventsData);
					else if (replayEvent.Data is ThornSpawnEventData thornSpawn)
						ThornSpawnEvents.RenderEdit(eventIndex, thornSpawn, replayEventsData);
					else if (replayEvent.Data is TransmuteEventData transmute)
						TransmuteEvents.RenderEdit(eventIndex, transmute, replayEventsData);
				}
			}

			ImGui.EndTable();
		}

		if (ImGui.Button("Select all"))
		{
			_checkedEvents.Clear();
			_checkedEvents.AddRange(selectedEvents);
		}

		if (ImGui.Button("Deselect all"))
		{
			_checkedEvents.Clear();
		}

		ImGui.BeginDisabled(_checkedEvents.Count == 0);
		if (ImGui.Button("Delete selected events"))
		{
			foreach (ReplayEvent replayEvent in _checkedEvents.OrderByDescending(e => replayEventsData.Events.IndexOf(e)))
			{
				int eventIndex = replayEventsData.Events.IndexOf(replayEvent);
				replayEventsData.RemoveEvent(eventIndex);
				selectedEvents.Remove(replayEvent);
			}

			TimelineCache.Clear();
			_checkedEvents.Clear();
		}

		if (ImGui.Button("Duplicate selected events"))
		{
			DuplicateSelectedEvents(replayEventsData);
		}

		ImGui.EndDisabled();
	}

	private static void DuplicateSelectedEvents(ReplayEventsData replayEventsData)
	{
		if (_checkedEvents.Count == 0)
			return;

		int indexToInsertAt = _checkedEvents.Max(e => replayEventsData.Events.IndexOf(e));
		foreach (ReplayEvent replayEvent in _checkedEvents.OrderByDescending(e => replayEventsData.Events.IndexOf(e)))
		{
			replayEventsData.InsertEvent(indexToInsertAt, replayEvent.Data.CloneEventData());
		}

		// TODO: Reselect the current tick events.
		TimelineCache.Clear();
		_checkedEvents.Clear();
	}
}
