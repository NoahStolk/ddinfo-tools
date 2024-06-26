using DevilDaggersInfo.Core.Wiki;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Web.ApiSpec.Tools.CustomLeaderboards;
using System.Diagnostics;

namespace DevilDaggersInfo.Tools.Ui.CustomLeaderboards.LeaderboardList;

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
			null => new Core.Wiki.Structs.Color(127, 143, 127),
			_ => throw new UnreachableException(),
		}).ToEngineColor();
	}
}
