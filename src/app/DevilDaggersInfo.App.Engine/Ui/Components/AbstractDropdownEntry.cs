using DevilDaggersInfo.App.Engine.Maths.Numerics;
using Silk.NET.GLFW;

namespace DevilDaggersInfo.App.Engine.Ui.Components;

public abstract class AbstractDropdownEntry : AbstractComponent
{
	private readonly AbstractDropdown _parent;
	private readonly Action _onClick;

	protected AbstractDropdownEntry(IBounds bounds, AbstractDropdown parent, Action onClick)
		: base(bounds)
	{
		_parent = parent;
		_onClick = onClick;
	}

	protected bool Hover { get; private set; }

	public bool IsDisabled { get; set; }

	public override void Update(Vector2i<int> scrollOffset)
	{
		base.Update(scrollOffset);

		Hover = MouseUiContext.Contains(scrollOffset, Bounds);

		if (!Hover || IsDisabled || !Input.IsButtonPressed(MouseButton.Left))
			return;

		_onClick();
		_parent.Toggle(false);
	}
}
