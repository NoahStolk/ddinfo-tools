using DevilDaggersInfo.App.Engine.Maths.Numerics;
using Silk.NET.GLFW;

namespace DevilDaggersInfo.App.Engine.Ui.Components;

public abstract class AbstractCheckbox : AbstractComponent
{
	private readonly Action<bool> _onClick;

	protected AbstractCheckbox(IBounds bounds, Action<bool> onClick)
		: base(bounds)
	{
		_onClick = onClick;
	}

	public bool CurrentValue { get; set; }

	protected bool Hover { get; private set; }

	public bool IsDisabled { get; set; }

	public override void Update(Vector2i<int> scrollOffset)
	{
		base.Update(scrollOffset);

		Hover = MouseUiContext.Contains(scrollOffset, Bounds);

		if (Hover && !IsDisabled && Input.IsButtonPressed(MouseButton.Left))
		{
			CurrentValue = !CurrentValue;
			_onClick(CurrentValue);
		}
	}
}
