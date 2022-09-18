using Warp.Ui;
using Warp.Ui.Components;

namespace DevilDaggersInfo.App.Tools.Components;

public class Checkbox : AbstractCheckbox
{
	public Checkbox(Layout parent, Rectangle metric, Action onClick)
		: base(metric, onClick)
	{
	}

	public override void Render(Vector2i<int> parentPosition)
	{
		base.Render(parentPosition);

		const int margin = 8;
		const int border = 4;
		const int borderTick = 16;

		Vector2i<int> marginVec = new(margin);
		Vector2i<int> borderVec = new(border);
		Vector2i<int> borderTickVec = new(borderTick);
		Vector2i<int> fullScale = new(Metric.X2 - Metric.X1, Metric.Y2 - Metric.Y1);
		Vector2i<int> topLeft = new(Metric.X1, Metric.Y1);
		Vector2i<int> center = topLeft + fullScale / 2;
		Vector2i<int> scale = fullScale - marginVec;

		Base.Game.UiRenderer.RenderCenter(scale + borderVec, parentPosition + center, Depth, Vector3.One);
		Base.Game.UiRenderer.RenderCenter(scale, parentPosition + center, Depth + 1, Hover ? new(0.25f) : Vector3.Zero);

		if (CurrentValue)
			Base.Game.UiRenderer.RenderCenter(scale - borderTickVec, parentPosition + center, Depth + 2, new(0.75f));
	}
}
