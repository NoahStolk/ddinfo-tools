using DevilDaggersInfo.Core.Replay.Events.Data;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;

public record EditorEvent
{
	public EditorEvent(int tickIndex, int? entityId, IEventData data)
	{
		TickIndex = tickIndex;
		EntityId = entityId;
		Data = data;
	}

	public int TickIndex { get; }
	public int? EntityId { get; }
	public IEventData Data { get; }
}
