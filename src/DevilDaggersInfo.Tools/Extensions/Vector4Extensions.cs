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
	}
}
