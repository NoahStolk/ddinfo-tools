using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Core.Replay.Events.Data;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Events;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Timeline;

public static class ReplayTimelineActionsChild
{
	private static bool _insertAtEnd;
	private static int _tickIndex;
	private static int _tickCount = 5;

	public static void Render(ReplayEventsData eventsData)
	{
		if (ImGui.BeginChild("ActionsChild", new(320, 96)))
		{
			ImGui.PushItemWidth(128);
			ImGui.Checkbox("Insert at end", ref _insertAtEnd);
			ImGui.BeginDisabled(_insertAtEnd);
			ImGui.InputInt("Insert at tick index", ref _tickIndex);
			ImGui.EndDisabled();
			ImGui.InputInt("Amount of ticks to insert", ref _tickCount);
			ImGui.PopItemWidth();

			if (ImGui.Button("Insert empty data"))
			{
				if (_insertAtEnd)
				{
					ReplayEvent? lastEndEvent = eventsData.Events.LastOrDefault(e => e.GetEventType() == EventType.End);
					if (lastEndEvent == null)
					{
						// Add 1 second of data at the end of the replay.
						for (int i = 0; i < _tickCount; i++)
							FileStates.Replay.Object.EventsData.AddEvent(InputsEventData.CreateDefault());
					}
					else
					{
						// Add 1 second of data before the last End event.
						int indexOfLastEndEvent = eventsData.Events.IndexOf(lastEndEvent);
						InsertEmptyData(indexOfLastEndEvent);
					}
				}
				else
				{
					InsertEmptyData(_tickIndex);
				}

				TimelineCache.Clear();
			}
		}

		ImGui.EndChild(); // End ActionsChild
	}

	private static void InsertEmptyData(int index)
	{
		ReplayEvent? lastInitialInputsEvent = FileStates.Replay.Object.EventsData.Events.LastOrDefault(e => e.GetEventType() == EventType.InitialInputs);
		int indexOfLastInitialInputsEvent = FileStates.Replay.Object.EventsData.Events.IndexOf(lastInitialInputsEvent);
		index = Math.Clamp(index, indexOfLastInitialInputsEvent + 1, FileStates.Replay.Object.EventsData.Events.Count - 1);

		_tickCount = Math.Clamp(_tickCount, 1, 1000);

		for (int i = 0; i < _tickCount; i++)
			FileStates.Replay.Object.EventsData.InsertEvent(index, InputsEventData.CreateDefault());
	}
}
