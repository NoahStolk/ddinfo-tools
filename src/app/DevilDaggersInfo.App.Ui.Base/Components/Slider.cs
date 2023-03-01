using DevilDaggersInfo.App.Ui.Base.Components.Styles;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using Warp.NET.Ui;
using Warp.NET.Ui.Components;

namespace DevilDaggersInfo.App.Ui.Base.Components;

public class Slider : AbstractSlider
{
	public Slider(IBounds bounds, Action<float> onChange, bool applyInstantly, float min, float max, float step, float defaultValue, SliderStyle sliderStyle)
		: base(bounds, onChange, applyInstantly, min, max, step, defaultValue)
	{
		SliderStyle = sliderStyle;
	}

	public SliderStyle SliderStyle { get; set; }

	protected void RenderHighlighter(Vector2i<int> scrollOffset, float percentage, Color color)
	{
		const int width = 4;
		int height = Bounds.Size.Y - SliderStyle.BorderSize;
		int position = (int)(percentage * (Bounds.Size.X - SliderStyle.BorderSize * 2 - width / 2));
		Vector2i<int> origin = scrollOffset + Bounds.TopLeft;
		Root.Game.RectangleRenderer.Schedule(new(width, height), origin + new Vector2i<int>(position + SliderStyle.BorderSize / 2, SliderStyle.BorderSize / 2) + new Vector2i<int>(width, height) / 2, Depth + 2, color);
	}

	protected float GetPercentage(float value)
	{
		return (value - Min) / (Max - Min);
	}

	protected virtual void RenderHighlighters(Vector2i<int> scrollOffset)
	{
		RenderHighlighter(scrollOffset, GetPercentage(CurrentValue), Color.Gray(0.75f));
	}

	public override void Render(Vector2i<int> scrollOffset)
	{
		base.Render(scrollOffset);

		Vector2i<int> borderVec = new(SliderStyle.BorderSize);

		Root.Game.RectangleRenderer.Schedule(Bounds.Size, scrollOffset + Bounds.Center, Depth, Color.White);
		Root.Game.RectangleRenderer.Schedule(Bounds.Size - borderVec, scrollOffset + Bounds.Center, Depth + 1, Hold ? Color.Gray(0.5f) : Hover ? Color.Gray(0.25f) : Color.Black);

		Root.Game.GetFontRenderer(SliderStyle.FontSize).Schedule(Vector2i<int>.One, scrollOffset + Bounds.Center, Depth + 3, SliderStyle.TextColor, CurrentValue.ToString(SliderStyle.ValueFormat), SliderStyle.TextAlign);

		RenderHighlighters(scrollOffset);
	}
}
