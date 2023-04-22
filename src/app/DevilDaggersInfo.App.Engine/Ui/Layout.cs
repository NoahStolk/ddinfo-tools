namespace DevilDaggersInfo.App.Engine.Ui;

public class Layout : ILayout
{
	public Layout()
	{
		NestingContext = new(new NormalizedBounds(0, 0, 1, 1));
	}

	public NestingContext NestingContext { get; }
}
