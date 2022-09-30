using DevilDaggersInfo.App.Ui.Base.DependencyPattern.Inversion;
using Silk.NET.OpenGL;

namespace DevilDaggersInfo.App.Tools.Renderers;

public class UiRenderer : IUiRenderer
{
	private const int _circleSubdivisionCount = 40;

	private readonly uint _vaoRectangle;
	private readonly uint _vaoCircle;

	public unsafe UiRenderer()
	{
		float[] rectangle =
		{
			-0.5f, +0.5f, // top left
			+0.5f, +0.5f, // top right
			-0.5f, -0.5f, // bottom left

			+0.5f, +0.5f, // top right
			+0.5f, -0.5f, // bottom right
			-0.5f, -0.5f, // bottom left
		};
		_vaoRectangle = CreateVao(rectangle);
		_vaoCircle = CreateVao(Circle());

		float[] Circle()
		{
			float[] vertices = new float[(_circleSubdivisionCount + 2) * 2];
			for (uint i = 0; i <= _circleSubdivisionCount; i++)
			{
				float angle = i * (MathF.PI * 2) / _circleSubdivisionCount;
				vertices[i * 2] = MathF.Cos(angle);
				vertices[i * 2 + 1] = MathF.Sin(angle);
			}

			return vertices;
		}

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

	public void RenderTopLeft(Vector2i<int> scale, Vector2i<int> topLeft, float depth, Vector3 color)
		=> RenderCenter(scale, topLeft + scale / 2, depth, color);

	public void RenderCenter(Vector2i<int> scale, Vector2i<int> center, float depth, Vector3 color)
	{
		Gl.BindVertexArray(_vaoRectangle);

		Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(scale.X, scale.Y, 1);

		Shaders.Ui.SetMatrix4x4("model", scaleMatrix * Matrix4x4.CreateTranslation(center.X, center.Y, depth));
		Shaders.Ui.SetVector3("color", color);
		Gl.DrawArrays(PrimitiveType.Triangles, 0, 6);

		Gl.BindVertexArray(0);
	}

	public void RenderCircle(Vector2i<int> center, int radius, float depth, Vector3 color)
	{
		Gl.BindVertexArray(_vaoCircle);

		Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(radius, radius, 1);

		Shaders.Ui.SetMatrix4x4("model", scaleMatrix * Matrix4x4.CreateTranslation(center.X, center.Y, depth));
		Shaders.Ui.SetVector3("color", color);
		Gl.DrawArrays(PrimitiveType.LineStrip, 0, _circleSubdivisionCount + 1);

		Gl.BindVertexArray(0);
	}
}
