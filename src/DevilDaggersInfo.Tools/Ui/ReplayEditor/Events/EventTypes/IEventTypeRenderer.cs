using DevilDaggersInfo.Core.Replay.Events.Enums;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public interface IEventTypeRenderer<T>
{
	static abstract void Render(IReadOnlyList<(int Index, T Event)> events, IReadOnlyList<EntityType> entityTypes, IReadOnlyList<EventColumn> columns);
}
