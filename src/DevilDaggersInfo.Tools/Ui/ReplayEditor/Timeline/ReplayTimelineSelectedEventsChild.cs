using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Events;
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
			ImGui.TableNextColumn();
			UtilsRendering.Checkbox(selectedTick, nameof(InputsEventData.Left), ref e.Left, "On", "Off");
			ImGui.TableNextColumn();
			UtilsRendering.Checkbox(selectedTick, nameof(InputsEventData.Right), ref e.Right, "On", "Off");
			ImGui.TableNextColumn();
			UtilsRendering.Checkbox(selectedTick, nameof(InputsEventData.Forward), ref e.Forward, "On", "Off");
			ImGui.TableNextColumn();
			UtilsRendering.Checkbox(selectedTick, nameof(InputsEventData.Backward), ref e.Backward, "On", "Off");
			ImGui.TableNextColumn();
			UtilsRendering.InputByteEnum(selectedTick, nameof(InputsEventData.Jump), ref e.Jump, EnumUtils.JumpTypes, _jumpTypeNamesArray);
			ImGui.TableNextColumn();
			UtilsRendering.InputByteEnum(selectedTick, nameof(InputsEventData.Shoot), ref e.Shoot, EnumUtils.ShootTypes, _shootTypeNamesArray);
			ImGui.TableNextColumn();
			UtilsRendering.InputByteEnum(selectedTick, nameof(InputsEventData.ShootHoming), ref e.ShootHoming, EnumUtils.ShootTypes, _shootTypeNamesArray);
			ImGui.TableNextColumn();
			UtilsRendering.InputShort(selectedTick, nameof(InputsEventData.MouseX), ref e.MouseX);
			ImGui.TableNextColumn();
			UtilsRendering.InputShort(selectedTick, nameof(InputsEventData.MouseY), ref e.MouseY);

			ImGui.EndTable();
		}

		if (selectedEvents.Count == 0)
		{
			ImGui.Text("No events selected");
			return;
		}

		ImGui.SeparatorText("Events");

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
				replay.RemoveEvent(editorEvent);

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

		if (ImGui.BeginChild("EventsTableWrapper"))
		{
			RenderEventsTable(replay, selectedEvents);
		}

		ImGui.EndChild();
	}

	private static void RenderEventsTable(EditorReplayModel replay, List<EditorEvent> selectedEvents)
	{
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
				ImGui.PushStyleColor(ImGuiCol.Text, eventType.GetColor());
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
					if (replayEvent.EntityId.HasValue)
						ImGui.Text(Inline.Span($"Entity Id: {replayEvent.EntityId.Value}"));

					if (replayEvent.Data is BoidSpawnEventData boidSpawn)
						BoidSpawn.RenderEdit(i, boidSpawn, replay);
					else if (replayEvent.Data is DaggerSpawnEventData daggerSpawn)
						DaggerSpawn.RenderEdit(i, daggerSpawn);
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
						PedeSpawn.RenderEdit(i, pedeSpawn);
					else if (replayEvent.Data is SpiderEggSpawnEventData spiderEggSpawn)
						SpiderEggSpawn.RenderEdit(i, spiderEggSpawn, replay);
					else if (replayEvent.Data is SpiderSpawnEventData spiderSpawn)
						SpiderSpawn.RenderEdit(i, spiderSpawn);
					else if (replayEvent.Data is SquidSpawnEventData squidSpawn)
						SquidSpawn.RenderEdit(i, squidSpawn);
					else if (replayEvent.Data is ThornSpawnEventData thornSpawn)
						ThornSpawn.RenderEdit(i, thornSpawn);
					else if (replayEvent.Data is TransmuteEventData transmute)
						Transmute.RenderEdit(i, transmute, replay);
				}
			}

			ImGui.EndTable();
		}
	}

	private static void DuplicateSelectedEvents(EditorReplayModel replay)
	{
		if (_checkedEvents.Count == 0)
			return;

		foreach (EditorEvent replayEvent in _checkedEvents)
		{
			replay.AddEvent(replayEvent.TickIndex, replayEvent.Data.CloneEventData());
		}

		ReplayTimelineChild.SelectEvents(replay, _checkedEvents[0].TickIndex);
		TimelineCache.Clear();
		_checkedEvents.Clear();
	}
}
