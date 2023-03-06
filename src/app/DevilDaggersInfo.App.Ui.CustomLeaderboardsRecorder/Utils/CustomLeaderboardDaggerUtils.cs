using DevilDaggersInfo.App.Ui.Base.Extensions;
using DevilDaggersInfo.Core.Wiki;
using DevilDaggersInfo.Types.Web;
using System.Diagnostics;

namespace DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Utils;

public static class CustomLeaderboardDaggerUtils
{
	public static Color GetColor(CustomLeaderboardDagger? customLeaderboardDagger)
	{
		return (customLeaderboardDagger switch
		{
			CustomLeaderboardDagger.Default => DaggerColors.Default,
			CustomLeaderboardDagger.Bronze => DaggerColors.Bronze,
			CustomLeaderboardDagger.Silver => DaggerColors.Silver,
			CustomLeaderboardDagger.Golden => DaggerColors.Golden,
			CustomLeaderboardDagger.Devil => DaggerColors.Devil,
			CustomLeaderboardDagger.Leviathan => DaggerColors.Leviathan,
			null => new(127, 143, 127),
			_ => throw new UnreachableException(),
		}).ToWarpColor();
	}
}
