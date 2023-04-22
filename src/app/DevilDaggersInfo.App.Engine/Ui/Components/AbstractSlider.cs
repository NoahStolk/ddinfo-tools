using DevilDaggersInfo.App.Engine.Maths.Numerics;
using Silk.NET.GLFW;

namespace DevilDaggersInfo.App.Engine.Ui.Components;

public abstract class AbstractSlider : AbstractComponent
{
	private readonly Action<float> _onChange;
	private readonly bool _applyInstantly;

	protected AbstractSlider(IBounds bounds, Action<float> onChange, bool applyInstantly, float min, float max, float step, float defaultValue)
		: base(bounds)
	{
		_onChange = onChange;
		_applyInstantly = applyInstantly;
		Min = min;
		Max = max;
		Step = step;

		CurrentValue = defaultValue;
	}

	public float Min { get; set; }
	public float Max { get; set; }
	public float Step { get; set; }

	public float CurrentValue { get; set; }

	protected bool Hold { get; private set; }

	protected bool Hover { get; private set; }

	public override void Update(Vector2i<int> scrollOffset)
	{
		base.Update(scrollOffset);

		Hover = MouseUiContext.Contains(scrollOffset, Bounds);

		if (Hover && Input.IsButtonPressed(MouseButton.Left))
		{
			Hold = true;
		}
		else if (Hold)
		{
			UpdateValue();

			if (_applyInstantly)
				_onChange(CurrentValue);
		}

		if (Hold && Input.IsButtonReleased(MouseButton.Left))
		{
			if (Hover)
				UpdateValue();

			_onChange(CurrentValue);
			Hold = false;
		}

		void UpdateValue()
		{
			float percentage = (MouseUiContext.MousePosition.X - scrollOffset.X - Bounds.X1) / (Bounds.X2 - Bounds.X1);
			float realValue = Math.Clamp(percentage * (Max - Min) + Min, Min, Max);
			CurrentValue = MathF.Round(realValue / Step) * Step;
		}
	}
}
