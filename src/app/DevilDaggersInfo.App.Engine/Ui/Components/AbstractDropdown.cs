using DevilDaggersInfo.App.Engine.Maths.Numerics;
using Silk.NET.GLFW;

namespace DevilDaggersInfo.App.Engine.Ui.Components;

public abstract class AbstractDropdown : AbstractComponent
{
	private readonly List<AbstractDropdownEntry> _children = new();

	protected AbstractDropdown(IBounds bounds)
		: base(bounds)
	{
	}

	protected bool IsOpen { get; private set; }

	protected bool Hover { get; private set; }

	public override void Update(Vector2i<int> scrollOffset)
	{
		base.Update(scrollOffset);

		Hover = MouseUiContext.Contains(scrollOffset, Bounds);
		if (!Input.IsButtonPressed(MouseButton.Left))
			return;

		if (Hover)
			Toggle(!IsOpen);
		else
			Toggle(false);
	}

	public void AddChild(AbstractDropdownEntry child)
	{
		_children.Add(child);
	}

	public void Toggle(bool isOpen)
	{
		IsOpen = isOpen;

		foreach (AbstractDropdownEntry child in _children)
			child.IsActive = IsOpen;
	}
}
