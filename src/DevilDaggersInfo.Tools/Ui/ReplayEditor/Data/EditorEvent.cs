using DevilDaggersInfo.Core.Replay.Events.Data;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;

internal sealed record EditorEvent(int TickIndex, int? EntityId, IEventData Data)
{
	public int? EntityId { get; set; } = EntityId;
}
