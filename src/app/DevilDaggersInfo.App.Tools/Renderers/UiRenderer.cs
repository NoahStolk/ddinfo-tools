using DevilDaggersInfo.App.Tools.Utils;
using DevilDaggersInfo.App.Ui.Base;
using DevilDaggersInfo.App.Ui.Base.Rendering;
using DevilDaggersInfo.App.Ui.Base.Rendering.Data;
using Silk.NET.OpenGL;

namespace DevilDaggersInfo.App.Tools.Renderers;

public class UiRenderer
{
	private const int _circleSubdivisionCount = 40;

	private readonly uint _vaoRectangleTriangles;
	private readonly uint _vaoCircleLines;

	public unsafe UiRenderer()
	{
		_vaoRectangleTriangles = CreateVao(VertexBuilder.RectangleTriangles());
		_vaoCircleLines = CreateVao(VertexBuilder.CircleLines(_circleSubdivisionCount));

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

	public void RenderRectangleTriangles()
	{
		if (RenderBatchCollector.RectangleTriangles.Count == 0)
			return;

		Gl.BindVertexArray(_vaoRectangleTriangles);

		foreach (RectangleTriangle rt in RenderBatchCollector.RectangleTriangles)
		{
			ScissorRenderUtils.ActivateScissor(rt.Scissor);

			Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(rt.Scale.X, rt.Scale.Y, 1);

			Shaders.Ui.SetMatrix4x4("model", scaleMatrix * Matrix4x4.CreateTranslation(rt.CenterPosition.X, rt.CenterPosition.Y, rt.Depth));
			Shaders.Ui.SetVector4("color", rt.Color);
			Gl.DrawArrays(PrimitiveType.Triangles, 0, 6);
		}

		Gl.BindVertexArray(0);
	}

	public void RenderCircleLines()
	{
		if (RenderBatchCollector.CircleLines.Count == 0)
			return;

		Gl.BindVertexArray(_vaoCircleLines);

		foreach (CircleLine cl in RenderBatchCollector.CircleLines)
		{
			ScissorRenderUtils.ActivateScissor(cl.Scissor);

			Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(cl.Radius, cl.Radius, 1);

			Shaders.Ui.SetMatrix4x4("model", scaleMatrix * Matrix4x4.CreateTranslation(cl.CenterPosition.X, cl.CenterPosition.Y, cl.Depth));
			Shaders.Ui.SetVector4("color", cl.Color);
			Gl.DrawArrays(PrimitiveType.LineStrip, 0, _circleSubdivisionCount + 1);
		}

		Gl.BindVertexArray(0);
	}
}
