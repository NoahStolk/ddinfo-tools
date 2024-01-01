using System.Collections;

namespace DevilDaggersInfo.Tools.Engine.Content.Extensions;

internal static class BitArrayExtensions
{
	public static byte[] ToBytes(this BitArray bitArray)
	{
		byte[] bytes = new byte[(bitArray.Length - 1) / 8 + 1];
		bitArray.CopyTo(bytes, 0);
		return bytes;
	}
}
