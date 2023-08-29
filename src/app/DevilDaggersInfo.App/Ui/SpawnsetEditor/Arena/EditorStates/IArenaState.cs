namespace DevilDaggersInfo.App.Ui.SpawnsetEditor.Arena.EditorStates;

public interface IArenaState
{
	void InitializeSession(ArenaMousePosition mousePosition);

	void Handle(ArenaMousePosition mousePosition);

	void HandleOutOfRange(ArenaMousePosition mousePosition);

	void Render(ArenaMousePosition mousePosition);
}
