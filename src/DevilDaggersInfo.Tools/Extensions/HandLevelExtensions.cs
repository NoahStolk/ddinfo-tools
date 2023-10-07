using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Core.Wiki;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using System.Diagnostics;

namespace DevilDaggersInfo.Tools.Extensions;

public static class HandLevelExtensions
{
	public static Color GetColor(this HandLevel handLevel) => handLevel switch
	{
		HandLevel.Level1 => UpgradeColors.Level1.ToEngineColor(),
		HandLevel.Level2 => UpgradeColors.Level2.ToEngineColor(),
		HandLevel.Level3 => UpgradeColors.Level3.ToEngineColor(),
		HandLevel.Level4 => UpgradeColors.Level4.ToEngineColor(),
		_ => throw new UnreachableException(),
	};
}
