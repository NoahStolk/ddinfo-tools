using Silk.NET.OpenGL;

namespace DevilDaggersInfo.App.Tools.Renderers;

public class UiRenderer
{
	private readonly uint _vao;

	public unsafe UiRenderer()
	{
		_vao = Gl.GenVertexArray();
		Gl.BindVertexArray(_vao);

		uint vbo = Gl.GenBuffer();
		Gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);

		float[] vertices =
		{
			-0.5f, +0.5f, // top left
			+0.5f, +0.5f, // top right
			-0.5f, -0.5f, // bottom left

			+0.5f, +0.5f, // top right
			+0.5f, -0.5f, // bottom right
			-0.5f, -0.5f, // bottom left
		};
		fixed (float* v = &vertices[0])
			Gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)vertices.Length * sizeof(float), v, BufferUsageARB.StaticDraw);

		Gl.EnableVertexAttribArray(0);
		Gl.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), (void*)0);

		Gl.BindVertexArray(0);

		Gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
	}

	public void RenderTopLeft(Vector2i<int> scale, Vector2i<int> topLeft, float depth, Vector3 color)
		=> RenderCenter(scale, topLeft + scale / 2, depth, color);

	public void RenderCenter(Vector2i<int> scale, Vector2i<int> center, float depth, Vector3 color)
	{
		Gl.BindVertexArray(_vao);

		Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(scale.X, scale.Y, 1);

		Shaders.Ui.SetMatrix4x4("model", scaleMatrix * Matrix4x4.CreateTranslation(center.X, center.Y, depth));
		Shaders.Ui.SetVector3("color", color);
		Gl.DrawArrays(PrimitiveType.Triangles, 0, 6);

		Gl.BindVertexArray(0);
	}
}
