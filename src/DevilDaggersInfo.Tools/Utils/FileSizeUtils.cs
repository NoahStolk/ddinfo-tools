namespace DevilDaggersInfo.Tools.Utils;

public static class FileSizeUtils
{
	public static ReadOnlySpan<char> Format(long fileSizeInBytes)
	{
		return fileSizeInBytes switch
		{
			>= 1_000_000_000 => Inline.Span($"{fileSizeInBytes / 1_000_000_000f:0.00} GB"),
			>= 1_000_000 => Inline.Span($"{fileSizeInBytes / 1_000_000f:0.00} MB"),
			>= 1000 => Inline.Span($"{fileSizeInBytes / 1000f:0.00} KB"),
			_ => Inline.Span($"{fileSizeInBytes} bytes"),
		};
	}
}
