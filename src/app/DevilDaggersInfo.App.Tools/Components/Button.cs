using DevilDaggersInfo.App.Tools.Enums;
using DevilDaggersInfo.App.Tools.Renderers;
using DevilDaggersInfo.App.Tools.States;
using DevilDaggersInfo.App.Tools.Utils;
using Warp.Ui;
using Warp.Ui.Components;

namespace DevilDaggersInfo.App.Tools.Components;

public class Button : AbstractButton
{
	public Color BackgroundColor { get; set; }
	public Color BorderColor { get; set; }
	public Color HoverBackgroundColor { get; set; }
	public Color TextColor { get; set; }
	public string Text { get; set; }
	public TextAlign TextAlign { get; set; }
	public int BorderSize { get; set; }
	public bool UseSmallFont { get; set; }

	public Button(Rectangle metric, Action onClick, Color backgroundColor, Color borderColor, Color hoverBackgroundColor, Color textColor, string text, TextAlign textAlign, int borderSize, bool useSmallFont)
		: base(metric, onClick)
	{
		BackgroundColor = backgroundColor;
		BorderColor = borderColor;
		HoverBackgroundColor = hoverBackgroundColor;
		TextColor = textColor;
		Text = text;
		TextAlign = textAlign;
		BorderSize = borderSize;
		UseSmallFont = useSmallFont;
	}

	public override void Render(Vector2i<int> parentPosition)
	{
		base.Render(parentPosition);

		Vector2i<int> borderVec = new(BorderSize);
		Vector2i<int> scale = Metric.Size;
		Vector2i<int> topLeft = new(Metric.X1, Metric.Y1);
		Vector2i<int> center = topLeft + scale / 2;

		Base.Game.UiRenderer.RenderCenter(scale, parentPosition + center, Depth, BorderColor);
		Base.Game.UiRenderer.RenderCenter(scale - borderVec, parentPosition + center, Depth + 1, Hover ? HoverBackgroundColor : BackgroundColor);
	}

	public override void RenderText(Vector2i<int> parentPosition)
	{
		base.RenderText(parentPosition);

		int padding = (int)MathF.Round((Metric.Y2 - Metric.Y1) / 4f);
		Vector2i<int> textPosition = TextAlign switch
		{
			TextAlign.Middle => new Vector2i<int>(Metric.X1 + Metric.X2, Metric.Y1 + Metric.Y2) / 2,
			TextAlign.Left => new(Metric.X1 + padding, Metric.Y1 + padding),
			TextAlign.Right => new(Metric.X2 - padding, Metric.Y1 + padding),
			_ => throw new InvalidOperationException("Invalid text align."),
		};

		MonoSpaceFontRenderer fontRenderer = UseSmallFont ? Base.Game.MonoSpaceSmallFontRenderer : Base.Game.MonoSpaceFontRenderer;
		fontRenderer.Render(Vector2i<int>.One, parentPosition + textPosition, Depth + 2, TextColor, Text, TextAlign);
	}

	public class MenuButton : Button
	{
		public MenuButton(Rectangle metric, Action onClick, string text)
			: base(metric, onClick, Color.Black, Color.White, Color.Gray(0.75f), Color.White, text, TextAlign.Middle, 2, false)
		{
			Depth = 102;
			IsActive = false;
		}
	}

	public class PathButton : Button
	{
		public PathButton(Rectangle metric, Action onClick, string text, Color textColor)
			: base(metric, onClick, Color.Black, Color.Gray(0.7f), Color.Gray(0.4f), textColor, text, TextAlign.Left, 2, false)
		{
		}
	}

	public class ArenaButton : Button
	{
		public ArenaButton(Rectangle metric, Action onClick, Color backgroundColor, Color textColor, string text, bool useSmallFont)
			: base(metric, onClick, backgroundColor, Color.Black, Color.Blue, textColor, text, TextAlign.Middle, 2, useSmallFont)
		{
		}
	}

	public class HeightButton : Button
	{
		private readonly float _height;
		private readonly Color _heightColor;

		public HeightButton(Rectangle metric, Action onClick, float height)
			: base(metric, onClick, Color.White, Color.Black, Color.Black, Color.Black, string.Empty, TextAlign.Middle, 2, false)
		{
			_height = height;
			_heightColor = TileUtils.GetColorFromHeight(height);

			BackgroundColor = _heightColor;
			HoverBackgroundColor = _heightColor.Intensify(128); // TODO: Find a better way to do this.
			TextColor = BackgroundColor.ReadableColorForBrightness();
			Text = height.ToString();
			UseSmallFont = Text.Length > 2;
		}

		public override void Update(Vector2i<int> parentPosition)
		{
			base.Update(parentPosition);

			BackgroundColor = Math.Abs(StateManager.ArenaEditorState.SelectedHeight - _height) < 0.001f ? Color.Blue : _heightColor;
		}
	}
}
