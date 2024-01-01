namespace DevilDaggersInfo.Tools.Engine.Content.Parsers.Texture;

internal static class BitUtils
{
	public static bool IsBitSet(byte b, int index)
	{
		return (b >> index & 1) != 0;
	}
}
