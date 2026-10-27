using DevilDaggersInfo.Core.Wiki;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Web.ApiSpec.Tools.CustomLeaderboards;
using System.Diagnostics;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.CustomLeaderboards;

internal static class CustomLeaderboardCriteriaTypeExtensions
{
	extension(CustomLeaderboardCriteriaType type)
	{
		// ReSharper disable RedundantSwitchExpressionArms
		public Vector4 GetColor()
		{
			Vector3 color = type switch
			{
				CustomLeaderboardCriteriaType.GemsCollected => new Vector3(1, 0, 0),
				CustomLeaderboardCriteriaType.GemsDespawned => new Vector3(0.3f, 0.3f, 0.3f),
				CustomLeaderboardCriteriaType.GemsEaten => new Vector3(0, 1, 0),
				CustomLeaderboardCriteriaType.EnemiesKilled => new Vector3(1, 0.5f, 0),
				CustomLeaderboardCriteriaType.DaggersFired => new Vector3(1, 1, 0),
				CustomLeaderboardCriteriaType.DaggersHit => new Vector3(1, 0.5f, 0),
				CustomLeaderboardCriteriaType.HomingStored => Vector3.One,
				CustomLeaderboardCriteriaType.HomingEaten => new Vector3(1, 0, 0),
				CustomLeaderboardCriteriaType.Skull1Kills => EnemiesV3_2.Skull1.Color,
				CustomLeaderboardCriteriaType.Skull2Kills => EnemiesV3_2.Skull2.Color,
				CustomLeaderboardCriteriaType.Skull3Kills => EnemiesV3_2.Skull3.Color,
				CustomLeaderboardCriteriaType.Skull4Kills => EnemiesV3_2.Skull4.Color,
				CustomLeaderboardCriteriaType.SpiderlingKills => EnemiesV3_2.Spiderling.Color,
				CustomLeaderboardCriteriaType.SpiderEggKills => EnemiesV3_2.SpiderEgg1.Color,
				CustomLeaderboardCriteriaType.Squid1Kills => EnemiesV3_2.Squid1.Color,
				CustomLeaderboardCriteriaType.Squid2Kills => EnemiesV3_2.Squid2.Color,
				CustomLeaderboardCriteriaType.Squid3Kills => EnemiesV3_2.Squid3.Color,
				CustomLeaderboardCriteriaType.CentipedeKills => EnemiesV3_2.Centipede.Color,
				CustomLeaderboardCriteriaType.GigapedeKills => EnemiesV3_2.Gigapede.Color,
				CustomLeaderboardCriteriaType.GhostpedeKills => EnemiesV3_2.Ghostpede.Color,
				CustomLeaderboardCriteriaType.Spider1Kills => EnemiesV3_2.Spider1.Color,
				CustomLeaderboardCriteriaType.Spider2Kills => EnemiesV3_2.Spider2.Color,
				CustomLeaderboardCriteriaType.LeviathanKills => EnemiesV3_2.Leviathan.Color,
				CustomLeaderboardCriteriaType.OrbKills => EnemiesV3_2.TheOrb.Color,
				CustomLeaderboardCriteriaType.ThornKills => EnemiesV3_2.Thorn.Color,
				CustomLeaderboardCriteriaType.Skull1sAlive => EnemiesV3_2.Skull1.Color,
				CustomLeaderboardCriteriaType.Skull2sAlive => EnemiesV3_2.Skull2.Color,
				CustomLeaderboardCriteriaType.Skull3sAlive => EnemiesV3_2.Skull3.Color,
				CustomLeaderboardCriteriaType.Skull4sAlive => EnemiesV3_2.Skull4.Color,
				CustomLeaderboardCriteriaType.SpiderlingsAlive => EnemiesV3_2.Spiderling.Color,
				CustomLeaderboardCriteriaType.SpiderEggsAlive => EnemiesV3_2.SpiderEgg1.Color,
				CustomLeaderboardCriteriaType.Squid1sAlive => EnemiesV3_2.Squid1.Color,
				CustomLeaderboardCriteriaType.Squid2sAlive => EnemiesV3_2.Squid2.Color,
				CustomLeaderboardCriteriaType.Squid3sAlive => EnemiesV3_2.Squid3.Color,
				CustomLeaderboardCriteriaType.CentipedesAlive => EnemiesV3_2.Centipede.Color,
				CustomLeaderboardCriteriaType.GigapedesAlive => EnemiesV3_2.Gigapede.Color,
				CustomLeaderboardCriteriaType.GhostpedesAlive => EnemiesV3_2.Ghostpede.Color,
				CustomLeaderboardCriteriaType.Spider1sAlive => EnemiesV3_2.Spider1.Color,
				CustomLeaderboardCriteriaType.Spider2sAlive => EnemiesV3_2.Spider2.Color,
				CustomLeaderboardCriteriaType.LeviathansAlive => EnemiesV3_2.Leviathan.Color,
				CustomLeaderboardCriteriaType.OrbsAlive => EnemiesV3_2.TheOrb.Color,
				CustomLeaderboardCriteriaType.ThornsAlive => EnemiesV3_2.Thorn.Color,
				CustomLeaderboardCriteriaType.DeathType => Vector3.One,
				CustomLeaderboardCriteriaType.Time => Vector3.One,
				CustomLeaderboardCriteriaType.LevelUpTime2 => Vector3.One,
				CustomLeaderboardCriteriaType.LevelUpTime3 => Vector3.One,
				CustomLeaderboardCriteriaType.LevelUpTime4 => Vector3.One,
				CustomLeaderboardCriteriaType.EnemiesAlive => Vector3.One,
				_ => Vector3.One,
			};
			return color.ToVector4();
		}

