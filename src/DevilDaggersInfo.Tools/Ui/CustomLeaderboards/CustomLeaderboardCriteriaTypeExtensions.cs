using DevilDaggersInfo.Core.Wiki;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Web.ApiSpec.Tools.CustomLeaderboards;
using System.Diagnostics;

namespace DevilDaggersInfo.Tools.Ui.CustomLeaderboards;

public static class CustomLeaderboardCriteriaTypeExtensions
{
	public static Texture GetTexture(this CustomLeaderboardCriteriaType type)
	{
		return type switch
		{
			CustomLeaderboardCriteriaType.GemsCollected => Root.GameResources.IconMaskGemTexture,
			CustomLeaderboardCriteriaType.GemsDespawned => Root.GameResources.IconMaskGemTexture,
			CustomLeaderboardCriteriaType.GemsEaten => Root.GameResources.IconMaskGemTexture,
			CustomLeaderboardCriteriaType.EnemiesKilled => Root.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.DaggersFired => Root.GameResources.IconMaskDaggerTexture,
			CustomLeaderboardCriteriaType.DaggersHit => Root.GameResources.IconMaskCrosshairTexture,
			CustomLeaderboardCriteriaType.HomingStored => Root.GameResources.IconMaskHomingTexture,
			CustomLeaderboardCriteriaType.HomingEaten => Root.InternalResources.IconHomingMaskTexture,
			CustomLeaderboardCriteriaType.Skull1Kills => Root.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.Skull2Kills => Root.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.Skull3Kills => Root.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.Skull4Kills => Root.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.SpiderlingKills => Root.InternalResources.IconSpiderTexture,
			CustomLeaderboardCriteriaType.SpiderEggKills => Root.InternalResources.IconEggTexture,
			CustomLeaderboardCriteriaType.Squid1Kills => Root.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.Squid2Kills => Root.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.Squid3Kills => Root.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.CentipedeKills => Root.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.GigapedeKills => Root.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.GhostpedeKills => Root.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.Spider1Kills => Root.InternalResources.IconSpiderTexture,
			CustomLeaderboardCriteriaType.Spider2Kills => Root.InternalResources.IconSpiderTexture,
			CustomLeaderboardCriteriaType.LeviathanKills => Root.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.OrbKills => Root.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.ThornKills => Root.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.Skull1sAlive => Root.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.Skull2sAlive => Root.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.Skull3sAlive => Root.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.Skull4sAlive => Root.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.SpiderlingsAlive => Root.InternalResources.IconSpiderTexture,
			CustomLeaderboardCriteriaType.SpiderEggsAlive => Root.InternalResources.IconEggTexture,
			CustomLeaderboardCriteriaType.Squid1sAlive => Root.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.Squid2sAlive => Root.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.Squid3sAlive => Root.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.CentipedesAlive => Root.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.GigapedesAlive => Root.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.GhostpedesAlive => Root.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.Spider1sAlive => Root.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.Spider2sAlive => Root.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.LeviathansAlive => Root.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.OrbsAlive => Root.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.ThornsAlive => Root.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.DeathType => Root.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.Time => Root.GameResources.IconMaskStopwatchTexture,
			CustomLeaderboardCriteriaType.LevelUpTime2 => Root.GameResources.IconMaskStopwatchTexture,
			CustomLeaderboardCriteriaType.LevelUpTime3 => Root.GameResources.IconMaskStopwatchTexture,
			CustomLeaderboardCriteriaType.LevelUpTime4 => Root.GameResources.IconMaskStopwatchTexture,
			CustomLeaderboardCriteriaType.EnemiesAlive => Root.GameResources.IconMaskSkullTexture,
			_ => Root.InternalResources.IconEyeTexture,
		};
	}

