using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Events;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Timeline;

public static class TimelineCache
{
	private static readonly List<TickMarkers> _markers = [];

	public static IReadOnlyList<TickMarkers> Markers => _markers;

	public static bool IsEmpty => _markers.Count == 0;

	public static void Clear()
	{
		_markers.Clear();
	}

	public static void Build(ReplayEventsData replayEventsData)
	{
		if (replayEventsData.Events.Count == 0)
			return;

		Clear();

		int tickIndex = 0;
		foreach (ReplayEvent replayEvent in replayEventsData.Events)
		{
			EventType eventType = replayEvent.GetEventType();
			if (tickIndex < _markers.Count)
			{
				TickMarkers tickMarkers = _markers[tickIndex];
				if (!tickMarkers.ReplayEvents.TryGetValue(eventType, out List<ReplayEvent>? replayEvents))
				{
					replayEvents = [];
					tickMarkers.ReplayEvents[eventType] = replayEvents;
				}

				replayEvents.Add(replayEvent);
			}
			else
			{
				TickMarkers tickMarkers = new(tickIndex, new());
				tickMarkers.ReplayEvents.Add(eventType, [replayEvent]);
				_markers.Add(tickMarkers);
			}

			if (eventType is EventType.InitialInputs or EventType.Inputs)
				tickIndex++;
		}
	}
}
