namespace DevilDaggersInfo.App.Ui.Popups;

public abstract class Popup
{
	protected Popup(string id)
	{
		Id = id;
	}

	public string Id { get; }

	public bool HasOpened { get; set; }

	public abstract bool Render();
}
