namespace DevilDaggersInfo.Tools.Utils;

internal static class FileSizeUtils
{
	public static ReadOnlySpan<byte> Format(long fileSizeInBytes)
	{
		return fileSizeInBytes switch
		{
			>= 1_000_000_000 => Inline.Utf8($"{fileSizeInBytes / 1_000_000_000f:0.00} GB"),
			>= 1_000_000 => Inline.Utf8($"{fileSizeInBytes / 1_000_000f:0.00} MB"),
			>= 1000 => Inline.Utf8($"{fileSizeInBytes / 1000f:0.00} KB"),
			_ => Inline.Utf8($"{fileSizeInBytes} bytes"),
		};
	}
}
