namespace DevilDaggersInfo.Tools.Utils;

public static class FileSizeUtils
{
	public static ReadOnlySpan<char> Format(long fileSizeInBytes)
	{
		if (fileSizeInBytes >= 1_000_000_000)
			return Inline.Span($"{fileSizeInBytes / 1_000_000_000f:0.00} GB");

		if (fileSizeInBytes >= 1_000_000)
			return Inline.Span($"{fileSizeInBytes / 1_000_000f:0.00} MB");

		if (fileSizeInBytes >= 1000)
			return Inline.Span($"{fileSizeInBytes / 1000f:0.00} KB");

		return Inline.Span($"{fileSizeInBytes} bytes");
	}
}
