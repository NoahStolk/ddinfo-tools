using DevilDaggersInfo.App.Tools.Enums;
using Warp.Ui;
using Warp.Ui.Components;

namespace DevilDaggersInfo.App.Tools.Components;

public class Slider : AbstractSlider
{
	private readonly Color _textColor;

	public Slider(Rectangle metric, Action<float> onChange, bool applyInstantly, float min, float max, float step, float defaultValue, Color textColor)
		: base(metric, onChange, applyInstantly, min, max, step, defaultValue)
	{
		_textColor = textColor;
	}

	public override void Render(Vector2i<int> parentPosition)
	{
		base.Render(parentPosition);

		const int margin = 8;
		const int border = 4;

		Vector2i<int> marginVec = new(margin);
		Vector2i<int> borderVec = new(border);
		Vector2i<int> fullScale = new(Metric.X2 - Metric.X1, Metric.Y2 - Metric.Y1);
		Vector2i<int> topLeft = new(Metric.X1, Metric.Y1);
		Vector2i<int> center = topLeft + fullScale / 2;
		Vector2i<int> scale = fullScale - marginVec;

		Base.Game.UiRenderer.RenderCenter(scale + borderVec, parentPosition + center, Depth, Vector3.One);
		Base.Game.UiRenderer.RenderCenter(scale, parentPosition + center, Depth + 1, Hold ? new(0.5f) : Hover ? new(0.25f) : Vector3.Zero);
	}

	public override void RenderText(Vector2i<int> parentPosition)
	{
		base.RenderText(parentPosition);

		Vector2i<int> centerPosition = new Vector2i<int>(Metric.X1 + Metric.X2, Metric.Y1 + Metric.Y2) / 2;
		Base.Game.MonoSpaceFontRenderer.Render(Vector2i<int>.One, parentPosition + centerPosition, Depth + 2, _textColor, CurrentValue.ToString("0.00"), TextAlign.Middle);
	}
}
