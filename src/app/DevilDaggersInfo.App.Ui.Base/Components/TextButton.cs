using DevilDaggersInfo.App.Ui.Base.Components.Styles;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using Warp.NET.Text;
using Warp.NET.Ui;

namespace DevilDaggersInfo.App.Ui.Base.Components;

public class TextButton : Button
{
	public TextButton(IBounds bounds, Action onClick, ButtonStyle buttonStyle, TextButtonStyle textButtonStyle, string text)
		: base(bounds, onClick, buttonStyle)
	{
		TextButtonStyle = textButtonStyle;
		Text = text;
	}

	public string Text { get; set; }

	public TextButtonStyle TextButtonStyle { get; set; }

	public override void Render(Vector2i<int> scrollOffset)
	{
		base.Render(scrollOffset);

		if (Text.Length == 0)
			return;

		int padding = (int)MathF.Round((Bounds.Y2 - Bounds.Y1) / 4f);
		Vector2i<int> textPosition = TextButtonStyle.TextAlign switch
		{
			TextAlign.Middle => new Vector2i<int>(Bounds.X1 + Bounds.X2, Bounds.Y1 + Bounds.Y2) / 2,
			TextAlign.Left => new(Bounds.X1 + padding, Bounds.Y1 + padding),
			TextAlign.Right => new(Bounds.X2 - padding, Bounds.Y1 + padding),
			_ => throw new InvalidOperationException("Invalid text align."),
		};

		Root.Game.GetFontRenderer(TextButtonStyle.FontSize).Schedule(Vector2i<int>.One, scrollOffset + textPosition, Depth + 2, TextButtonStyle.TextColor, Text, TextButtonStyle.TextAlign);
	}
}
