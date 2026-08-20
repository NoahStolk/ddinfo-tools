using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Events;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Timeline.EventTypes;
using Hexa.NET.ImGui;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Timeline;

internal sealed class ReplayTimelineSelectedEventsChild
{
	private readonly List<EditorEvent> _checkedEvents = [];

	public void Render(EditorReplayModel replay, List<EditorEvent> selectedEvents, int selectedTick, Action<EditorReplayModel, int> selectEvents)
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
			UtilsRendering.Checkbox(selectedTick, "Left"u8, ref e.Left, "On"u8, "Off"u8);
			ImGui.TableNextColumn();
			UtilsRendering.Checkbox(selectedTick, "Right"u8, ref e.Right, "On"u8, "Off"u8);
			ImGui.TableNextColumn();
			UtilsRendering.Checkbox(selectedTick, "Forward"u8, ref e.Forward, "On"u8, "Off"u8);
			ImGui.TableNextColumn();
			UtilsRendering.Checkbox(selectedTick, "Backward"u8, ref e.Backward, "On"u8, "Off"u8);
			ImGui.TableNextColumn();
			UtilsRendering.InputByteEnum(selectedTick, "Jump"u8, ref e.Jump, JumpTypeGen.Values, JumpTypeGen.NullTerminatedMemberNames);
			ImGui.TableNextColumn();
			UtilsRendering.InputByteEnum(selectedTick, "Shoot"u8, ref e.Shoot, ShootTypeGen.Values, ShootTypeGen.NullTerminatedMemberNames);
			ImGui.TableNextColumn();
			UtilsRendering.InputByteEnum(selectedTick, "ShootHoming"u8, ref e.ShootHoming, ShootTypeGen.Values, ShootTypeGen.NullTerminatedMemberNames);
			ImGui.TableNextColumn();
			UtilsRendering.InputShort(selectedTick, "MouseX"u8, ref e.MouseX);
			ImGui.TableNextColumn();
			UtilsRendering.InputShort(selectedTick, "MouseY"u8, ref e.MouseY);

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
			DuplicateSelectedEvents(replay, selectEvents);
		}

		ImGui.EndDisabled();

		if (ImGui.BeginChild("EventsTableWrapper"))
		{
			RenderEventsTable(replay, selectedEvents);
		}

		ImGui.EndChild();
	}

	private void RenderEventsTable(EditorReplayModel replay, List<EditorEvent> selectedEvents)
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
				if (ImGui.Checkbox(Inline.Utf8($"{eventType.AsUtf8DisplaySpan()}##EventCheckbox{i}"), ref temp))
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
				else if (ImGui.CollapsingHeader(Inline.Utf8($"Event data##{i}")))
				{
					if (replayEvent.EntityId.HasValue)
						ImGui.Text(Inline.Utf8($"Entity Id: {replayEvent.EntityId.Value}"));

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

	private void DuplicateSelectedEvents(EditorReplayModel replay, Action<EditorReplayModel, int> selectEvents)
	{
		if (_checkedEvents.Count == 0)
			return;

		foreach (EditorEvent replayEvent in _checkedEvents)
		{
			replay.AddEvent(replayEvent.TickIndex, replayEvent.Data.CloneEventData());
		}

		selectEvents(replay, _checkedEvents[0].TickIndex);
		TimelineCache.Clear();
		_checkedEvents.Clear();
	}
}
