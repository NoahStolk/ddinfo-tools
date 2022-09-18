using DevilDaggersInfo.App.Tools.Enums;
using Warp.Ui;
using Warp.Ui.Components;

namespace DevilDaggersInfo.App.Tools.Components;

public class TextInput : AbstractTextInput
{
	private const float _cursorTimerSwitch = 0.45f;

	public Color BackgroundColor { get; set; }
	public Color BorderColor { get; set; }
	public Color HoverBackgroundColor { get; set; }
	public Color TextColor { get; set; }
	public Color ActiveBorderColor { get; set; }
	public Color CursorColor { get; set; }
	public Color SelectionColor { get; set; }
	public int Margin { get; set; }
	public int BorderSize { get; set; }

	public TextInput(Rectangle metric, bool isNumeric, Color backgroundColor, Color borderColor, Color hoverBackgroundColor, Color textColor, Color activeBorderColor, Color cursorColor, Color selectionColor, int margin, int borderSize)
		: base(metric, isNumeric)
	{
		BackgroundColor = backgroundColor;
		BorderColor = borderColor;
		HoverBackgroundColor = hoverBackgroundColor;
		TextColor = textColor;
		ActiveBorderColor = activeBorderColor;
		CursorColor = cursorColor;
		SelectionColor = selectionColor;
		Margin = margin;
		BorderSize = borderSize;
	}

	public override void Render(Vector2i<int> parentPosition)
	{
		base.Render(parentPosition);

		int padding = (int)MathF.Round((Metric.Y2 - Metric.Y1) / 4f);
		Vector2i<int> marginVec = new(Margin);
		Vector2i<int> borderVec = new(BorderSize);
		Vector2i<int> fullScale = new(Metric.X2 - Metric.X1, Metric.Y2 - Metric.Y1);
		Vector2i<int> topLeft = new(Metric.X1, Metric.Y1);
		Vector2i<int> center = topLeft + fullScale / 2;
		Vector2i<int> scale = fullScale - marginVec;

		Base.Game.UiRenderer.RenderCenter(scale + borderVec, parentPosition + center, Depth, IsSelected ? ActiveBorderColor : BorderColor);
		Base.Game.UiRenderer.RenderCenter(scale, parentPosition + center, Depth + 1, Hover ? HoverBackgroundColor : BackgroundColor);

		int charWidth = Base.Game.Font.Texture.Width / Base.Game.Font.CharAmount;
		if (CursorPositionStart == CursorPositionEnd && CursorTimer <= _cursorTimerSwitch && IsSelected)
		{
			int cursorPositionX = Metric.X1 + CursorPositionStart * charWidth + padding;
			Base.Game.UiRenderer.RenderTopLeft(new(2, Metric.Size.Y - Margin * 2), parentPosition + new Vector2i<int>(cursorPositionX, Metric.Y1 + Margin), Depth + 2, CursorColor);
		}
		else if (GetSelectionLength() > 0)
		{
			int selectionStart = Math.Min(CursorPositionStart, CursorPositionEnd);
			int cursorSelectionStartX = Metric.X1 + selectionStart * charWidth + padding;
			Base.Game.UiRenderer.RenderTopLeft(new(GetSelectionLength() * charWidth + 1, Metric.Size.Y - Margin * 2), parentPosition + new Vector2i<int>(cursorSelectionStartX, Metric.Y1 + Margin), Depth + 2, SelectionColor);
		}
	}

	public override void RenderText(Vector2i<int> parentPosition)
	{
		base.RenderText(parentPosition);

		int padding = (int)MathF.Round((Metric.Y2 - Metric.Y1) / 4f);
		Vector2i<int> position = new(Metric.X1 + padding, Metric.Y1 + padding);
		Base.Game.MonoSpaceFontRenderer.Render(Vector2i<int>.One, parentPosition + position, Depth + 3, TextColor, Value, TextAlign.Left);
	}
}
