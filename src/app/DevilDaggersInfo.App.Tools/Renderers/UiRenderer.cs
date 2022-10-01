using DevilDaggersInfo.App.Ui.Base;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern.Inversion;
using Silk.NET.OpenGL;

namespace DevilDaggersInfo.App.Tools.Renderers;

public class UiRenderer : IUiRenderer
{
	private const int _circleSubdivisionCount = 40;

	private readonly uint _vaoRectangleTriangles;
	private readonly uint _vaoCircleLines;
	private readonly uint _vaoRectangleLines;

	public unsafe UiRenderer()
	{
		_vaoRectangleTriangles = CreateVao(VertexBuilder.RectangleTriangles());
		_vaoCircleLines = CreateVao(VertexBuilder.CircleLines(_circleSubdivisionCount));
		_vaoRectangleLines = CreateVao(VertexBuilder.RectangleLines());

		uint CreateVao(float[] vertices)
		{
			uint vao = Gl.GenVertexArray();
			Gl.BindVertexArray(vao);

			uint vbo = Gl.GenBuffer();
			Gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);

			fixed (float* v = &vertices[0])
				Gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)vertices.Length * sizeof(float), v, BufferUsageARB.StaticDraw);

			Gl.EnableVertexAttribArray(0);
			Gl.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), (void*)0);

			Gl.BindVertexArray(0);

			Gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);

			return vao;
		}
	}

	public void RenderRectangleTopLeft(Vector2i<int> scale, Vector2i<int> topLeft, float depth, Vector3 color)
		=> RenderRectangleCenter(scale, topLeft + scale / 2, depth, color);

	public void RenderRectangleCenter(Vector2i<int> scale, Vector2i<int> center, float depth, Vector3 color)
	{
		Gl.BindVertexArray(_vaoRectangleTriangles);

		Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(scale.X, scale.Y, 1);

		Shaders.Ui.SetMatrix4x4("model", scaleMatrix * Matrix4x4.CreateTranslation(center.X, center.Y, depth));
		Shaders.Ui.SetVector3("color", color);
		Gl.DrawArrays(PrimitiveType.Triangles, 0, 6); // TODO: TriangleStrip? Or maybe not because this probably won't work for batching.

		Gl.BindVertexArray(0);
	}

	public void RenderCircleCenter(Vector2i<int> center, float radius, float depth, Vector3 color)
	{
		Gl.BindVertexArray(_vaoCircleLines);

		Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(radius, radius, 1);

		Shaders.Ui.SetMatrix4x4("model", scaleMatrix * Matrix4x4.CreateTranslation(center.X, center.Y, depth));
		Shaders.Ui.SetVector3("color", color);
		Gl.DrawArrays(PrimitiveType.LineStrip, 0, _circleSubdivisionCount + 1);

		Gl.BindVertexArray(0);
	}

	public void RenderHollowRectangleCenter(Vector2 scale, Vector2 center, float depth, Vector3 color)
	{
		Gl.BindVertexArray(_vaoRectangleLines);

		Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(scale.X, scale.Y, 1);

		Shaders.Ui.SetMatrix4x4("model", scaleMatrix * Matrix4x4.CreateTranslation(center.X, center.Y, depth));
		Shaders.Ui.SetVector3("color", color);
		Gl.DrawArrays(PrimitiveType.LineStrip, 0, 5);

		Gl.BindVertexArray(0);
	}
}
