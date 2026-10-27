using System.Numerics;

namespace DevilDaggersInfo.Tools.Engine.Maths.Numerics;

[Obsolete("Use Vector4 instead.")]
public readonly record struct Color(byte R, byte G, byte B, byte A)
{
	public static implicit operator Vector4(Color color)
	{
		return new Vector4(color.R / (float)byte.MaxValue, color.G / (float)byte.MaxValue, color.B / (float)byte.MaxValue, color.A / (float)byte.MaxValue);
	}
}
