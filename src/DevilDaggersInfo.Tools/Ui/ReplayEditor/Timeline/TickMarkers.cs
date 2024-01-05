using DevilDaggersInfo.Core.Replay.Events;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Events;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Timeline;

public record TickMarkers(int FrameIndex, Dictionary<EventType, List<ReplayEvent>> ReplayEvents);
