using DevilDaggersInfo.Common.Exceptions;
using DevilDaggersInfo.Core.Wiki;
using DevilDaggersInfo.Types.Web;

namespace DevilDaggersInfo.App.Ui.Base.Extensions;

public static class CustomLeaderboardDaggerExtensions
{
	public static Color GetColor(this CustomLeaderboardDagger customLeaderboardDagger)
	{
		return (customLeaderboardDagger switch
		{
			CustomLeaderboardDagger.Default => DaggerColors.Default,
			CustomLeaderboardDagger.Bronze => DaggerColors.Bronze,
			CustomLeaderboardDagger.Silver => DaggerColors.Silver,
			CustomLeaderboardDagger.Golden => DaggerColors.Golden,
			CustomLeaderboardDagger.Devil => DaggerColors.Devil,
			CustomLeaderboardDagger.Leviathan => DaggerColors.Leviathan,
			_ => throw new InvalidEnumConversionException(customLeaderboardDagger),
		}).ToWarpColor();
	}
}
