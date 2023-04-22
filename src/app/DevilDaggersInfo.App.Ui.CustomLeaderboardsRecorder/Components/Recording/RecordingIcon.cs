using DevilDaggersInfo.App.Engine.Content;
using DevilDaggersInfo.App.Engine.Extensions;
using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Engine.Ui;
using DevilDaggersInfo.App.Engine.Ui.Components;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;

namespace DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Components.Recording;

public class RecordingIcon : AbstractComponent
{
	private readonly Texture _texture;
	private readonly Color _color;

	private readonly Vector2 _size;

	public RecordingIcon(IBounds bounds, Texture texture, Color color)
		: base(bounds)
	{
		_texture = texture;
		_color = color;

		_size = new(_texture.Width, _texture.Height);
	}

	public override void Render(Vector2i<int> scrollOffset)
	{
		base.Render(scrollOffset);

		Root.Game.SpriteRenderer.Schedule(_size, (Bounds.Center + scrollOffset).ToVector2(), Depth, _texture, _color);
	}
}
