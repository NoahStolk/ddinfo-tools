using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Core.Wiki;
using System.Diagnostics;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Extensions;

internal static class HandLevelExtensions
{
	public static Vector4 GetColor(this HandLevel handLevel)
	{
		return handLevel switch
		{
			HandLevel.Level1 => UpgradeColors.Level1,
			HandLevel.Level2 => UpgradeColors.Level2,
			HandLevel.Level3 => UpgradeColors.Level3,
			HandLevel.Level4 => UpgradeColors.Level4,
			_ => throw new UnreachableException(),
		};
	}
}
