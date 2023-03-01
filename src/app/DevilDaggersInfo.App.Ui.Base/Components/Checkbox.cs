using DevilDaggersInfo.App.Ui.Base.Components.Styles;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using Warp.NET.Ui;
using Warp.NET.Ui.Components;

namespace DevilDaggersInfo.App.Ui.Base.Components;

public class Checkbox : AbstractCheckbox
{
	public Checkbox(IBounds bounds, Action<bool> onClick, CheckboxStyle checkboxStyle)
		: base(bounds, onClick)
	{
		CheckboxStyle = checkboxStyle;
	}

	public CheckboxStyle CheckboxStyle { get; set; }

	public override void Render(Vector2i<int> scrollOffset)
	{
		base.Render(scrollOffset);

		Vector2i<int> marginVec = new(CheckboxStyle.Margin);
		Vector2i<int> borderVec = new(CheckboxStyle.BorderSize);
		Vector2i<int> borderTickVec = new(CheckboxStyle.TickPadding);
		Vector2i<int> fullScale = new(Bounds.X2 - Bounds.X1, Bounds.Y2 - Bounds.Y1);
		Vector2i<int> topLeft = new(Bounds.X1, Bounds.Y1);
		Vector2i<int> center = topLeft + fullScale / 2;
		Vector2i<int> scale = fullScale - marginVec;

		Root.Game.RectangleRenderer.Schedule(scale + borderVec, scrollOffset + center, Depth, Color.White);
		Root.Game.RectangleRenderer.Schedule(scale, scrollOffset + center, Depth + 1, Hover ? Color.Gray(0.25f) : Color.Black);

		if (CurrentValue)
			Root.Game.RectangleRenderer.Schedule(scale - borderTickVec, scrollOffset + center, Depth + 2, Color.Gray(0.75f));
	}
}
