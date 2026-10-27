using System.Numerics;

namespace DevilDaggersInfo.Tools.Extensions;

internal static class Vector4Extensions
{
	extension(Vector4)
	{
		public static Vector4 InvertColor(Vector4 color)
		{
			return new Vector4(
				x: 1 - color.X,
				y: 1 - color.Y,
				z: 1 - color.Z,
				w: color.W);
		}

		public static Vector4 ReadableColorForBrightness(Vector4 color)
		{
			float r = color.X * 255f;
			float g = color.Y * 255f;
			float b = color.Z * 255f;

			int perceivedBrightness = (int)Math.Sqrt(r * r * 0.299 + g * g * 0.587 + b * b * 0.114);
			return perceivedBrightness < 140 ? Vector4.One : new Vector4(0, 0, 0, 1);
		}

		public static Vector4 DesaturateColor(Vector4 color, float amount)
		{
			float r = color.X;
			float g = color.Y;
			float b = color.Z;

			float l = 0.3f * r + 0.6f * g + 0.1f * b;
			float newR = r + amount * (l - r);
			float newG = g + amount * (l - g);
			float newB = b + amount * (l - b);

			return new Vector4(newR, newG, newB, color.W);
		}

		public static Vector4 CreateColorFromHsv(float hue, float saturation, float value)
		{
			saturation = Math.Clamp(saturation, 0, 1);
			value = Math.Clamp(value, 0, 1);

			int hi = (int)MathF.Floor(hue / 60) % 6;
			float f = hue / 60 - MathF.Floor(hue / 60);

			float v = value;
			float p = value * (1 - saturation);
			float q = value * (1 - f * saturation);
			float t = value * (1 - (1 - f) * saturation);

			return hi switch
			{
				0 => new Vector4(v, t, p, 1),
				1 => new Vector4(q, v, p, 1),
				2 => new Vector4(p, v, t, 1),
				3 => new Vector4(p, q, v, 1),
				4 => new Vector4(t, p, v, 1),
				_ => new Vector4(v, p, q, 1),
			};
		}
	}
}
