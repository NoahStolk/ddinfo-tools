namespace DevilDaggersInfo.Tools.Utils;

internal static class DateTimeUtils
{
	/// <summary>
	/// Formats a UTC date time relative to now into the <see cref="Inline" /> buffer.
	/// To include the value in another <see cref="Inline" /> interpolated string, interpolate a <see cref="TimeAgo" /> instead.
	/// </summary>
	public static ReadOnlySpan<byte> FormatTimeAgo(DateTime? utcDateTime)
	{
		return Inline.Utf8(new TimeAgo(utcDateTime));
	}
}
