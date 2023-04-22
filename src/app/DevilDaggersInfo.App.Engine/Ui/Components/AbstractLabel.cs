namespace DevilDaggersInfo.App.Engine.Ui.Components;

public abstract class AbstractLabel : AbstractComponent
{
	protected AbstractLabel(IBounds bounds, string text)
		: base(bounds)
	{
		Text = text;
	}

	public string Text { get; set; }
}
