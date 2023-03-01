using DevilDaggersInfo.App.Ui.Base.Rendering.Scissors;
using DevilDaggersInfo.App.Ui.Base.Utils;
using Silk.NET.OpenGL;

namespace DevilDaggersInfo.App.Ui.Base.Rendering.Renderers;

public class RectangleRenderer
{
	private readonly uint _vaoRectangleTriangles = VertexArrayObjectUtils.CreateFromVertices(UiVertexBuilder.RectangleTriangles());
	private readonly List<Rectangle> _collection = new();

	public void Schedule(Vector2i<int> scale, Vector2i<int> center, float depth, Color color)
	{
		_collection.Add(new(scale, center, depth, color, ScissorScheduler.GetCalculatedScissor()));
	}

	public void Render()
	{
		if (_collection.Count == 0)
			return;

		Gl.BindVertexArray(_vaoRectangleTriangles);

		foreach (Rectangle rt in _collection)
		{
			ScissorActivator.SetScissor(rt.Scissor);

			Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(rt.Scale.X, rt.Scale.Y, 1);

			Shader.SetMatrix4x4(UiUniforms.Model, scaleMatrix * Matrix4x4.CreateTranslation(rt.CenterPosition.X, rt.CenterPosition.Y, rt.Depth));
			Shader.SetVector4(UiUniforms.Color, rt.Color);
			Gl.DrawArrays(PrimitiveType.Triangles, 0, 6);
		}

		Gl.BindVertexArray(0);

		_collection.Clear();
	}

	private readonly record struct Rectangle(Vector2i<int> Scale, Vector2i<int> CenterPosition, float Depth, Color Color, Scissor? Scissor);
}