		// ReSharper restore RedundantSwitchExpressionArms
		public DevilDaggersInfo.Core.CriteriaExpression.CustomLeaderboardCriteriaType ToCore()
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
				CustomLeaderboardCriteriaType.Skull1Kills => Core.CriteriaExpression.CustomLeaderboardCriteriaType.Skull1KillCount,
				CustomLeaderboardCriteriaType.Skull2Kills => Core.CriteriaExpression.CustomLeaderboardCriteriaType.Skull2KillCount,
				CustomLeaderboardCriteriaType.Skull3Kills => Core.CriteriaExpression.CustomLeaderboardCriteriaType.Skull3KillCount,
				CustomLeaderboardCriteriaType.Skull4Kills => Core.CriteriaExpression.CustomLeaderboardCriteriaType.Skull4KillCount,
				CustomLeaderboardCriteriaType.SpiderlingKills => Core.CriteriaExpression.CustomLeaderboardCriteriaType.SpiderlingKillCount,
				CustomLeaderboardCriteriaType.SpiderEggKills => Core.CriteriaExpression.CustomLeaderboardCriteriaType.SpiderEggKillCount,
				CustomLeaderboardCriteriaType.Squid1Kills => Core.CriteriaExpression.CustomLeaderboardCriteriaType.Squid1KillCount,
				CustomLeaderboardCriteriaType.Squid2Kills => Core.CriteriaExpression.CustomLeaderboardCriteriaType.Squid2KillCount,
				CustomLeaderboardCriteriaType.Squid3Kills => Core.CriteriaExpression.CustomLeaderboardCriteriaType.Squid3KillCount,
				CustomLeaderboardCriteriaType.CentipedeKills => Core.CriteriaExpression.CustomLeaderboardCriteriaType.CentipedeKillCount,
				CustomLeaderboardCriteriaType.GigapedeKills => Core.CriteriaExpression.CustomLeaderboardCriteriaType.GigapedeKillCount,
				CustomLeaderboardCriteriaType.GhostpedeKills => Core.CriteriaExpression.CustomLeaderboardCriteriaType.GhostpedeKillCount,
				CustomLeaderboardCriteriaType.Spider1Kills => Core.CriteriaExpression.CustomLeaderboardCriteriaType.Spider1KillCount,
				CustomLeaderboardCriteriaType.Spider2Kills => Core.CriteriaExpression.CustomLeaderboardCriteriaType.Spider2KillCount,
				CustomLeaderboardCriteriaType.LeviathanKills => Core.CriteriaExpression.CustomLeaderboardCriteriaType.LeviathanKillCount,
				CustomLeaderboardCriteriaType.OrbKills => Core.CriteriaExpression.CustomLeaderboardCriteriaType.OrbKillCount,
				CustomLeaderboardCriteriaType.ThornKills => Core.CriteriaExpression.CustomLeaderboardCriteriaType.ThornKillCount,
				CustomLeaderboardCriteriaType.Skull1sAlive => Core.CriteriaExpression.CustomLeaderboardCriteriaType.Skull1AliveCount,
				CustomLeaderboardCriteriaType.Skull2sAlive => Core.CriteriaExpression.CustomLeaderboardCriteriaType.Skull2AliveCount,
				CustomLeaderboardCriteriaType.Skull3sAlive => Core.CriteriaExpression.CustomLeaderboardCriteriaType.Skull3AliveCount,
				CustomLeaderboardCriteriaType.Skull4sAlive => Core.CriteriaExpression.CustomLeaderboardCriteriaType.Skull4AliveCount,
				CustomLeaderboardCriteriaType.SpiderlingsAlive => Core.CriteriaExpression.CustomLeaderboardCriteriaType.SpiderlingAliveCount,
				CustomLeaderboardCriteriaType.SpiderEggsAlive => Core.CriteriaExpression.CustomLeaderboardCriteriaType.SpiderEggAliveCount,
				CustomLeaderboardCriteriaType.Squid1sAlive => Core.CriteriaExpression.CustomLeaderboardCriteriaType.Squid1AliveCount,
				CustomLeaderboardCriteriaType.Squid2sAlive => Core.CriteriaExpression.CustomLeaderboardCriteriaType.Squid2AliveCount,
				CustomLeaderboardCriteriaType.Squid3sAlive => Core.CriteriaExpression.CustomLeaderboardCriteriaType.Squid3AliveCount,
				CustomLeaderboardCriteriaType.CentipedesAlive => Core.CriteriaExpression.CustomLeaderboardCriteriaType.CentipedeAliveCount,
				CustomLeaderboardCriteriaType.GigapedesAlive => Core.CriteriaExpression.CustomLeaderboardCriteriaType.GigapedeAliveCount,
				CustomLeaderboardCriteriaType.GhostpedesAlive => Core.CriteriaExpression.CustomLeaderboardCriteriaType.GhostpedeAliveCount,
				CustomLeaderboardCriteriaType.Spider1sAlive => Core.CriteriaExpression.CustomLeaderboardCriteriaType.Spider1AliveCount,
				CustomLeaderboardCriteriaType.Spider2sAlive => Core.CriteriaExpression.CustomLeaderboardCriteriaType.Spider2AliveCount,
				CustomLeaderboardCriteriaType.LeviathansAlive => Core.CriteriaExpression.CustomLeaderboardCriteriaType.LeviathanAliveCount,
				CustomLeaderboardCriteriaType.OrbsAlive => Core.CriteriaExpression.CustomLeaderboardCriteriaType.OrbAliveCount,
				CustomLeaderboardCriteriaType.ThornsAlive => Core.CriteriaExpression.CustomLeaderboardCriteriaType.ThornAliveCount,
				CustomLeaderboardCriteriaType.DeathType => Core.CriteriaExpression.CustomLeaderboardCriteriaType.DeathType,
				CustomLeaderboardCriteriaType.Time => Core.CriteriaExpression.CustomLeaderboardCriteriaType.Time,
				CustomLeaderboardCriteriaType.LevelUpTime2 => Core.CriteriaExpression.CustomLeaderboardCriteriaType.LevelUpTime2,
				CustomLeaderboardCriteriaType.LevelUpTime3 => Core.CriteriaExpression.CustomLeaderboardCriteriaType.LevelUpTime3,
				CustomLeaderboardCriteriaType.LevelUpTime4 => Core.CriteriaExpression.CustomLeaderboardCriteriaType.LevelUpTime4,
				CustomLeaderboardCriteriaType.EnemiesAlive => Core.CriteriaExpression.CustomLeaderboardCriteriaType.EnemiesAlive,
				_ => throw new UnreachableException(),
			};
		}
	}
}
