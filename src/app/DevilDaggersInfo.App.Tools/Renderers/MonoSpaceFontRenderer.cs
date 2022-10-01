using DevilDaggersInfo.App.Ui.Base.Enums;
using DevilDaggersInfo.App.Ui.Base.Rendering.Data;
using Silk.NET.OpenGL;
using Warp.Text;

namespace DevilDaggersInfo.App.Tools.Renderers;

public class MonoSpaceFontRenderer
{
	private uint _vao;
	private MonoSpaceFont? _font;

	// TODO: Ctor and non-nullable _font.
	public unsafe void SetFont(MonoSpaceFont font)
	{
		_font = font;

		_vao = Gl.GenVertexArray();
		Gl.BindVertexArray(_vao);

		uint vbo = Gl.GenBuffer();
		Gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);

		fixed (float* v = &_font.Vertices.ToArray()[0])
			Gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)font.Vertices.Length * sizeof(float), v, BufferUsageARB.StaticDraw);

		Gl.EnableVertexAttribArray(0);
		Gl.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), (void*)0);

		Gl.EnableVertexAttribArray(1);
		Gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), (void*)(2 * sizeof(float)));

		Gl.BindVertexArray(0);

		Gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
	}

	public void Render(List<MonoSpaceText> texts)
	{
		if (texts.Count == 0)
			return;

		if (_font == null)
			throw new InvalidOperationException("Font is not set.");

		_font.Texture.Use();

		Gl.BindVertexArray(_vao);

		foreach (MonoSpaceText mst in texts)
		{
			int charHeight = _font.Texture.Height;
			int charWidth = _font.Texture.Width / _font.CharAmount;
			int scaledCharWidth = mst.Scale.X * charWidth;
			int scaledCharHeight = mst.Scale.Y * charHeight;
			Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(scaledCharWidth, scaledCharHeight, 1);

			Vector2i<int> relativePosition = new(scaledCharWidth / 2, scaledCharHeight / 2);
			if (mst.TextAlign == TextAlign.Middle)
			{
				Vector2i<int> textSize = _font.MeasureText(mst.Text) * mst.Scale;
				relativePosition -= textSize / 2;
			}
			else if (mst.TextAlign == TextAlign.Right)
			{
				Vector2i<int> textSize = _font.MeasureText(mst.Text) * mst.Scale;
				relativePosition -= textSize with { Y = 0 };
			}

			int originX = relativePosition.X;

			foreach (char c in mst.Text)
			{
				Matrix4x4 translationMatrix = Matrix4x4.CreateTranslation(mst.Position.X + relativePosition.X, mst.Position.Y + relativePosition.Y, mst.Depth);
				Shaders.Font.SetMatrix4x4("model", scaleMatrix * translationMatrix);
				Shaders.Font.SetFloat("offset", _font.GetTextureOffset(c));
				Shaders.Font.SetVector4("color", mst.Color);
				Gl.DrawArrays(PrimitiveType.Triangles, 0, 6);

				_font.AdvancePosition(c, ref relativePosition, originX, scaledCharWidth, scaledCharHeight);
			}
		}

		Gl.BindVertexArray(0);
	}
}
