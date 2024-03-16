using DevilDaggersInfo.Core.Replay.Events.Data;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;

public record EditorEvent
{
	public EditorEvent(int tickIndex, IEventData data)
	{
		TickIndex = tickIndex;
		Data = data;
	}

	public int TickIndex { get; }
	public IEventData Data { get; }
}
