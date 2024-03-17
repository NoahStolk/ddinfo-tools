using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Events;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Timeline.EventTypes;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Timeline;

public static class ReplayTimelineSelectedEventsChild
{
	private static readonly List<EditorEvent> _checkedEvents = [];

	public static void Render(EditorReplayModel replay, List<EditorEvent> selectedEvents, EventCache selectedEventDataCache)
	{
		if (selectedEvents.Count == 0)
		{
			ImGui.Text("No events selected");
			return;
		}

		ImGui.SeparatorText("Inputs");

		if (selectedEventDataCache.InitialInputsEvents.Count == 1)
			EventTypeRendererUtils.RenderInputsTable<InitialInputsEventData, InitialInputsEvents>("InputsTable", selectedEventDataCache.InitialInputsEvents, replay);
		else if (selectedEventDataCache.InputsEvents.Count == 1)
			EventTypeRendererUtils.RenderInputsTable<InputsEventData, InputsEvents>("InputsTable", selectedEventDataCache.InputsEvents, replay);
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
				EditorEvent replayEvent = selectedEvents[i];
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
					if (replayEvent.Data is BoidSpawnEventData boidSpawn)
						BoidSpawn.RenderEdit(i, boidSpawn, replay);
					else if (replayEvent.Data is DaggerSpawnEventData daggerSpawn)
						DaggerSpawn.RenderEdit(i, daggerSpawn, replay);
					else if (replayEvent.Data is EntityOrientationEventData entityOrientation)
						EntityOrientation.RenderEdit(i, entityOrientation, replay);
					else if (replayEvent.Data is EntityPositionEventData entityPosition)
						EntityPosition.RenderEdit(i, entityPosition, replay);
					else if (replayEvent.Data is EntityTargetEventData entityTarget)
						EntityTarget.RenderEdit(i, entityTarget, replay);
					else if (replayEvent.Data is HitEventData hitEvent)
						Hit.RenderEdit(i, hitEvent, replay);
					else if (replayEvent.Data is LeviathanSpawnEventData leviathanSpawn)
						LeviathanSpawn.RenderEdit(i, leviathanSpawn, replay);
					else if (replayEvent.Data is PedeSpawnEventData pedeSpawn)
						PedeSpawn.RenderEdit(i, pedeSpawn, replay);
					else if (replayEvent.Data is SpiderEggSpawnEventData spiderEggSpawn)
						SpiderEggSpawn.RenderEdit(i, spiderEggSpawn, replay);
					else if (replayEvent.Data is SpiderSpawnEventData spiderSpawn)
						SpiderSpawn.RenderEdit(i, spiderSpawn, replay);
					else if (replayEvent.Data is SquidSpawnEventData squidSpawn)
						SquidSpawn.RenderEdit(i, squidSpawn, replay);
					else if (replayEvent.Data is ThornSpawnEventData thornSpawn)
						ThornSpawn.RenderEdit(i, thornSpawn, replay);
					else if (replayEvent.Data is TransmuteEventData transmute)
						Transmute.RenderEdit(i, transmute, replay);
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
			foreach (EditorEvent editorEvent in _checkedEvents)
			{
				// replayEventsData.RemoveEvent(eventIndex);
				// replay.
				selectedEvents.Remove(editorEvent);
			}

			TimelineCache.Clear();
			_checkedEvents.Clear();
		}

		if (ImGui.Button("Duplicate selected events"))
		{
			DuplicateSelectedEvents(replay);
		}

		ImGui.EndDisabled();
	}

	private static void DuplicateSelectedEvents(EditorReplayModel replay)
	{
		if (_checkedEvents.Count == 0)
			return;

		// int indexToInsertAt = _checkedEvents.Max(e => replayEventsData.Events.IndexOf(e));
		// foreach (EditorEvent replayEvent in _checkedEvents.OrderByDescending(e => replayEventsData.Events.IndexOf(e)))
		// {
		// 	replayEventsData.InsertEvent(indexToInsertAt, replayEvent.Data.CloneEventData());
		// }

		// TODO: Reselect the current tick events.
		TimelineCache.Clear();
		_checkedEvents.Clear();
	}
}
