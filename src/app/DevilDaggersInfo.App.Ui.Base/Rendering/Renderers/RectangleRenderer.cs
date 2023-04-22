using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Ui.Base.Rendering.Scissors;
using DevilDaggersInfo.App.Ui.Base.Utils;
using Silk.NET.OpenGL;

namespace DevilDaggersInfo.App.Ui.Base.Rendering.Renderers;

public class RectangleRenderer
{
	private readonly uint _vaoRectangleTriangles = VertexArrayObjectUtils.CreateFromVertices(UiVertexBuilder.RectangleTriangles());
	private readonly uint _vaoRectangleLineLoop = VertexArrayObjectUtils.CreateFromVertices(UiVertexBuilder.RectangleLineLoop());

	private readonly List<RectangleTriangles> _collectionTriangles = new();
	private readonly List<RectangleLineLoop> _collectionLineLoop = new();

	public void Schedule(Vector2i<int> scale, Vector2i<int> center, float depth, Color color, bool filled = true)
	{
		if (filled)
			_collectionTriangles.Add(new(scale, center, depth, color, ScissorScheduler.GetCalculatedScissor()));
		else
			_collectionLineLoop.Add(new(scale, center, depth, color, ScissorScheduler.GetCalculatedScissor()));
	}

	public void Render()
	{
		RenderTriangles();
		RenderLineLoop();
	}

	private void RenderTriangles()
	{
		if (_collectionTriangles.Count == 0)
			return;

		Gl.BindVertexArray(_vaoRectangleTriangles);

		foreach (RectangleTriangles rt in _collectionTriangles)
		{
			ScissorActivator.SetScissor(rt.Scissor);

			Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(rt.Scale.X, rt.Scale.Y, 1);

			UiShader.SetModel(scaleMatrix * Matrix4x4.CreateTranslation(rt.CenterPosition.X, rt.CenterPosition.Y, rt.Depth));
			UiShader.SetColor(rt.Color);
			Gl.DrawArrays(PrimitiveType.Triangles, 0, 6);
		}

		Gl.BindVertexArray(0);

		_collectionTriangles.Clear();
	}

	private void RenderLineLoop()
	{
		if (_collectionLineLoop.Count == 0)
			return;

		Gl.BindVertexArray(_vaoRectangleLineLoop);

		foreach (RectangleLineLoop rll in _collectionLineLoop)
		{
			ScissorActivator.SetScissor(rll.Scissor);

			Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(rll.Scale.X, rll.Scale.Y, 1);

			UiShader.SetModel(scaleMatrix * Matrix4x4.CreateTranslation(rll.CenterPosition.X, rll.CenterPosition.Y, rll.Depth));
			UiShader.SetColor(rll.Color);
			Gl.DrawArrays(PrimitiveType.LineLoop, 0, 4);
		}

		Gl.BindVertexArray(0);

		_collectionLineLoop.Clear();
	}

	private readonly record struct RectangleTriangles(Vector2i<int> Scale, Vector2i<int> CenterPosition, float Depth, Color Color, Scissor? Scissor);

	private readonly record struct RectangleLineLoop(Vector2i<int> Scale, Vector2i<int> CenterPosition, float Depth, Color Color, Scissor? Scissor);
}
