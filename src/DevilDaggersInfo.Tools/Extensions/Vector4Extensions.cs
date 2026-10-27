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
	}
}
