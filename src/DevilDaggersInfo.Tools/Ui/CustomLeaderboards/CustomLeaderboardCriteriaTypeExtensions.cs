using DevilDaggersInfo.Core.Wiki;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Web.ApiSpec.Tools.CustomLeaderboards;
using System.Diagnostics;

namespace DevilDaggersInfo.Tools.Ui.CustomLeaderboards;

public static class CustomLeaderboardCriteriaTypeExtensions
{
	public static Texture GetTexture(this CustomLeaderboardCriteriaType type) => type switch
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

	public static Color GetColor(this CustomLeaderboardCriteriaType type) => type switch
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

	public static DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType ToCore(this CustomLeaderboardCriteriaType type) => type switch
	{
		CustomLeaderboardCriteriaType.GemsCollected => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.GemsCollected,
		CustomLeaderboardCriteriaType.GemsDespawned => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.GemsDespawned,
		CustomLeaderboardCriteriaType.GemsEaten => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.GemsEaten,
		CustomLeaderboardCriteriaType.EnemiesKilled => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.EnemiesKilled,
		CustomLeaderboardCriteriaType.DaggersFired => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.DaggersFired,
		CustomLeaderboardCriteriaType.DaggersHit => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.DaggersHit,
		CustomLeaderboardCriteriaType.HomingStored => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.HomingStored,
		CustomLeaderboardCriteriaType.HomingEaten => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.HomingEaten,
		CustomLeaderboardCriteriaType.Skull1Kills => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.Skull1Kills,
		CustomLeaderboardCriteriaType.Skull2Kills => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.Skull2Kills,
		CustomLeaderboardCriteriaType.Skull3Kills => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.Skull3Kills,
		CustomLeaderboardCriteriaType.Skull4Kills => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.Skull4Kills,
		CustomLeaderboardCriteriaType.SpiderlingKills => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.SpiderlingKills,
		CustomLeaderboardCriteriaType.SpiderEggKills => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.SpiderEggKills,
		CustomLeaderboardCriteriaType.Squid1Kills => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.Squid1Kills,
		CustomLeaderboardCriteriaType.Squid2Kills => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.Squid2Kills,
		CustomLeaderboardCriteriaType.Squid3Kills => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.Squid3Kills,
		CustomLeaderboardCriteriaType.CentipedeKills => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.CentipedeKills,
		CustomLeaderboardCriteriaType.GigapedeKills => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.GigapedeKills,
		CustomLeaderboardCriteriaType.GhostpedeKills => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.GhostpedeKills,
		CustomLeaderboardCriteriaType.Spider1Kills => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.Spider1Kills,
		CustomLeaderboardCriteriaType.Spider2Kills => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.Spider2Kills,
		CustomLeaderboardCriteriaType.LeviathanKills => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.LeviathanKills,
		CustomLeaderboardCriteriaType.OrbKills => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.OrbKills,
		CustomLeaderboardCriteriaType.ThornKills => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.ThornKills,
		CustomLeaderboardCriteriaType.Skull1sAlive => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.Skull1sAlive,
		CustomLeaderboardCriteriaType.Skull2sAlive => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.Skull2sAlive,
		CustomLeaderboardCriteriaType.Skull3sAlive => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.Skull3sAlive,
		CustomLeaderboardCriteriaType.Skull4sAlive => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.Skull4sAlive,
		CustomLeaderboardCriteriaType.SpiderlingsAlive => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.SpiderlingsAlive,
		CustomLeaderboardCriteriaType.SpiderEggsAlive => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.SpiderEggsAlive,
		CustomLeaderboardCriteriaType.Squid1sAlive => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.Squid1sAlive,
		CustomLeaderboardCriteriaType.Squid2sAlive => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.Squid2sAlive,
		CustomLeaderboardCriteriaType.Squid3sAlive => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.Squid3sAlive,
		CustomLeaderboardCriteriaType.CentipedesAlive => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.CentipedesAlive,
		CustomLeaderboardCriteriaType.GigapedesAlive => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.GigapedesAlive,
		CustomLeaderboardCriteriaType.GhostpedesAlive => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.GhostpedesAlive,
		CustomLeaderboardCriteriaType.Spider1sAlive => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.Spider1sAlive,
		CustomLeaderboardCriteriaType.Spider2sAlive => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.Spider2sAlive,
		CustomLeaderboardCriteriaType.LeviathansAlive => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.LeviathansAlive,
		CustomLeaderboardCriteriaType.OrbsAlive => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.OrbsAlive,
		CustomLeaderboardCriteriaType.ThornsAlive => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.ThornsAlive,
		CustomLeaderboardCriteriaType.DeathType => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.DeathType,
		CustomLeaderboardCriteriaType.Time => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.Time,
		CustomLeaderboardCriteriaType.LevelUpTime2 => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.LevelUpTime2,
		CustomLeaderboardCriteriaType.LevelUpTime3 => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.LevelUpTime3,
		CustomLeaderboardCriteriaType.LevelUpTime4 => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.LevelUpTime4,
		CustomLeaderboardCriteriaType.EnemiesAlive => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType.EnemiesAlive,
		_ => throw new UnreachableException(),
	};

	public static DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaOperator ToCore(this CustomLeaderboardCriteriaOperator @operator) => @operator switch
	{
		CustomLeaderboardCriteriaOperator.Any => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaOperator.Any,
		CustomLeaderboardCriteriaOperator.Equal => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaOperator.Equal,
		CustomLeaderboardCriteriaOperator.LessThan => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaOperator.LessThan,
		CustomLeaderboardCriteriaOperator.GreaterThan => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaOperator.GreaterThan,
		CustomLeaderboardCriteriaOperator.LessThanOrEqual => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaOperator.LessThanOrEqual,
		CustomLeaderboardCriteriaOperator.GreaterThanOrEqual => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaOperator.GreaterThanOrEqual,
		CustomLeaderboardCriteriaOperator.Modulo => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaOperator.Modulo,
		CustomLeaderboardCriteriaOperator.NotEqual => DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaOperator.NotEqual,
		_ => throw new UnreachableException(),
	};
}
