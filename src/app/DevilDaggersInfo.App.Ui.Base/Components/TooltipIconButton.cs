using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Engine.Text;
using DevilDaggersInfo.App.Engine.Ui;
using DevilDaggersInfo.App.Ui.Base.Components.Styles;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;

namespace DevilDaggersInfo.App.Ui.Base.Components;

public class TooltipIconButton : IconButton
{
	private readonly string _tooltipText;
	private readonly TextAlign _textAlign;

	public TooltipIconButton(IBounds bounds, Action onClick, ButtonStyle buttonStyle, Texture texture, string tooltipText, TextAlign textAlign, Color disabledColor, Color enabledColor)
		: base(bounds, onClick, buttonStyle, texture, disabledColor, enabledColor)
	{
		_tooltipText = tooltipText;
		_textAlign = textAlign;
	}

	public override void Update(Vector2i<int> scrollOffset)
	{
		base.Update(scrollOffset);

		if (Hover && !IsDisabled)
		{
			Root.Game.TooltipContext = new()
			{
				Text = _tooltipText,
				ForegroundColor = Color.White,
				BackgroundColor = Color.Black,
				TextAlign = _textAlign,
			};
		}
	}
}
