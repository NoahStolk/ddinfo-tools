using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Events;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Timeline;

public static class TimelineCache
{
	private static readonly List<Dictionary<EventType, List<(ReplayEvent Event, int EventIndex)>>> _tickData = [];

	public static IReadOnlyList<Dictionary<EventType, List<(ReplayEvent Event, int EventIndex)>>> TickData => _tickData;

	public static bool IsEmpty => _tickData.Count == 0;

	public static void Clear()
	{
		_tickData.Clear();
	}

	public static void Build(ReplayEventsData replayEventsData)
	{
		if (replayEventsData.Events.Count == 0)
			return;

		Clear();

		int tickIndex = 0;
		int eventIndex = 0;
		foreach (ReplayEvent replayEvent in replayEventsData.Events)
		{
			EventType eventType = replayEvent.GetEventType();
			if (tickIndex < _tickData.Count)
			{
				Dictionary<EventType, List<(ReplayEvent Event, int EventIndex)>> tickMarkers = _tickData[tickIndex];
				if (!tickMarkers.TryGetValue(eventType, out List<(ReplayEvent Event, int EventIndex)>? replayEvents))
				{
					replayEvents = [];
					tickMarkers[eventType] = replayEvents;
				}

				replayEvents.Add((replayEvent, eventIndex));
			}
			else
			{
				_tickData.Add(new() { [eventType] = [(replayEvent, eventIndex)] });
			}

			if (eventType is EventType.InitialInputs or EventType.Inputs)
				tickIndex++;

			eventIndex++;
		}
	}
}
