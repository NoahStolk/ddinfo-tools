using DevilDaggersInfo.App.Engine.Content;
using DevilDaggersInfo.App.Engine.Maths.Numerics;
using System.Collections.Immutable;

namespace DevilDaggersInfo.App.Engine.Text;

public class MonoSpaceFont
{
	private readonly string _charset;
	private readonly char _lineBreak;

	public MonoSpaceFont(Texture texture, Charset charset, char lineBreak = '\n')
	{
		Texture = texture;
		_charset = charset.Characters;
		_lineBreak = lineBreak;

		CharAmount = charset.Characters.Length;
		CharWidth = Texture.Width / CharAmount;
		CharVertexWidth = 1.0f / CharAmount;

		Vertices = ImmutableArray.Create(stackalloc float[24]
		{
			-0.5f, +0.5f, 0.0f, 1.0f, // top left
			+0.5f, +0.5f, CharVertexWidth, 1.0f, // top right
			-0.5f, -0.5f, 0.0f, 0.0f, // bottom left

			+0.5f, +0.5f, CharVertexWidth, 1.0f, // top right
			+0.5f, -0.5f, CharVertexWidth, 0.0f, // bottom right
			-0.5f, -0.5f, 0.0f, 0.0f, // bottom left
		});
	}

	public Texture Texture { get; }
	public int CharAmount { get; }
	public int CharWidth { get; }
	public float CharVertexWidth { get; }

	public ImmutableArray<float> Vertices { get; }

	public float? GetTextureOffset(char c)
	{
		int index = _charset.IndexOf(c);
		if (index == -1)
			return null;

		return index / (float)CharAmount;
	}

	public Vector2i<int> MeasureText(string text)
	{
		int width = 0;
		int maxWidth = 0;
		int height = 1;
		foreach (char c in text)
		{
			if (c == _lineBreak)
			{
				width = 0;
				height++;
			}
			else
			{
				width++;
				if (maxWidth < width)
					maxWidth = width;
			}
		}

		return new Vector2i<int>(maxWidth, height) * new Vector2i<int>(CharWidth, Texture.Height);
	}

	public void AdvancePosition(char c, ref Vector2i<int> relativePosition, int xOrigin, int charWidth, int charHeight)
	{
		if (c == _lineBreak)
		{
			relativePosition.X = xOrigin;
			relativePosition.Y += charHeight;
		}
		else
		{
			relativePosition.X += charWidth;
		}
	}
}
