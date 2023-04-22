using DevilDaggersInfo.App.Engine.Content;
using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Ui.Base;
using DevilDaggersInfo.App.Ui.Base.Extensions;
using DevilDaggersInfo.Core.Wiki;
using DevilDaggersInfo.Types.Core.CustomLeaderboards;

namespace DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Extensions;

public static class CustomLeaderboardCriteriaTypeExtensions
{
	public static Texture GetTexture(this CustomLeaderboardCriteriaType type) => type switch
	{
		CustomLeaderboardCriteriaType.GemsCollected => Textures.IconGem,
		CustomLeaderboardCriteriaType.GemsDespawned => Textures.IconGem,
		CustomLeaderboardCriteriaType.GemsEaten => Textures.IconGem,
		CustomLeaderboardCriteriaType.EnemiesKilled => Textures.IconSkull,
		CustomLeaderboardCriteriaType.DaggersFired => Textures.IconDagger,
		CustomLeaderboardCriteriaType.DaggersHit => Textures.IconCrosshair,
		CustomLeaderboardCriteriaType.HomingStored => Textures.IconHoming,
		CustomLeaderboardCriteriaType.HomingEaten => Textures.IconHomingMask,
		CustomLeaderboardCriteriaType.Skull1Kills => Textures.IconSkull,
		CustomLeaderboardCriteriaType.Skull2Kills => Textures.IconSkull,
		CustomLeaderboardCriteriaType.Skull3Kills => Textures.IconSkull,
		CustomLeaderboardCriteriaType.Skull4Kills => Textures.IconSkull,
		CustomLeaderboardCriteriaType.SpiderlingKills => Textures.IconSpider,
		CustomLeaderboardCriteriaType.SpiderEggKills => Textures.IconEgg,
		CustomLeaderboardCriteriaType.Squid1Kills => Textures.IconSkull,
		CustomLeaderboardCriteriaType.Squid2Kills => Textures.IconSkull,
		CustomLeaderboardCriteriaType.Squid3Kills => Textures.IconSkull,
		CustomLeaderboardCriteriaType.CentipedeKills => Textures.IconSkull,
		CustomLeaderboardCriteriaType.GigapedeKills => Textures.IconSkull,
		CustomLeaderboardCriteriaType.GhostpedeKills => Textures.IconSkull,
		CustomLeaderboardCriteriaType.Spider1Kills => Textures.IconSpider,
		CustomLeaderboardCriteriaType.Spider2Kills => Textures.IconSpider,
		CustomLeaderboardCriteriaType.LeviathanKills => Textures.IconSkull,
		CustomLeaderboardCriteriaType.OrbKills => Textures.IconSkull,
		CustomLeaderboardCriteriaType.ThornKills => Textures.IconSkull,
		CustomLeaderboardCriteriaType.Skull1sAlive => Textures.IconSkull,
		CustomLeaderboardCriteriaType.Skull2sAlive => Textures.IconSkull,
		CustomLeaderboardCriteriaType.Skull3sAlive => Textures.IconSkull,
		CustomLeaderboardCriteriaType.Skull4sAlive => Textures.IconSkull,
		CustomLeaderboardCriteriaType.SpiderlingsAlive => Textures.IconSpider,
		CustomLeaderboardCriteriaType.SpiderEggsAlive => Textures.IconEgg,
		CustomLeaderboardCriteriaType.Squid1sAlive => Textures.IconSkull,
		CustomLeaderboardCriteriaType.Squid2sAlive => Textures.IconSkull,
		CustomLeaderboardCriteriaType.Squid3sAlive => Textures.IconSkull,
		CustomLeaderboardCriteriaType.CentipedesAlive => Textures.IconSkull,
		CustomLeaderboardCriteriaType.GigapedesAlive => Textures.IconSkull,
		CustomLeaderboardCriteriaType.GhostpedesAlive => Textures.IconSkull,
		CustomLeaderboardCriteriaType.Spider1sAlive => Textures.IconSkull,
		CustomLeaderboardCriteriaType.Spider2sAlive => Textures.IconSkull,
		CustomLeaderboardCriteriaType.LeviathansAlive => Textures.IconSkull,
		CustomLeaderboardCriteriaType.OrbsAlive => Textures.IconSkull,
		CustomLeaderboardCriteriaType.ThornsAlive => Textures.IconSkull,
		CustomLeaderboardCriteriaType.DeathType => Textures.IconSkull,
		CustomLeaderboardCriteriaType.Time => Textures.IconStopwatch,
		CustomLeaderboardCriteriaType.LevelUpTime2 => Textures.IconStopwatch,
		CustomLeaderboardCriteriaType.LevelUpTime3 => Textures.IconStopwatch,
		CustomLeaderboardCriteriaType.LevelUpTime4 => Textures.IconStopwatch,
		CustomLeaderboardCriteriaType.EnemiesAlive => Textures.IconSkull,
		_ => Textures.IconEye,
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
		CustomLeaderboardCriteriaType.Skull1Kills => EnemiesV3_2.Skull1.Color.ToWarpColor(),
		CustomLeaderboardCriteriaType.Skull2Kills => EnemiesV3_2.Skull2.Color.ToWarpColor(),
		CustomLeaderboardCriteriaType.Skull3Kills => EnemiesV3_2.Skull3.Color.ToWarpColor(),
		CustomLeaderboardCriteriaType.Skull4Kills => EnemiesV3_2.Skull4.Color.ToWarpColor(),
		CustomLeaderboardCriteriaType.SpiderlingKills => EnemiesV3_2.Spiderling.Color.ToWarpColor(),
		CustomLeaderboardCriteriaType.SpiderEggKills => EnemiesV3_2.SpiderEgg1.Color.ToWarpColor(),
		CustomLeaderboardCriteriaType.Squid1Kills => EnemiesV3_2.Squid1.Color.ToWarpColor(),
		CustomLeaderboardCriteriaType.Squid2Kills => EnemiesV3_2.Squid2.Color.ToWarpColor(),
		CustomLeaderboardCriteriaType.Squid3Kills => EnemiesV3_2.Squid3.Color.ToWarpColor(),
		CustomLeaderboardCriteriaType.CentipedeKills => EnemiesV3_2.Centipede.Color.ToWarpColor(),
		CustomLeaderboardCriteriaType.GigapedeKills => EnemiesV3_2.Gigapede.Color.ToWarpColor(),
		CustomLeaderboardCriteriaType.GhostpedeKills => EnemiesV3_2.Ghostpede.Color.ToWarpColor(),
		CustomLeaderboardCriteriaType.Spider1Kills => EnemiesV3_2.Spider1.Color.ToWarpColor(),
		CustomLeaderboardCriteriaType.Spider2Kills => EnemiesV3_2.Spider2.Color.ToWarpColor(),
		CustomLeaderboardCriteriaType.LeviathanKills => EnemiesV3_2.Leviathan.Color.ToWarpColor(),
		CustomLeaderboardCriteriaType.OrbKills => EnemiesV3_2.TheOrb.Color.ToWarpColor(),
		CustomLeaderboardCriteriaType.ThornKills => EnemiesV3_2.Thorn.Color.ToWarpColor(),
		CustomLeaderboardCriteriaType.Skull1sAlive => EnemiesV3_2.Skull1.Color.ToWarpColor(),
		CustomLeaderboardCriteriaType.Skull2sAlive => EnemiesV3_2.Skull2.Color.ToWarpColor(),
		CustomLeaderboardCriteriaType.Skull3sAlive => EnemiesV3_2.Skull3.Color.ToWarpColor(),
		CustomLeaderboardCriteriaType.Skull4sAlive => EnemiesV3_2.Skull4.Color.ToWarpColor(),
		CustomLeaderboardCriteriaType.SpiderlingsAlive => EnemiesV3_2.Spiderling.Color.ToWarpColor(),
		CustomLeaderboardCriteriaType.SpiderEggsAlive => EnemiesV3_2.SpiderEgg1.Color.ToWarpColor(),
		CustomLeaderboardCriteriaType.Squid1sAlive => EnemiesV3_2.Squid1.Color.ToWarpColor(),
		CustomLeaderboardCriteriaType.Squid2sAlive => EnemiesV3_2.Squid2.Color.ToWarpColor(),
		CustomLeaderboardCriteriaType.Squid3sAlive => EnemiesV3_2.Squid3.Color.ToWarpColor(),
		CustomLeaderboardCriteriaType.CentipedesAlive => EnemiesV3_2.Centipede.Color.ToWarpColor(),
		CustomLeaderboardCriteriaType.GigapedesAlive => EnemiesV3_2.Gigapede.Color.ToWarpColor(),
		CustomLeaderboardCriteriaType.GhostpedesAlive => EnemiesV3_2.Ghostpede.Color.ToWarpColor(),
		CustomLeaderboardCriteriaType.Spider1sAlive => EnemiesV3_2.Spider1.Color.ToWarpColor(),
		CustomLeaderboardCriteriaType.Spider2sAlive => EnemiesV3_2.Spider2.Color.ToWarpColor(),
		CustomLeaderboardCriteriaType.LeviathansAlive => EnemiesV3_2.Leviathan.Color.ToWarpColor(),
		CustomLeaderboardCriteriaType.OrbsAlive => EnemiesV3_2.TheOrb.Color.ToWarpColor(),
		CustomLeaderboardCriteriaType.ThornsAlive => EnemiesV3_2.Thorn.Color.ToWarpColor(),
		CustomLeaderboardCriteriaType.DeathType => Color.White,
		CustomLeaderboardCriteriaType.Time => Color.White,
		CustomLeaderboardCriteriaType.LevelUpTime2 => Color.White,
		CustomLeaderboardCriteriaType.LevelUpTime3 => Color.White,
		CustomLeaderboardCriteriaType.LevelUpTime4 => Color.White,
		CustomLeaderboardCriteriaType.EnemiesAlive => Color.White,
		_ => Color.White,
	};
}
