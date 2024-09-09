using System.Numerics;

namespace DevilDaggersInfo.Tools.Engine.Content.Extensions;

internal static class BinaryReaderExtensions
{
	public static Vector2 ReadVector2AsHalfPrecision(this BinaryReader br)
	{
		return new Vector2((float)br.ReadHalf(), (float)br.ReadHalf());
	}

	public static Vector3 ReadVector3AsHalfPrecision(this BinaryReader br)
	{
		return new Vector3((float)br.ReadHalf(), (float)br.ReadHalf(), (float)br.ReadHalf());
	}
}