	public static Color GetColor(this CustomLeaderboardCriteriaType type)
	{
		return type switch
		{
			CustomLeaderboardCriteriaType.GemsCollected => Color.Red,
			CustomLeaderboardCriteriaType.GemsDespawned => Color.Gray(0.3f),
			CustomLeaderboardCriteriaType.GemsEaten => Color.Green,
			CustomLeaderboardCriteriaType.EnemiesKilled => Color.Orange,
			CustomLeaderboardCriteriaType.DaggersFired => Color.Yellow,
			CustomLeaderboardCriteriaType.DaggersHit => Color.Orange,
			CustomLeaderboardCriteriaType.HomingStored => Color.White,
			CustomLeaderboardCriteriaType.HomingEaten => Color.Red,
			CustomLeaderboardCriteriaType.Skull1Kills => EnemiesV3_2.Skull1.Color.ToEngineColor(),
			CustomLeaderboardCriteriaType.Skull2Kills => EnemiesV3_2.Skull2.Color.ToEngineColor(),
			CustomLeaderboardCriteriaType.Skull3Kills => EnemiesV3_2.Skull3.Color.ToEngineColor(),
			CustomLeaderboardCriteriaType.Skull4Kills => EnemiesV3_2.Skull4.Color.ToEngineColor(),
			CustomLeaderboardCriteriaType.SpiderlingKills => EnemiesV3_2.Spiderling.Color.ToEngineColor(),
			CustomLeaderboardCriteriaType.SpiderEggKills => EnemiesV3_2.SpiderEgg1.Color.ToEngineColor(),
			CustomLeaderboardCriteriaType.Squid1Kills => EnemiesV3_2.Squid1.Color.ToEngineColor(),
			CustomLeaderboardCriteriaType.Squid2Kills => EnemiesV3_2.Squid2.Color.ToEngineColor(),
			CustomLeaderboardCriteriaType.Squid3Kills => EnemiesV3_2.Squid3.Color.ToEngineColor(),
			CustomLeaderboardCriteriaType.CentipedeKills => EnemiesV3_2.Centipede.Color.ToEngineColor(),
			CustomLeaderboardCriteriaType.GigapedeKills => EnemiesV3_2.Gigapede.Color.ToEngineColor(),
			CustomLeaderboardCriteriaType.GhostpedeKills => EnemiesV3_2.Ghostpede.Color.ToEngineColor(),
			CustomLeaderboardCriteriaType.Spider1Kills => EnemiesV3_2.Spider1.Color.ToEngineColor(),
			CustomLeaderboardCriteriaType.Spider2Kills => EnemiesV3_2.Spider2.Color.ToEngineColor(),
			CustomLeaderboardCriteriaType.LeviathanKills => EnemiesV3_2.Leviathan.Color.ToEngineColor(),
			CustomLeaderboardCriteriaType.OrbKills => EnemiesV3_2.TheOrb.Color.ToEngineColor(),
			CustomLeaderboardCriteriaType.ThornKills => EnemiesV3_2.Thorn.Color.ToEngineColor(),
			CustomLeaderboardCriteriaType.Skull1sAlive => EnemiesV3_2.Skull1.Color.ToEngineColor(),
			CustomLeaderboardCriteriaType.Skull2sAlive => EnemiesV3_2.Skull2.Color.ToEngineColor(),
			CustomLeaderboardCriteriaType.Skull3sAlive => EnemiesV3_2.Skull3.Color.ToEngineColor(),
			CustomLeaderboardCriteriaType.Skull4sAlive => EnemiesV3_2.Skull4.Color.ToEngineColor(),
			CustomLeaderboardCriteriaType.SpiderlingsAlive => EnemiesV3_2.Spiderling.Color.ToEngineColor(),
			CustomLeaderboardCriteriaType.SpiderEggsAlive => EnemiesV3_2.SpiderEgg1.Color.ToEngineColor(),
			CustomLeaderboardCriteriaType.Squid1sAlive => EnemiesV3_2.Squid1.Color.ToEngineColor(),
			CustomLeaderboardCriteriaType.Squid2sAlive => EnemiesV3_2.Squid2.Color.ToEngineColor(),
			CustomLeaderboardCriteriaType.Squid3sAlive => EnemiesV3_2.Squid3.Color.ToEngineColor(),
			CustomLeaderboardCriteriaType.CentipedesAlive => EnemiesV3_2.Centipede.Color.ToEngineColor(),
			CustomLeaderboardCriteriaType.GigapedesAlive => EnemiesV3_2.Gigapede.Color.ToEngineColor(),
			CustomLeaderboardCriteriaType.GhostpedesAlive => EnemiesV3_2.Ghostpede.Color.ToEngineColor(),
			CustomLeaderboardCriteriaType.Spider1sAlive => EnemiesV3_2.Spider1.Color.ToEngineColor(),
			CustomLeaderboardCriteriaType.Spider2sAlive => EnemiesV3_2.Spider2.Color.ToEngineColor(),
			CustomLeaderboardCriteriaType.LeviathansAlive => EnemiesV3_2.Leviathan.Color.ToEngineColor(),
			CustomLeaderboardCriteriaType.OrbsAlive => EnemiesV3_2.TheOrb.Color.ToEngineColor(),
			CustomLeaderboardCriteriaType.ThornsAlive => EnemiesV3_2.Thorn.Color.ToEngineColor(),
			CustomLeaderboardCriteriaType.DeathType => Color.White,
			CustomLeaderboardCriteriaType.Time => Color.White,
			CustomLeaderboardCriteriaType.LevelUpTime2 => Color.White,
			CustomLeaderboardCriteriaType.LevelUpTime3 => Color.White,
			CustomLeaderboardCriteriaType.LevelUpTime4 => Color.White,
			CustomLeaderboardCriteriaType.EnemiesAlive => Color.White,
			_ => Color.White,
		};
	}

