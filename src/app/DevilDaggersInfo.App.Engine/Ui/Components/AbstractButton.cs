using DevilDaggersInfo.App.Engine.Maths.Numerics;
using Silk.NET.GLFW;

namespace DevilDaggersInfo.App.Engine.Ui.Components;

public abstract class AbstractButton : AbstractComponent
{
	private readonly Action _onClick;

	protected AbstractButton(IBounds bounds, Action onClick)
		: base(bounds)
	{
		_onClick = onClick;
	}

	protected bool Hover { get; private set; }

	public bool IsDisabled { get; set; }

	public override void Update(Vector2i<int> scrollOffset)
	{
		base.Update(scrollOffset);

		Hover = MouseUiContext.Contains(scrollOffset, Bounds);

		if (Hover && !IsDisabled && Input.IsButtonPressed(MouseButton.Left))
			_onClick();
	}
}
