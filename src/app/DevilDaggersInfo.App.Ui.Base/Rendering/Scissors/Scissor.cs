using Warp.NET.Extensions;
using Warp.NET.Ui;

namespace DevilDaggersInfo.App.Ui.Base.Rendering.Scissors;

public readonly record struct Scissor(int X, int Y, uint Width, uint Height)
{
	public static Scissor Create(IBounds bounds, Vector2i<int> scrollOffset, Vector2 viewportOffset, Vector2 uiScale)
	{
		return Create(bounds.X1, bounds.Y1, bounds.Size.X, bounds.Size.Y, scrollOffset, viewportOffset, uiScale);
	}

	public static Scissor Create(int x, int y, int w, int h, Vector2i<int> scrollOffset, Vector2 viewportOffset, Vector2 uiScale)
	{
		Vector2i<int> topLeft = new(x, y);
		Vector2i<int> size = new(w, h);
		Vector2i<int> scaledSize = (size.ToVector2() * uiScale).RoundToVector2Int32();
		Vector2i<int> scaledTopLeft = (topLeft.ToVector2() * uiScale).RoundToVector2Int32();
		Vector2i<int> scaledScrollOffset = (scrollOffset.ToVector2() * uiScale).RoundToVector2Int32();
		return new(
			scaledTopLeft.X + (int)viewportOffset.X + scaledScrollOffset.X,
			CurrentWindowState.Height - (scaledSize.Y + scaledScrollOffset.Y) - (int)viewportOffset.Y - scaledTopLeft.Y,
			(uint)scaledSize.X,
			(uint)scaledSize.Y);
	}
}
