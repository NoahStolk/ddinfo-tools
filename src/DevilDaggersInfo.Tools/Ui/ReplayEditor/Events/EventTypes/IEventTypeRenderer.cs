using DevilDaggersInfo.Core.Replay.Events.Enums;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public interface IEventTypeRenderer<in T>
{
	static abstract void Render(int index, T e, IReadOnlyList<EntityType> entityTypes);
}
