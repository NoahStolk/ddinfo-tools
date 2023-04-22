using DevilDaggersInfo.App.Engine;
using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Engine.Text;
using DevilDaggersInfo.App.Engine.Ui;
using DevilDaggersInfo.App.Engine.Ui.Components;
using DevilDaggersInfo.App.Ui.Base.Components.Styles;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using Silk.NET.GLFW;

namespace DevilDaggersInfo.App.Ui.Base.Components;

public class ClickableLabel : AbstractComponent
{
	private readonly string _text;
	private readonly Action _onClick;
	private readonly ClickableLabelStyle _clickableLabelStyle;

	private bool _hover;

	public ClickableLabel(IBounds bounds, string text, Action onClick, ClickableLabelStyle clickableLabelStyle)
		: base(bounds)
	{
		_text = text;
		_onClick = onClick;
		_clickableLabelStyle = clickableLabelStyle;
	}

	public override void Update(Vector2i<int> scrollOffset)
	{
		base.Update(scrollOffset);

		_hover = MouseUiContext.Contains(scrollOffset, Bounds);
		if (!_hover || !Input.IsButtonPressed(MouseButton.Left))
			return;

		_onClick();
	}

	public override void Render(Vector2i<int> scrollOffset)
	{
		base.Render(scrollOffset);

		if (_text.Length == 0)
			return;

		Vector2i<int> textPosition = _clickableLabelStyle.TextAlign switch
		{
			TextAlign.Middle => new Vector2i<int>(Bounds.X1 + Bounds.X2, Bounds.Y1 + Bounds.Y2) / 2,
			TextAlign.Left => new(Bounds.X1 + _clickableLabelStyle.Padding, Bounds.Y1 + _clickableLabelStyle.Padding),
			TextAlign.Right => new(Bounds.X2 - _clickableLabelStyle.Padding, Bounds.Y1 + _clickableLabelStyle.Padding),
			_ => throw new InvalidOperationException("Invalid text align."),
		};

		Color textColor = _hover ? _clickableLabelStyle.HoverTextColor : _clickableLabelStyle.TextColor;
		Root.Game.GetFontRenderer(_clickableLabelStyle.FontSize).Schedule(Vector2i<int>.One, scrollOffset + textPosition, Depth, textColor, _text, _clickableLabelStyle.TextAlign);
	}
}
