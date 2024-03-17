using DevilDaggersInfo.Tools.Ui.ReplayEditor.Data;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Events.EventTypes;

public interface IEventTypeRenderer<in T>
{
	static abstract int ColumnCount { get; }

	static abstract void SetupColumns();

	static abstract void Render(int eventIndex, int entityId, T e, EditorReplayModel replay);
}
