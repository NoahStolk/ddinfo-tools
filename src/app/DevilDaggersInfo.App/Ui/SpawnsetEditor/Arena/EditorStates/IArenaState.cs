namespace DevilDaggersInfo.App.Ui.SpawnsetEditor.Arena.EditorStates;

public interface IArenaState
{
	void Handle(ArenaMousePosition mousePosition);

	void HandleOutOfRange(ArenaMousePosition mousePosition);

	void Render(ArenaMousePosition mousePosition);
}