	public static DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType ToCore(this CustomLeaderboardCriteriaType type)
	{
		return type switch
		{
			CustomLeaderboardCriteriaType.GemsCollected => Core.CriteriaExpression.CustomLeaderboardCriteriaType.GemsCollected,
			CustomLeaderboardCriteriaType.GemsDespawned => Core.CriteriaExpression.CustomLeaderboardCriteriaType.GemsDespawned,
			CustomLeaderboardCriteriaType.GemsEaten => Core.CriteriaExpression.CustomLeaderboardCriteriaType.GemsEaten,
			CustomLeaderboardCriteriaType.EnemiesKilled => Core.CriteriaExpression.CustomLeaderboardCriteriaType.EnemiesKilled,
			CustomLeaderboardCriteriaType.DaggersFired => Core.CriteriaExpression.CustomLeaderboardCriteriaType.DaggersFired,
			CustomLeaderboardCriteriaType.DaggersHit => Core.CriteriaExpression.CustomLeaderboardCriteriaType.DaggersHit,
			CustomLeaderboardCriteriaType.HomingStored => Core.CriteriaExpression.CustomLeaderboardCriteriaType.HomingStored,
			CustomLeaderboardCriteriaType.HomingEaten => Core.CriteriaExpression.CustomLeaderboardCriteriaType.HomingEaten,
			CustomLeaderboardCriteriaType.Skull1Kills => Core.CriteriaExpression.CustomLeaderboardCriteriaType.Skull1Kills,
			CustomLeaderboardCriteriaType.Skull2Kills => Core.CriteriaExpression.CustomLeaderboardCriteriaType.Skull2Kills,
			CustomLeaderboardCriteriaType.Skull3Kills => Core.CriteriaExpression.CustomLeaderboardCriteriaType.Skull3Kills,
			CustomLeaderboardCriteriaType.Skull4Kills => Core.CriteriaExpression.CustomLeaderboardCriteriaType.Skull4Kills,
			CustomLeaderboardCriteriaType.SpiderlingKills => Core.CriteriaExpression.CustomLeaderboardCriteriaType.SpiderlingKills,
			CustomLeaderboardCriteriaType.SpiderEggKills => Core.CriteriaExpression.CustomLeaderboardCriteriaType.SpiderEggKills,
			CustomLeaderboardCriteriaType.Squid1Kills => Core.CriteriaExpression.CustomLeaderboardCriteriaType.Squid1Kills,
			CustomLeaderboardCriteriaType.Squid2Kills => Core.CriteriaExpression.CustomLeaderboardCriteriaType.Squid2Kills,
			CustomLeaderboardCriteriaType.Squid3Kills => Core.CriteriaExpression.CustomLeaderboardCriteriaType.Squid3Kills,
			CustomLeaderboardCriteriaType.CentipedeKills => Core.CriteriaExpression.CustomLeaderboardCriteriaType.CentipedeKills,
			CustomLeaderboardCriteriaType.GigapedeKills => Core.CriteriaExpression.CustomLeaderboardCriteriaType.GigapedeKills,
			CustomLeaderboardCriteriaType.GhostpedeKills => Core.CriteriaExpression.CustomLeaderboardCriteriaType.GhostpedeKills,
			CustomLeaderboardCriteriaType.Spider1Kills => Core.CriteriaExpression.CustomLeaderboardCriteriaType.Spider1Kills,
			CustomLeaderboardCriteriaType.Spider2Kills => Core.CriteriaExpression.CustomLeaderboardCriteriaType.Spider2Kills,
			CustomLeaderboardCriteriaType.LeviathanKills => Core.CriteriaExpression.CustomLeaderboardCriteriaType.LeviathanKills,
			CustomLeaderboardCriteriaType.OrbKills => Core.CriteriaExpression.CustomLeaderboardCriteriaType.OrbKills,
			CustomLeaderboardCriteriaType.ThornKills => Core.CriteriaExpression.CustomLeaderboardCriteriaType.ThornKills,
			CustomLeaderboardCriteriaType.Skull1sAlive => Core.CriteriaExpression.CustomLeaderboardCriteriaType.Skull1sAlive,
			CustomLeaderboardCriteriaType.Skull2sAlive => Core.CriteriaExpression.CustomLeaderboardCriteriaType.Skull2sAlive,
			CustomLeaderboardCriteriaType.Skull3sAlive => Core.CriteriaExpression.CustomLeaderboardCriteriaType.Skull3sAlive,
			CustomLeaderboardCriteriaType.Skull4sAlive => Core.CriteriaExpression.CustomLeaderboardCriteriaType.Skull4sAlive,
			CustomLeaderboardCriteriaType.SpiderlingsAlive => Core.CriteriaExpression.CustomLeaderboardCriteriaType.SpiderlingsAlive,
			CustomLeaderboardCriteriaType.SpiderEggsAlive => Core.CriteriaExpression.CustomLeaderboardCriteriaType.SpiderEggsAlive,
			CustomLeaderboardCriteriaType.Squid1sAlive => Core.CriteriaExpression.CustomLeaderboardCriteriaType.Squid1sAlive,
			CustomLeaderboardCriteriaType.Squid2sAlive => Core.CriteriaExpression.CustomLeaderboardCriteriaType.Squid2sAlive,
			CustomLeaderboardCriteriaType.Squid3sAlive => Core.CriteriaExpression.CustomLeaderboardCriteriaType.Squid3sAlive,
			CustomLeaderboardCriteriaType.CentipedesAlive => Core.CriteriaExpression.CustomLeaderboardCriteriaType.CentipedesAlive,
			CustomLeaderboardCriteriaType.GigapedesAlive => Core.CriteriaExpression.CustomLeaderboardCriteriaType.GigapedesAlive,
			CustomLeaderboardCriteriaType.GhostpedesAlive => Core.CriteriaExpression.CustomLeaderboardCriteriaType.GhostpedesAlive,
			CustomLeaderboardCriteriaType.Spider1sAlive => Core.CriteriaExpression.CustomLeaderboardCriteriaType.Spider1sAlive,
			CustomLeaderboardCriteriaType.Spider2sAlive => Core.CriteriaExpression.CustomLeaderboardCriteriaType.Spider2sAlive,
			CustomLeaderboardCriteriaType.LeviathansAlive => Core.CriteriaExpression.CustomLeaderboardCriteriaType.LeviathansAlive,
			CustomLeaderboardCriteriaType.OrbsAlive => Core.CriteriaExpression.CustomLeaderboardCriteriaType.OrbsAlive,
			CustomLeaderboardCriteriaType.ThornsAlive => Core.CriteriaExpression.CustomLeaderboardCriteriaType.ThornsAlive,
			CustomLeaderboardCriteriaType.DeathType => Core.CriteriaExpression.CustomLeaderboardCriteriaType.DeathType,
			CustomLeaderboardCriteriaType.Time => Core.CriteriaExpression.CustomLeaderboardCriteriaType.Time,
			CustomLeaderboardCriteriaType.LevelUpTime2 => Core.CriteriaExpression.CustomLeaderboardCriteriaType.LevelUpTime2,
			CustomLeaderboardCriteriaType.LevelUpTime3 => Core.CriteriaExpression.CustomLeaderboardCriteriaType.LevelUpTime3,
			CustomLeaderboardCriteriaType.LevelUpTime4 => Core.CriteriaExpression.CustomLeaderboardCriteriaType.LevelUpTime4,
			CustomLeaderboardCriteriaType.EnemiesAlive => Core.CriteriaExpression.CustomLeaderboardCriteriaType.EnemiesAlive,
			_ => throw new UnreachableException(),
		};
	}

