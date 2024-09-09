namespace DevilDaggersInfo.Tools.Utils;

public static class DateTimeUtils
{
	// TODO: Implement ReadOnlySpan<char> variant.
	public static string FormatTimeAgo(DateTime? utcDateTime)
	{
		if (!utcDateTime.HasValue)
			return "Never";

		if (utcDateTime.Value.Kind != DateTimeKind.Utc)
			throw new ArgumentException($"DateTime must be in UTC. Kind was {utcDateTime.Value.Kind}.", nameof(utcDateTime));

		TimeSpan diff = DateTime.UtcNow - utcDateTime.Value;
		if (diff < TimeSpan.FromSeconds(5))
			return "just now";

		if (diff < TimeSpan.FromMinutes(1))
			return $"{diff.Seconds} second{S(diff.Seconds)} ago";

		if (diff < TimeSpan.FromHours(1))
			return $"{diff.Minutes} minute{S(diff.Minutes)} ago";

		if (diff < TimeSpan.FromDays(1))
			return $"{diff.Hours} hour{S(diff.Hours)} ago";

		return $"{diff.Days} day{S(diff.Days)} ago";

		static string S(int value)
		{
			return value == 1 ? string.Empty : "s";
		}
	}
}
