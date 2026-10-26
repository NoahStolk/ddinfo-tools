using System.Text.Unicode;

namespace DevilDaggersInfo.Tools.Utils;

/// <summary>
/// Formats a UTC date time relative to now (e.g. "5 minutes ago") directly to UTF-8.
/// Use this when the value needs to be part of an <see cref="Inline" /> interpolated string, otherwise use <see cref="DateTimeUtils.FormatTimeAgo" />.
/// </summary>
internal readonly record struct TimeAgo : IUtf8SpanFormattable
{
	private readonly DateTime? _utcDateTime;

	public TimeAgo(DateTime? utcDateTime)
	{
		if (utcDateTime.HasValue && utcDateTime.Value.Kind != DateTimeKind.Utc)
			throw new ArgumentException($"DateTime must be in UTC. Kind was {utcDateTime.Value.Kind}.", nameof(utcDateTime));

		_utcDateTime = utcDateTime;
	}

	public bool TryFormat(Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
	{
		if (!_utcDateTime.HasValue)
			return TryCopy("Never"u8, utf8Destination, out bytesWritten);

		TimeSpan diff = DateTime.UtcNow - _utcDateTime.Value;
		if (diff < TimeSpan.FromSeconds(5))
			return TryCopy("Just now"u8, utf8Destination, out bytesWritten);

		if (diff < TimeSpan.FromMinutes(1))
			return Utf8.TryWrite(utf8Destination, $"{diff.Seconds} second{Plural(diff.Seconds)} ago", out bytesWritten);

		if (diff < TimeSpan.FromHours(1))
			return Utf8.TryWrite(utf8Destination, $"{diff.Minutes} minute{Plural(diff.Minutes)} ago", out bytesWritten);

		if (diff < TimeSpan.FromDays(1))
			return Utf8.TryWrite(utf8Destination, $"{diff.Hours} hour{Plural(diff.Hours)} ago", out bytesWritten);

		return Utf8.TryWrite(utf8Destination, $"{diff.Days} day{Plural(diff.Days)} ago", out bytesWritten);
	}

	private static bool TryCopy(ReadOnlySpan<byte> value, Span<byte> utf8Destination, out int bytesWritten)
	{
		bool success = value.TryCopyTo(utf8Destination);
		bytesWritten = success ? value.Length : 0;
		return success;
	}

	private static ReadOnlySpan<byte> Plural(int value)
	{
		return value == 1 ? [] : "s"u8;
	}
}
