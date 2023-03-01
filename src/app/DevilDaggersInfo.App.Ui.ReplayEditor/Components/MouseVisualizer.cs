using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using Warp.NET.Extensions;
using Warp.NET.Ui;
using Warp.NET.Ui.Components;
using Warp.NET.Utils;

namespace DevilDaggersInfo.App.Ui.ReplayEditor.Components;

public class MouseVisualizer : AbstractComponent
{
	public MouseVisualizer(IBounds bounds)
		: base(bounds)
	{
	}

	public short MouseX { get; set; }
	public short MouseY { get; set; }

	public override void Render(Vector2i<int> scrollOffset)
	{
		base.Render(scrollOffset);

		const int border = 4;
		const int pointerSize = 4;
		const float pointerScale = 0.25f;
		Root.Game.RectangleRenderer.Schedule(Bounds.Size, scrollOffset + Bounds.Center, Depth, Color.Gray(0.6f));
		Root.Game.RectangleRenderer.Schedule(Bounds.Size - new Vector2i<int>(border * 2), scrollOffset + Bounds.Center, Depth + 1, Color.Black);

		float max = Bounds.Size.X / 2f - border - pointerScale / 2;
		Root.Game.RectangleRenderer.Schedule(new(pointerSize), scrollOffset + Bounds.Center + VectorUtils.Clamp(new Vector2(MouseX, MouseY) * pointerScale, -max, max).RoundToVector2Int32(), Depth + 2, Color.White);
	}
}
