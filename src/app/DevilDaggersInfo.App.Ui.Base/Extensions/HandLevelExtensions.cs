using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Core.Wiki;
using System.Diagnostics;

namespace DevilDaggersInfo.App.Ui.Base.Extensions;

public static class HandLevelExtensions
{
	public static Color GetColor(this HandLevel handLevel) => handLevel switch
	{
		HandLevel.Level1 => UpgradeColors.Level1.ToWarpColor(),
		HandLevel.Level2 => UpgradeColors.Level2.ToWarpColor(),
		HandLevel.Level3 => UpgradeColors.Level3.ToWarpColor(),
		HandLevel.Level4 => UpgradeColors.Level4.ToWarpColor(),
		_ => throw new UnreachableException(),
	};
}
