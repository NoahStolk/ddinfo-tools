using DevilDaggersInfo.App.Ui.Base.Components.Styles;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using Silk.NET.GLFW;
using System.Diagnostics;
using Warp.NET;
using Warp.NET.Text;
using Warp.NET.Ui;
using Warp.NET.Ui.Components;

namespace DevilDaggersInfo.App.Ui.Base.Components;

public class Hyperlink : AbstractComponent
{
	private readonly string _text;
	private readonly HyperlinkStyle _hyperlinkStyle;
	private readonly string _url;
	private bool _hover;

	public Hyperlink(IBounds bounds, string text, HyperlinkStyle hyperlinkStyle, string url)
		: base(bounds)
	{
		_text = text;
		_hyperlinkStyle = hyperlinkStyle;
		_url = url;
	}

	public override void Update(Vector2i<int> scrollOffset)
	{
		base.Update(scrollOffset);

		_hover = MouseUiContext.Contains(scrollOffset, Bounds);
		if (!_hover || !Input.IsButtonPressed(MouseButton.Left))
			return;

		Process.Start(new ProcessStartInfo(_url) { UseShellExecute = true });
	}

	public override void Render(Vector2i<int> scrollOffset)
	{
		base.Render(scrollOffset);

		if (_text.Length == 0)
			return;

		Vector2i<int> textPosition = _hyperlinkStyle.TextAlign switch
		{
			TextAlign.Middle => new Vector2i<int>(Bounds.X1 + Bounds.X2, Bounds.Y1 + Bounds.Y2) / 2,
			TextAlign.Left => new(Bounds.X1, Bounds.Y1),
			TextAlign.Right => new(Bounds.X2, Bounds.Y1),
			_ => throw new InvalidOperationException("Invalid text align."),
		};

		Color textColor = _hover ? _hyperlinkStyle.HoverTextColor : _hyperlinkStyle.TextColor;
		Root.Game.GetFontRenderer(_hyperlinkStyle.FontSize).Schedule(Vector2i<int>.One, scrollOffset + textPosition, Depth, textColor, _text, _hyperlinkStyle.TextAlign);
	}
}
