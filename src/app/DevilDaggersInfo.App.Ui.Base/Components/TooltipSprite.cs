using DevilDaggersInfo.App.Engine.Extensions;
using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Engine.Text;
using DevilDaggersInfo.App.Engine.Ui;
using DevilDaggersInfo.App.Engine.Ui.Components;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;

namespace DevilDaggersInfo.App.Ui.Base.Components;

public class TooltipSprite : AbstractComponent
{
	private readonly Texture _texture;
	private readonly Color _textureColor;
	private readonly Vector2 _textureSize;
	private readonly string _tooltipText;
	private readonly TextAlign _textAlign;

	public TooltipSprite(IBounds bounds, Texture texture, Color textureColor, string tooltipText, TextAlign textAlign)
		: base(bounds)
	{
		_texture = texture;
		_textureColor = textureColor;
		_textureSize = new(texture.Width, texture.Height);
		_tooltipText = tooltipText;
		_textAlign = textAlign;
	}

	public override void Update(Vector2i<int> scrollOffset)
	{
		base.Update(scrollOffset);

		if (MouseUiContext.Contains(scrollOffset, Bounds))
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

	public override void Render(Vector2i<int> scrollOffset)
	{
		base.Render(scrollOffset);

		Vector2i<int> scale = Bounds.Size;
		Vector2i<int> topLeft = Bounds.TopLeft;
		Vector2i<int> center = topLeft + scale / 2;
		Root.Game.SpriteRenderer.Schedule(_textureSize, (scrollOffset + center).ToVector2(), Depth + 2, _texture, _textureColor);
	}
}
