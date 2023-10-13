namespace DevilDaggersInfo.Tools.Ui.Popups;

public abstract class Popup
{
	protected Popup(string id)
	{
		Id = id;
	}

	public string Id { get; }

	public bool HasOpened { get; set; }

	/// <summary>
	/// Renders the popup and returns whether the popup should be closed.
	/// </summary>
	/// <returns>Whether the popup should be closed.</returns>
	public abstract bool Render();
}
