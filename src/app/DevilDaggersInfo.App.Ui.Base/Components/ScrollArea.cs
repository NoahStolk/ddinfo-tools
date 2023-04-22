using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Engine.Ui;
using DevilDaggersInfo.App.Engine.Ui.Components;
using DevilDaggersInfo.App.Ui.Base.Components.Styles;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.Rendering;
using DevilDaggersInfo.App.Ui.Base.Rendering.Scissors;

namespace DevilDaggersInfo.App.Ui.Base.Components;

public class ScrollArea : AbstractScrollArea
{
	public ScrollArea(IBounds bounds, int scrollAmountInPixels, int scrollbarWidth, ScrollAreaStyle scrollAreaStyle)
		: base(bounds, scrollAmountInPixels, scrollbarWidth)
	{
		ScrollAreaStyle = scrollAreaStyle;
	}

	public ScrollAreaStyle ScrollAreaStyle { get; set; }

	public override void Render(Vector2i<int> scrollOffset)
	{
		ScissorScheduler.PushScissor(Scissor.Create(ContentBounds, scrollOffset, ViewportState.Offset, ViewportState.Scale));

		// Render content.
		base.Render(scrollOffset);

		ScissorScheduler.PopScissor();

		// Render scrollbar.
		Vector2i<int> borderVec = new(ScrollAreaStyle.BorderSize * 2);
		Vector2i<int> scale = ScrollbarBounds.Size;

		Root.Game.RectangleRenderer.Schedule(scale, scrollOffset + ScrollbarBounds.Center, Depth, ScrollAreaStyle.BorderColor);
		Root.Game.RectangleRenderer.Schedule(scale - borderVec, scrollOffset + ScrollbarBounds.Center, Depth + 1, IsDraggingScrollbar ? ScrollAreaStyle.DraggingBackgroundColor : IsScrollbarHovering ? ScrollAreaStyle.HoveringBackgroundColor : ScrollAreaStyle.BackgroundColor);

		int thumbPadding = (ScrollAreaStyle.BorderSize + ScrollAreaStyle.ThumbPadding) * 2;
		Vector2i<int> thumbScale = new(scale.X - thumbPadding, ScrollbarHeight - thumbPadding + 1); // + 1 is necessary for some reason.
		Vector2i<int> thumbOffset = new(0, ScrollbarStartY);
		Root.Game.RectangleRenderer.Schedule(thumbScale, scrollOffset + new Vector2i<int>(ScrollbarBounds.Center.X, ScrollbarBounds.TopLeft.Y + thumbScale.Y / 2 + thumbPadding / 2) + thumbOffset, Depth + 2, ScrollAreaStyle.ThumbColor);
	}
}