	public static DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaOperator ToCore(this CustomLeaderboardCriteriaOperator @operator)
	{
		return @operator switch
		{
			CustomLeaderboardCriteriaOperator.Any => Core.CriteriaExpression.CustomLeaderboardCriteriaOperator.Any,
			CustomLeaderboardCriteriaOperator.Equal => Core.CriteriaExpression.CustomLeaderboardCriteriaOperator.Equal,
			CustomLeaderboardCriteriaOperator.LessThan => Core.CriteriaExpression.CustomLeaderboardCriteriaOperator.LessThan,
			CustomLeaderboardCriteriaOperator.GreaterThan => Core.CriteriaExpression.CustomLeaderboardCriteriaOperator.GreaterThan,
			CustomLeaderboardCriteriaOperator.LessThanOrEqual => Core.CriteriaExpression.CustomLeaderboardCriteriaOperator.LessThanOrEqual,
			CustomLeaderboardCriteriaOperator.GreaterThanOrEqual => Core.CriteriaExpression.CustomLeaderboardCriteriaOperator.GreaterThanOrEqual,
			CustomLeaderboardCriteriaOperator.Modulo => Core.CriteriaExpression.CustomLeaderboardCriteriaOperator.Modulo,
			CustomLeaderboardCriteriaOperator.NotEqual => Core.CriteriaExpression.CustomLeaderboardCriteriaOperator.NotEqual,
			_ => throw new UnreachableException(),
		};
	}
}
