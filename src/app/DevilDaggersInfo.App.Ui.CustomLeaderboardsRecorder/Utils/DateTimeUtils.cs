using DevilDaggersInfo.Common;

namespace DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Utils;

public static class DateTimeUtils
{
	public static string FormatTimeAgo(DateTime? dateTime)
	{
		if (!dateTime.HasValue)
			return "Never";

		TimeSpan diff = DateTime.Now - dateTime.Value;
		if (diff < TimeSpan.FromSeconds(5))
			return "Just now";

		int seconds = diff.Seconds;
		if (diff < TimeSpan.FromMinutes(1))
			return $"{seconds} second{S(seconds)} ago";

		int minutes = diff.Minutes;
		if (diff < TimeSpan.FromHours(1))
			return $"{minutes} minute{S(minutes)} ago";

		int hours = diff.Hours;
		if (diff < TimeSpan.FromDays(1))
			return $"{hours} hour{S(hours)} and {minutes} minute{S(minutes)} ago";

		return $"{dateTime.Value.ToString(StringFormats.DateTimeFormat)}";

		static string S(int value)
			=> value == 1 ? string.Empty : "s";
	}
}
