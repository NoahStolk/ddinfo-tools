using DevilDaggersInfo.Core.Wiki;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Web.ApiSpec.Tools.CustomLeaderboards;
using System.Diagnostics;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.CustomLeaderboards.LeaderboardList;

internal static class CustomLeaderboardDaggerUtils
{
	public static Vector4 GetColor(CustomLeaderboardDagger? customLeaderboardDagger)
	{
		Vector3 color = customLeaderboardDagger switch
		{
			CustomLeaderboardDagger.Default => DaggerColors.Default,
			CustomLeaderboardDagger.Bronze => DaggerColors.Bronze,
			CustomLeaderboardDagger.Silver => DaggerColors.Silver,
			CustomLeaderboardDagger.Golden => DaggerColors.Golden,
			CustomLeaderboardDagger.Devil => DaggerColors.Devil,
			CustomLeaderboardDagger.Leviathan => DaggerColors.Leviathan,
			null => new Vector3(0.5f, 0.56f, 0.5f),
			_ => throw new UnreachableException(),
		};
		return color.ToVector4();
	}
}
