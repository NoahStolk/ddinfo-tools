using Warp.Ui;
using Warp.Ui.Components;

namespace DevilDaggersInfo.App.Tools.Components;

public class Scrollbar : AbstractScrollbar
{
	public Scrollbar(Rectangle metric, Action<float> onChange)
		: base(metric, onChange)
	{
	}

	public override void Render(Vector2i<int> parentPosition)
	{
		base.Render(parentPosition);

		const int border = 4;

		Vector2i<int> borderVec = new(border);
		Vector2i<int> scale = Metric.Size;
		Vector2i<int> topLeft = new(Metric.X1, Metric.Y1);

		Color thumbColor = Color.Gray(0.75f);
		Base.Game.UiRenderer.RenderTopLeft(scale, parentPosition + topLeft, Depth, thumbColor);
		Base.Game.UiRenderer.RenderTopLeft(scale - borderVec, parentPosition + topLeft + new Vector2i<int>(border / 2), Depth + 1, Hold ? new(0.5f) : Hover ? new(0.25f) : Vector3.Zero);

		const int thumbPadding = 4;
		Vector2i<int> thumbScale = new(scale.X - thumbPadding, (int)MathF.Round(scale.Y * ThumbPercentageSize));
		float percentageForRendering = Math.Clamp(TopPercentage, 0, 1 - ThumbPercentageSize);
		Base.Game.UiRenderer.RenderTopLeft(thumbScale, parentPosition + topLeft + new Vector2i<int>(thumbPadding / 2, (int)MathF.Round(percentageForRendering * scale.Y)), Depth + 2, thumbColor);
	}
}
