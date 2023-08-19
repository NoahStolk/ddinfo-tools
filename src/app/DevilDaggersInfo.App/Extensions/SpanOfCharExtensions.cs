namespace DevilDaggersInfo.App.Extensions;

public static class SpanOfCharExtensions
{
	public static Span<char> SliceUntilNull(this Span<char> span, int maxLength)
	{
		int indexOfNull = span.IndexOf('\0');
		int textLength = indexOfNull == -1 ? maxLength : indexOfNull;
		return span[..textLength];
	}
}
