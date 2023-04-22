using DevilDaggersInfo.App.Engine.Maths.Numerics;

namespace DevilDaggersInfo.App.Engine.Ui.Components;

public abstract class AbstractComponent
{
	protected AbstractComponent(IBounds bounds)
	{
		Bounds = bounds;
		NestingContext = new(bounds);
	}

	public IBounds Bounds { get; set; }
	public bool IsActive { get; set; } = true;
	public int Depth { get; set; }
	public NestingContext NestingContext { get; }

	public virtual void Update(Vector2i<int> scrollOffset)
	{
		NestingContext.Update(scrollOffset);
	}

	public virtual void Render(Vector2i<int> scrollOffset)
	{
		NestingContext.Render(scrollOffset);
	}
}
