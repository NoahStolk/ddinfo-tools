using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Core.Wiki;
using System.Diagnostics;

namespace DevilDaggersInfo.App.Ui.Base.Extensions;

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
