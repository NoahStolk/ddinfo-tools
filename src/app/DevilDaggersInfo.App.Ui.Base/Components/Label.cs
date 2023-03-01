using DevilDaggersInfo.App.Ui.Base.Components.Styles;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.Rendering;
using DevilDaggersInfo.App.Ui.Base.Rendering.Scissors;
using Warp.NET.Text;
using Warp.NET.Ui;
using Warp.NET.Ui.Components;

namespace DevilDaggersInfo.App.Ui.Base.Components;

public class Label : AbstractLabel
{
	public Label(IBounds bounds, string text, LabelStyle labelStyle)
		: base(bounds, text)
	{
		LabelStyle = labelStyle;
	}

	public LabelStyle LabelStyle { get; set; }

	public bool RenderOverflow { get; set; } = true;

	public override void Render(Vector2i<int> scrollOffset)
	{
		if (!RenderOverflow)
			ScissorScheduler.PushScissor(Scissor.Create(Bounds, scrollOffset, ViewportState.Offset, ViewportState.Scale));

		base.Render(scrollOffset);

		if (Text.Length == 0)
			return;

		int padding = (int)MathF.Round((Bounds.Y2 - Bounds.Y1) / 4f);
		Vector2i<int> textPosition = LabelStyle.TextAlign switch
		{
			TextAlign.Middle => new Vector2i<int>(Bounds.X1 + Bounds.X2, Bounds.Y1 + Bounds.Y2) / 2,
			TextAlign.Left => new(Bounds.X1 + padding, Bounds.Y1 + padding),
			TextAlign.Right => new(Bounds.X2 - padding, Bounds.Y1 + padding),
			_ => throw new InvalidOperationException("Invalid text align."),
		};

		Root.Game.GetFontRenderer(LabelStyle.FontSize).Schedule(Vector2i<int>.One, scrollOffset + textPosition, Depth, LabelStyle.TextColor, Text, LabelStyle.TextAlign);

		if (!RenderOverflow)
			ScissorScheduler.PopScissor();
	}
}
