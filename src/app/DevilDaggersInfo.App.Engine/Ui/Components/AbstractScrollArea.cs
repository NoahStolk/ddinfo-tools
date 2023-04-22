using DevilDaggersInfo.App.Engine.Extensions;
using DevilDaggersInfo.App.Engine.Maths.Numerics;
using Silk.NET.GLFW;

namespace DevilDaggersInfo.App.Engine.Ui.Components;

public abstract class AbstractScrollArea : AbstractComponent
{
	private readonly int _scrollAmountInPixels;

	private int _holdStartMouseY;
	private int _scrollbarStartYOld;

	private int _contentHeight;

	private (int Min, int Max)? _scheduledScrollTargetMinMax;

	protected AbstractScrollArea(IBounds bounds, int scrollAmountInPixels, int scrollbarWidth)
		: base(bounds)
	{
		_scrollAmountInPixels = scrollAmountInPixels;

		ContentBounds = Bounds.CreateNested(0, 0, bounds.Size.X - scrollbarWidth, bounds.Size.Y);
		ScrollbarBounds = Bounds.CreateNested(bounds.Size.X - scrollbarWidth, 0, scrollbarWidth, bounds.Size.Y);

		NestingContext.OnUpdateQueue = OnUpdateQueue;
	}

	public bool IsScrollbarHovering { get; private set; }
	public bool IsDraggingScrollbar { get; private set; }

	public int ScrollbarStartY { get; private set; }
	public int ScrollbarHeight { get; private set; }

	public IBounds ContentBounds { get; }
	public IBounds ScrollbarBounds { get; }

	/// <summary>
	/// Schedules a scroll to a specific Y position. This position will be applied after the next invocation of the <see cref="NestingContext.OnUpdateQueue"/> action on the scroll area's <see cref="NestingContext"/>.
	/// </summary>
	/// <param name="min">The min Y position to scroll to.</param>
	/// <param name="max">The max Y position to scroll to.</param>
	public void ScheduleScrollTarget(int min, int max)
	{
		_scheduledScrollTargetMinMax = (min, max);
	}

	private void OnUpdateQueue()
	{
		RecalculateHeight();

		ApplyScheduledScrollTarget();

		_scheduledScrollTargetMinMax = null;

		void ApplyScheduledScrollTarget()
		{
			if (!_scheduledScrollTargetMinMax.HasValue)
				return;

			// If the target is already visible, do nothing.
			if (IsTargetVisible(_scheduledScrollTargetMinMax.Value.Min, _scheduledScrollTargetMinMax.Value.Max))
				return;

			// Continuously move up or down until the target is visible.
			bool moveUp = -NestingContext.ScrollOffset.Y >= _scheduledScrollTargetMinMax.Value.Min;
			while (!IsTargetVisible(_scheduledScrollTargetMinMax.Value.Min, _scheduledScrollTargetMinMax.Value.Max))
				UpdateScrollOffsetAndScrollbarPosition(NestingContext.ScrollOffset + new Vector2i<int>(0, moveUp ? 1 : -1));
		}

		bool IsTargetVisible(int targetMin, int targetMax)
		{
			int viewStart = -NestingContext.ScrollOffset.Y;
			int viewEnd = viewStart + ContentBounds.Size.Y;
			return targetMin >= viewStart && targetMin <= viewEnd && targetMax >= viewStart && targetMax <= viewEnd;
		}
	}

	private void RecalculateHeight()
	{
		int min = NestingContext.OrderedComponents.Count == 0 ? 0 : NestingContext.OrderedComponents.Min(b => b.Bounds.Y1);
		int max = NestingContext.OrderedComponents.Count == 0 ? 0 : NestingContext.OrderedComponents.Max(b => b.Bounds.Y2);
		_contentHeight = max - min;
		ScrollbarHeight = _contentHeight == 0 ? ScrollbarBounds.Size.Y : Math.Min((int)(ContentBounds.Size.Y / (float)_contentHeight * ScrollbarBounds.Size.Y), ContentBounds.Size.Y);

		UpdateScrollOffsetAndScrollbarPosition(NestingContext.ScrollOffset);
	}

	private void UpdateScrollOffsetAndScrollbarPosition(Vector2i<int> newScrollOffset)
	{
		// Update and clamp scroll offset.
		NestingContext.ScrollOffset = Vector2i<int>.Clamp(newScrollOffset, new(0, -_contentHeight + Bounds.Size.Y), default);

		// Update scrollbar position.
		ScrollbarStartY = (int)(NestingContext.ScrollOffset.Y / (float)-_contentHeight * ScrollbarBounds.Size.Y);
	}

	#region Update loop

	public override void Update(Vector2i<int> scrollOffset)
	{
		bool wasActive = MouseUiContext.IsActive;

		base.Update(scrollOffset);

		HandleScrollWheel(scrollOffset, wasActive);
		HandleScrollbar(scrollOffset);
	}

	private void HandleScrollWheel(Vector2i<int> scrollOffset, bool wasActive)
	{
		bool hoverWithoutBlock = wasActive && ContentBounds.Contains(MouseUiContext.MousePosition.RoundToVector2Int32() - scrollOffset);
		if (!hoverWithoutBlock)
			return;

		int scroll = Input.GetScroll();
		if (scroll != 0)
		{
			Vector2i<int> newOffset = new(0, NestingContext.ScrollOffset.Y + scroll * _scrollAmountInPixels);
			UpdateScrollOffsetAndScrollbarPosition(newOffset);
		}
	}

	private void HandleScrollbar(Vector2i<int> scrollOffset)
	{
		IsScrollbarHovering = MouseUiContext.Contains(scrollOffset, ScrollbarBounds);

		if (IsScrollbarHovering && Input.IsButtonPressed(MouseButton.Left))
		{
			int mousePos = TranslatedMousePosition(scrollOffset.Y);
			if (mousePos > ScrollbarStartY)
			{
				if (mousePos < ScrollbarStartY + ScrollbarHeight)
				{
					IsDraggingScrollbar = true;
					_holdStartMouseY = mousePos;
					_scrollbarStartYOld = ScrollbarStartY;
				}
				else
				{
					UpdateScrollOffsetAndScrollbarPosition(new(0, NestingContext.ScrollOffset.Y - _scrollAmountInPixels));
				}
			}
			else
			{
				UpdateScrollOffsetAndScrollbarPosition(new(0, NestingContext.ScrollOffset.Y + _scrollAmountInPixels));
			}
		}
		else if (IsDraggingScrollbar)
		{
			UpdateScrollOffsetFromDrag();
		}

		if (IsDraggingScrollbar && Input.IsButtonReleased(MouseButton.Left))
		{
			if (IsScrollbarHovering)
				UpdateScrollOffsetFromDrag();

			IsDraggingScrollbar = false;
			_scrollbarStartYOld = ScrollbarStartY;
		}

		void UpdateScrollOffsetFromDrag()
		{
			int mouseDifferenceWithStartDrag = TranslatedMousePosition(scrollOffset.Y) - _holdStartMouseY;
			float multiplier = _contentHeight / (float)ScrollbarBounds.Size.Y;
			UpdateScrollOffsetAndScrollbarPosition(new(0, (int)((-_scrollbarStartYOld - mouseDifferenceWithStartDrag) * multiplier)));
		}

		int TranslatedMousePosition(int scrollOffsetY)
			=> (int)MouseUiContext.MousePosition.Y - Bounds.Y1 - scrollOffsetY;
	}

	#endregion Update loop
}
