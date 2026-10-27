using System.Numerics;

namespace DevilDaggersInfo.Tools.Engine.Maths.Numerics;

[Obsolete("Use Vector4 instead.")]
public readonly record struct Color(byte R, byte G, byte B, byte A)
{
	public static implicit operator Vector4(Color color)
	{
		return new Vector4(color.R / (float)byte.MaxValue, color.G / (float)byte.MaxValue, color.B / (float)byte.MaxValue, color.A / (float)byte.MaxValue);
	}

	public Vector4 ReadableColorForBrightness()
	{
		return GetPerceivedBrightness() < 140 ? Vector4.One : new Vector4(0, 0, 0, 1);
	}

	private int GetPerceivedBrightness()
	{
		return (int)Math.Sqrt(R * R * 0.299 + G * G * 0.587 + B * B * 0.114);
	}

	public Color Desaturate(float f)
	{
		float r = R / (float)byte.MaxValue;
		float g = G / (float)byte.MaxValue;
		float b = B / (float)byte.MaxValue;

		float l = 0.3f * r + 0.6f * g + 0.1f * b;
		float newR = r + f * (l - r);
		float newG = g + f * (l - g);
		float newB = b + f * (l - b);

		return new Color((byte)(newR * byte.MaxValue), (byte)(newG * byte.MaxValue), (byte)(newB * byte.MaxValue), A);
	}

	public Color Darken(float amount)
	{
		return new Color((byte)(R * (1 - amount)), (byte)(G * (1 - amount)), (byte)(B * (1 - amount)), A);
	}

	public static Color FromHsv(float hue, float saturation, float value)
	{
		saturation = Math.Clamp(saturation, 0, 1);
		value = Math.Clamp(value, 0, 1);

		int hi = (int)MathF.Floor(hue / 60) % 6;
		float f = hue / 60 - MathF.Floor(hue / 60);

		value *= byte.MaxValue;
		byte v = (byte)value;
		byte p = (byte)(value * (1 - saturation));
		byte q = (byte)(value * (1 - f * saturation));
		byte t = (byte)(value * (1 - (1 - f) * saturation));

		return hi switch
		{
			0 => new Color(v, t, p, byte.MaxValue),
			1 => new Color(q, v, p, byte.MaxValue),
			2 => new Color(p, v, t, byte.MaxValue),
			3 => new Color(p, q, v, byte.MaxValue),
			4 => new Color(t, p, v, byte.MaxValue),
			_ => new Color(v, p, q, byte.MaxValue),
		};
	}
}
