using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Ui.Base.Rendering.Scissors;
using DevilDaggersInfo.App.Ui.Base.Utils;
using Silk.NET.OpenGL;

namespace DevilDaggersInfo.App.Ui.Base.Rendering.Renderers;

public class EllipseRenderer
{
	private const int _circleSubdivisionCount = 40;

	private readonly uint _vaoCircleLineStrip = VertexArrayObjectUtils.CreateFromVertices(UiVertexBuilder.CircleLineStrip(_circleSubdivisionCount));
	private readonly List<EllipseLineStrip> _collection = new();

	public void Schedule(Vector2i<int> center, float radius, float depth, Color color)
	{
		_collection.Add(new(center, new(radius), depth, color, ScissorScheduler.GetCalculatedScissor()));
	}

	public void Schedule(Vector2i<int> center, Vector2 radius, float depth, Color color)
	{
		_collection.Add(new(center, radius, depth, color, ScissorScheduler.GetCalculatedScissor()));
	}

	public void Render()
	{
		if (_collection.Count == 0)
			return;

		Gl.BindVertexArray(_vaoCircleLineStrip);

		foreach (EllipseLineStrip els in _collection)
		{
			ScissorActivator.SetScissor(els.Scissor);

			Matrix4x4 scale = Matrix4x4.CreateScale(els.Radius.X, els.Radius.Y, 1);
			Matrix4x4 translation = Matrix4x4.CreateTranslation(els.CenterPosition.X, els.CenterPosition.Y, els.Depth);
			UiShader.SetModel(scale * translation);
			UiShader.SetColor(els.Color);
			Gl.DrawArrays(PrimitiveType.LineStrip, 0, _circleSubdivisionCount + 1);
		}

		Gl.BindVertexArray(0);

		_collection.Clear();
	}

	private readonly record struct EllipseLineStrip(Vector2i<int> CenterPosition, Vector2 Radius, float Depth, Color Color, Scissor? Scissor);
}
