using DevilDaggersInfo.App.Ui.Base.Rendering.Scissors;
using DevilDaggersInfo.App.Ui.Base.Utils;
using Silk.NET.OpenGL;

namespace DevilDaggersInfo.App.Ui.Base.Rendering.Renderers;

public class EllipseRenderer
{
	private const int _circleSubdivisionCount = 40;

	private readonly uint _vaoCircleLines = VertexArrayObjectUtils.CreateFromVertices(UiVertexBuilder.CircleLines(_circleSubdivisionCount));
	private readonly List<EllipseLines> _collection = new();

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

		Gl.BindVertexArray(_vaoCircleLines);

		foreach (EllipseLines el in _collection)
		{
			ScissorActivator.SetScissor(el.Scissor);

			Matrix4x4 scale = Matrix4x4.CreateScale(el.Radius.X, el.Radius.Y, 1);
			Matrix4x4 translation = Matrix4x4.CreateTranslation(el.CenterPosition.X, el.CenterPosition.Y, el.Depth);
			Shader.SetMatrix4x4(UiUniforms.Model, scale * translation);
			Shader.SetVector4(UiUniforms.Color, el.Color);
			Gl.DrawArrays(PrimitiveType.LineStrip, 0, _circleSubdivisionCount + 1);
		}

		Gl.BindVertexArray(0);

		_collection.Clear();
	}

	private readonly record struct EllipseLines(Vector2i<int> CenterPosition, Vector2 Radius, float Depth, Color Color, Scissor? Scissor);
}
