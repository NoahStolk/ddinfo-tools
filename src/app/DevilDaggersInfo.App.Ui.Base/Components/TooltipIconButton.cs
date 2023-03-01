using DevilDaggersInfo.App.Ui.Base.Components.Styles;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using Warp.NET.Ui;

namespace DevilDaggersInfo.App.Ui.Base.Components;

public class TooltipIconButton : IconButton
{
	private readonly string _tooltipText;

	public TooltipIconButton(IBounds bounds, Action onClick, ButtonStyle buttonStyle, Texture texture, string tooltipText, Color disabledColor, Color enabledColor)
		: base(bounds, onClick, buttonStyle, texture, disabledColor, enabledColor)
	{
		_tooltipText = tooltipText;
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
			};
		}
	}
}
