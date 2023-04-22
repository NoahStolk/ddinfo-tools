using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Engine.Text;
using DevilDaggersInfo.App.Engine.Ui;
using DevilDaggersInfo.App.Engine.Ui.Components;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.Rendering.Text;

namespace DevilDaggersInfo.App.Ui.ReplayEditor.Components;

public class InputVisualizer : AbstractComponent
{
	private readonly string _text;

	public InputVisualizer(IBounds bounds, string text)
		: base(bounds)
	{
		_text = text;
	}

	public bool IsEnabled { get; set; }

	public override void Render(Vector2i<int> scrollOffset)
	{
		base.Render(scrollOffset);

		Vector2i<int> textPosition = new Vector2i<int>(Bounds.X1 + Bounds.X2, Bounds.Y1 + Bounds.Y2) / 2;

		const int border = 4;
		Root.Game.RectangleRenderer.Schedule(Bounds.Size, scrollOffset + Bounds.Center, Depth, Color.Gray(0.6f));
		Root.Game.RectangleRenderer.Schedule(Bounds.Size - new Vector2i<int>(border * 2), scrollOffset + Bounds.Center, Depth + 1, IsEnabled ? Color.Red : Color.Black);

		Root.Game.GetFontRenderer(FontSize.H16).Schedule(Vector2i<int>.One, scrollOffset + textPosition, Depth + 2, Color.White, _text, TextAlign.Middle);
	}
}
