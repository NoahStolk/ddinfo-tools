using DevilDaggersInfo.App.Tools.Enums;
using Warp.Ui;
using Warp.Ui.Components;

namespace DevilDaggersInfo.App.Tools.Components;

public class Label : AbstractLabel
{
	private readonly Color _textColor;
	private readonly TextAlign _textAlign;

	public Label(Rectangle metric, Color textColor, string text, TextAlign textAlign)
		: base(metric, text)
	{
		_textColor = textColor;
		_textAlign = textAlign;
	}

	public override void RenderText(Vector2i<int> parentPosition)
	{
		base.RenderText(parentPosition);

		int padding = (int)MathF.Round((Metric.Y2 - Metric.Y1) / 4f);
		Vector2i<int> textPosition = _textAlign switch
		{
			TextAlign.Middle => new Vector2i<int>(Metric.X1 + Metric.X2, Metric.Y1 + Metric.Y2) / 2,
			TextAlign.Left => new(Metric.X1 + padding, Metric.Y1 + padding),
			TextAlign.Right => new(Metric.X2 - padding, Metric.Y1 + padding),
			_ => throw new InvalidOperationException("Invalid text align."),
		};

		Base.Game.MonoSpaceFontRenderer.Render(Vector2i<int>.One, parentPosition + textPosition, Depth + 2, _textColor, Text, _textAlign);
	}
}
