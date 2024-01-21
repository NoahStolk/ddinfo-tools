using DevilDaggersInfo.Core.Common;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor;

public static class DateTimeUtils
{
	public static string FormatTimeAgo(DateTime utcDateTime)
	{
		TimeSpan diff = DateTime.UtcNow - utcDateTime;
		if (diff < TimeSpan.FromSeconds(5))
			return "just now";

		int seconds = diff.Seconds;
		if (diff < TimeSpan.FromMinutes(1))
			return $"{seconds} second{S(seconds)} ago";

		int minutes = diff.Minutes;
		if (diff < TimeSpan.FromHours(1))
			return $"{minutes} minute{S(minutes)} ago";

		int hours = diff.Hours;
		if (diff < TimeSpan.FromDays(1))
			return $"{hours} hour{S(hours)} and {minutes} minute{S(minutes)} ago";

		return utcDateTime.ToString(StringFormats.DateTimeFormat);

		static string S(int value)
			=> value == 1 ? string.Empty : "s";
	}
}
