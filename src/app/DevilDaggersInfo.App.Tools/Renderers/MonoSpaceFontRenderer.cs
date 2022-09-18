using DevilDaggersInfo.App.Tools.Enums;
using Silk.NET.OpenGL;
using Warp.Text;

namespace DevilDaggersInfo.App.Tools.Renderers;

public class MonoSpaceFontRenderer
{
	private uint _vao;
	private MonoSpaceFont? _font;

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

	public void Render(Vector2i<int> scale, Vector2i<int> position, float depth, Color color, object? obj, TextAlign textAlign)
	{
		if (_font == null)
			throw new InvalidOperationException("Font is not set.");

		string text = obj?.ToString() ?? string.Empty;
		if (string.IsNullOrWhiteSpace(text))
			return;

		_font.Texture.Use();

		Gl.BindVertexArray(_vao);

		int charHeight = _font.Texture.Height;
		int charWidth = _font.Texture.Width / _font.CharAmount;
		int halfCharWidth = (int)Math.Ceiling(charWidth / 2f);
		int halfCharHeight = (int)Math.Ceiling(charHeight / 2f);
		int scaledCharWidth = scale.X * charWidth;
		int scaledCharHeight = scale.Y * charHeight;
		Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(scaledCharWidth, scaledCharHeight, 1);

		Vector2i<int> relativePosition = new(halfCharWidth, halfCharHeight);
		if (textAlign == TextAlign.Middle)
		{
			Vector2i<int> textSize = _font.MeasureText(text) * scale;
			relativePosition -= textSize / 2;
		}
		else if (textAlign == TextAlign.Right)
		{
			Vector2i<int> textSize = _font.MeasureText(text) * scale;
			relativePosition -= textSize with { Y = 0 };
		}

		foreach (char c in text)
		{
			Matrix4x4 translationMatrix = Matrix4x4.CreateTranslation(position.X + relativePosition.X, position.Y + relativePosition.Y, depth);
			Shaders.Font.SetMatrix4x4("model", scaleMatrix * translationMatrix);
			Shaders.Font.SetFloat("offset", _font.GetTextureOffset(c));
			Shaders.Font.SetVector4("color", color);
			Gl.DrawArrays(PrimitiveType.Triangles, 0, 6);

			_font.AdvancePosition(c, ref relativePosition, halfCharHeight, scaledCharWidth, scaledCharHeight);
		}

		Gl.BindVertexArray(0);
	}
}
