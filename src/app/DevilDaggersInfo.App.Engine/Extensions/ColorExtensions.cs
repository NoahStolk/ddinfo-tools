using DevilDaggersInfo.App.Engine.Maths.Numerics;

namespace DevilDaggersInfo.App.Engine.Extensions;

public static class ColorExtensions
{
	public static uint ToArgbUInt(this Color color)
	{
		return (uint)(color.A << 24 | color.B << 16 | color.G << 8 | color.R);
	}
}
