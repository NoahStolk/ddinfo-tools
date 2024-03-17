using DevilDaggersInfo.Core.Replay.Events.Data;
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
	private static readonly string[] _jumpTypeNamesArray = EnumUtils.JumpTypeNames.Values.ToArray();
	private static readonly string[] _shootTypeNamesArray = EnumUtils.ShootTypeNames.Values.ToArray();

	private static readonly List<EditorEvent> _checkedEvents = [];

	public static void Render(EditorReplayModel replay, List<EditorEvent> selectedEvents, int selectedTick)
	{
		if (selectedEvents.Count == 0)
		{
			ImGui.Text("No events selected");
			return;
		}

		ImGui.SeparatorText("Inputs");

		if (ImGui.BeginTable("InputsTable", 9, ImGuiTableFlags.Borders | ImGuiTableFlags.NoPadOuterX))
		{
			ImGui.TableSetupColumn("Left", ImGuiTableColumnFlags.WidthFixed, 64);
			ImGui.TableSetupColumn("Right", ImGuiTableColumnFlags.WidthFixed, 64);
			ImGui.TableSetupColumn("Forward", ImGuiTableColumnFlags.WidthFixed, 64);
			ImGui.TableSetupColumn("Backward", ImGuiTableColumnFlags.WidthFixed, 64);
			ImGui.TableSetupColumn("Jump", ImGuiTableColumnFlags.WidthFixed, 96);
			ImGui.TableSetupColumn("Shoot", ImGuiTableColumnFlags.WidthFixed, 96);
			ImGui.TableSetupColumn("Shoot Homing", ImGuiTableColumnFlags.WidthFixed, 96);
			ImGui.TableSetupColumn("Mouse X", ImGuiTableColumnFlags.WidthFixed, 64);
			ImGui.TableSetupColumn("Mouse Y", ImGuiTableColumnFlags.WidthFixed, 64);
			ImGui.TableHeadersRow();

			ImGui.TableNextRow();

			InputsEventData e = replay.InputsEvents[selectedTick];
			EventTypeRendererUtils.NextColumnCheckbox(selectedTick, nameof(InputsEventData.Left), ref e.Left, "On", "Off");
			EventTypeRendererUtils.NextColumnCheckbox(selectedTick, nameof(InputsEventData.Right), ref e.Right, "On", "Off");
			EventTypeRendererUtils.NextColumnCheckbox(selectedTick, nameof(InputsEventData.Forward), ref e.Forward, "On", "Off");
			EventTypeRendererUtils.NextColumnCheckbox(selectedTick, nameof(InputsEventData.Backward), ref e.Backward, "On", "Off");
			EventTypeRendererUtils.NextColumnInputByteEnum(selectedTick, nameof(InputsEventData.Jump), ref e.Jump, EnumUtils.JumpTypes, _jumpTypeNamesArray);
			EventTypeRendererUtils.NextColumnInputByteEnum(selectedTick, nameof(InputsEventData.Shoot), ref e.Shoot, EnumUtils.ShootTypes, _shootTypeNamesArray);
			EventTypeRendererUtils.NextColumnInputByteEnum(selectedTick, nameof(InputsEventData.ShootHoming), ref e.ShootHoming, EnumUtils.ShootTypes, _shootTypeNamesArray);
			EventTypeRendererUtils.NextColumnInputShort(selectedTick, nameof(InputsEventData.MouseX), ref e.MouseX);
			EventTypeRendererUtils.NextColumnInputShort(selectedTick, nameof(InputsEventData.MouseY), ref e.MouseY);

			ImGui.EndTable();
		}

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
				Remove(replay.BoidSpawnEvents);
				Remove(replay.DaggerSpawnEvents);
				Remove(replay.EntityOrientationEvents);
				Remove(replay.EntityPositionEvents);
				Remove(replay.EntityTargetEvents);
				Remove(replay.GemEvents);
				Remove(replay.HitEvents);
				Remove(replay.LeviathanSpawnEvents);
				Remove(replay.PedeSpawnEvents);
				Remove(replay.SpiderEggSpawnEvents);
				Remove(replay.SpiderSpawnEvents);
				Remove(replay.SquidSpawnEvents);
				Remove(replay.ThornSpawnEvents);
				Remove(replay.TransmuteEvents);

				void Remove(List<EditorEvent> list)
				{
					list.Remove(editorEvent);
				}

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
