using DevilDaggersInfo.App.Ui.Base.Rendering.Scissors;
using DevilDaggersInfo.App.Ui.Base.Utils;
using Silk.NET.OpenGL;
using Warp.NET.Extensions;

namespace DevilDaggersInfo.App.Ui.Base.Rendering.Renderers;

public class LineRenderer
{
	private readonly uint _vaoLines = VertexArrayObjectUtils.CreateFromVertices(UiVertexBuilder.Line());
	private readonly List<Line> _collection = new();

	public void Schedule(Vector2i<int> start, Vector2i<int> end, float depth, Color color)
	{
		_collection.Add(new(start, end, depth, color, ScissorScheduler.GetCalculatedScissor()));
	}

	public void Render()
	{
		if (_collection.Count == 0)
			return;

		Gl.BindVertexArray(_vaoLines);

		foreach (Line l in _collection)
		{
			ScissorActivator.SetScissor(l.Scissor);

			float length = (l.Start - l.End).ToVector2().Length();
			float angle = MathF.Atan2(l.End.Y - l.Start.Y, l.End.X - l.Start.X);

			Matrix4x4 scale = Matrix4x4.CreateScale(length, 1, 1);
			Matrix4x4 rotation = Matrix4x4.CreateRotationZ(angle);
			Matrix4x4 translation = Matrix4x4.CreateTranslation(l.Start.X, l.Start.Y, l.Depth);
			Shader.SetMatrix4x4(UiUniforms.Model, scale * rotation * translation);
			Shader.SetVector4(UiUniforms.Color, l.Color);
			Gl.DrawArrays(PrimitiveType.LineStrip, 0, 2);
		}

		Gl.BindVertexArray(0);

		_collection.Clear();
	}

	private readonly record struct Line(Vector2i<int> Start, Vector2i<int> End, float Depth, Color Color, Scissor? Scissor);
}
