using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Core.Wiki;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;

namespace DevilDaggersInfo.Tools.Extensions;

public static class EntityTypeExtensions
{
	public static Color GetColor(this EntityType? entityType)
	{
		return !entityType.HasValue ? new(191, 0, 255, 255) : GetColor(entityType.Value);
	}

	public static Color GetColor(this EntityType entityType)
	{
		return (entityType switch
		{
			EntityType.Level1Dagger => UpgradesV3_2.Level1.Color,
			EntityType.Level2Dagger => UpgradesV3_2.Level2.Color,
			EntityType.Level3Dagger => new(0xFF, 0xDD, 0x00),
			EntityType.Level3HomingDagger => UpgradesV3_2.Level3.Color,
			EntityType.Level4Dagger => new(0xBB, 0x00, 0x66),
			EntityType.Level4HomingDagger => UpgradesV3_2.Level4.Color,
			EntityType.Level4HomingSplash => new(0xFF, 0x77, 0xFF),
			EntityType.Squid1 => EnemiesV3_2.Squid1.Color,
			EntityType.Squid2 => EnemiesV3_2.Squid2.Color,
			EntityType.Squid3 => EnemiesV3_2.Squid3.Color,
			EntityType.Skull1 => EnemiesV3_2.Skull1.Color,
			EntityType.Skull2 => EnemiesV3_2.Skull2.Color,
			EntityType.Skull3 => EnemiesV3_2.Skull3.Color,
			EntityType.Spiderling => EnemiesV3_2.Spiderling.Color,
			EntityType.Skull4 => EnemiesV3_2.Skull4.Color,
			EntityType.Centipede => EnemiesV3_2.Centipede.Color,
			EntityType.Gigapede => EnemiesV3_2.Gigapede.Color,
			EntityType.Ghostpede => EnemiesV3_2.Ghostpede.Color,
			EntityType.Spider1 => EnemiesV3_2.Spider1.Color,
			EntityType.Spider2 => EnemiesV3_2.Spider2.Color,
			EntityType.SpiderEgg => EnemiesV3_2.SpiderEgg1.Color,
			EntityType.Leviathan => EnemiesV3_2.Leviathan.Color,
			EntityType.Thorn => EnemiesV3_2.Thorn.Color,
			_ => new(191, 0, 255),
		}).ToEngineColor();
	}
}
