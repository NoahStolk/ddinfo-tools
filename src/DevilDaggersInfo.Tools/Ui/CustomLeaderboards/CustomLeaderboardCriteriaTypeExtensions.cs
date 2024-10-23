using DevilDaggersInfo.Core.Wiki;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Web.ApiSpec.Tools.CustomLeaderboards;
using System.Diagnostics;

namespace DevilDaggersInfo.Tools.Ui.CustomLeaderboards;

public static class CustomLeaderboardCriteriaTypeExtensions
{
	public static Texture GetTexture(this ResourceManager resourceManager, CustomLeaderboardCriteriaType type)
	{
		if (resourceManager.GameResources == null)
			throw new UnreachableException("Game resources not loaded while attempting to retrieve criteria texture for custom leaderboards.");

		return type switch
		{
			CustomLeaderboardCriteriaType.GemsCollected => resourceManager.GameResources.IconMaskGemTexture,
			CustomLeaderboardCriteriaType.GemsDespawned => resourceManager.GameResources.IconMaskGemTexture,
			CustomLeaderboardCriteriaType.GemsEaten => resourceManager.GameResources.IconMaskGemTexture,
			CustomLeaderboardCriteriaType.EnemiesKilled => resourceManager.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.DaggersFired => resourceManager.GameResources.IconMaskDaggerTexture,
			CustomLeaderboardCriteriaType.DaggersHit => resourceManager.GameResources.IconMaskCrosshairTexture,
			CustomLeaderboardCriteriaType.HomingStored => resourceManager.GameResources.IconMaskHomingTexture,
			CustomLeaderboardCriteriaType.HomingEaten => resourceManager.InternalResources.IconHomingMaskTexture,
			CustomLeaderboardCriteriaType.Skull1Kills => resourceManager.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.Skull2Kills => resourceManager.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.Skull3Kills => resourceManager.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.Skull4Kills => resourceManager.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.SpiderlingKills => resourceManager.InternalResources.IconSpiderTexture,
			CustomLeaderboardCriteriaType.SpiderEggKills => resourceManager.InternalResources.IconEggTexture,
			CustomLeaderboardCriteriaType.Squid1Kills => resourceManager.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.Squid2Kills => resourceManager.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.Squid3Kills => resourceManager.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.CentipedeKills => resourceManager.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.GigapedeKills => resourceManager.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.GhostpedeKills => resourceManager.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.Spider1Kills => resourceManager.InternalResources.IconSpiderTexture,
			CustomLeaderboardCriteriaType.Spider2Kills => resourceManager.InternalResources.IconSpiderTexture,
			CustomLeaderboardCriteriaType.LeviathanKills => resourceManager.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.OrbKills => resourceManager.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.ThornKills => resourceManager.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.Skull1sAlive => resourceManager.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.Skull2sAlive => resourceManager.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.Skull3sAlive => resourceManager.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.Skull4sAlive => resourceManager.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.SpiderlingsAlive => resourceManager.InternalResources.IconSpiderTexture,
			CustomLeaderboardCriteriaType.SpiderEggsAlive => resourceManager.InternalResources.IconEggTexture,
			CustomLeaderboardCriteriaType.Squid1sAlive => resourceManager.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.Squid2sAlive => resourceManager.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.Squid3sAlive => resourceManager.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.CentipedesAlive => resourceManager.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.GigapedesAlive => resourceManager.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.GhostpedesAlive => resourceManager.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.Spider1sAlive => resourceManager.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.Spider2sAlive => resourceManager.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.LeviathansAlive => resourceManager.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.OrbsAlive => resourceManager.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.ThornsAlive => resourceManager.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.DeathType => resourceManager.GameResources.IconMaskSkullTexture,
			CustomLeaderboardCriteriaType.Time => resourceManager.GameResources.IconMaskStopwatchTexture,
			CustomLeaderboardCriteriaType.LevelUpTime2 => resourceManager.GameResources.IconMaskStopwatchTexture,
			CustomLeaderboardCriteriaType.LevelUpTime3 => resourceManager.GameResources.IconMaskStopwatchTexture,
			CustomLeaderboardCriteriaType.LevelUpTime4 => resourceManager.GameResources.IconMaskStopwatchTexture,
			CustomLeaderboardCriteriaType.EnemiesAlive => resourceManager.GameResources.IconMaskSkullTexture,
			_ => resourceManager.InternalResources.IconEyeTexture,
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
