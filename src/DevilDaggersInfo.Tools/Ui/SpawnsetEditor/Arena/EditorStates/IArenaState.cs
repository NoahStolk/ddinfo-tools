namespace DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Arena.EditorStates;

internal interface IArenaState
{
	void InitializeSession(ArenaMousePosition mousePosition);

	void Handle(ArenaMousePosition mousePosition);

	void HandleOutOfRange(ArenaMousePosition mousePosition);

	void Render(ArenaMousePosition mousePosition);
}
