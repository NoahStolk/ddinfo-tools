namespace DevilDaggersInfo.Tools.Ui.Popups;

internal abstract class Popup(string id)
{
	public string Id { get; } = $"{id}##{Guid.NewGuid()}";

	public bool HasOpened { get; set; }

	/// <summary>
	/// Renders the popup and returns whether the popup should be closed.
	/// </summary>
	/// <returns>Whether the popup should be closed.</returns>
	public abstract bool Render();
}
