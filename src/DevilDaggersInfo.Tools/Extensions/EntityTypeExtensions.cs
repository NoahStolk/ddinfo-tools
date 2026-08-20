using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Core.Wiki;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;

namespace DevilDaggersInfo.Tools.Extensions;

internal static class EntityTypeExtensions
{
	public static Color GetColor(this EntityType? entityType)
	{
		return entityType?.GetColor() ?? new Color(191, 0, 255, 255);
	}

	extension(EntityType entityType)
	{
		public Color GetColor()
		{
			return (entityType switch
			{
				EntityType.Level1Dagger => UpgradesV3_2.Level1.Color,
				EntityType.Level2Dagger => UpgradesV3_2.Level2.Color,
				EntityType.Level3Dagger => new Core.Wiki.Structs.Color(0xFF, 0xDD, 0x00),
				EntityType.Level3HomingDagger => UpgradesV3_2.Level3.Color,
				EntityType.Level4Dagger => new Core.Wiki.Structs.Color(0xBB, 0x00, 0x66),
				EntityType.Level4HomingDagger => UpgradesV3_2.Level4.Color,
				EntityType.Level4HomingSplash => new Core.Wiki.Structs.Color(0xFF, 0x77, 0xFF),
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
				_ => new Core.Wiki.Structs.Color(191, 0, 255),
			}).ToEngineColor();
		}

		public ReadOnlySpan<byte> AsUtf8ShortSpan()
		{
			return entityType switch
			{
				EntityType.Level1Dagger => "Lvl1"u8,
				EntityType.Level2Dagger => "Lvl2"u8,
				EntityType.Level3Dagger => "Lvl3"u8,
				EntityType.Level3HomingDagger => "Lvl3 Homing"u8,
				EntityType.Level4Dagger => "Lvl4"u8,
				EntityType.Level4HomingDagger => "Lvl4 Homing"u8,
				EntityType.Level4HomingSplash => "Lvl4 Splash"u8,
				EntityType.Squid1 => "Squid I"u8,
				EntityType.Squid2 => "Squid II"u8,
				EntityType.Squid3 => "Squid III"u8,
				EntityType.Skull1 => "Skull I"u8,
				EntityType.Skull2 => "Skull II"u8,
				EntityType.Skull3 => "Skull III"u8,
				EntityType.Spiderling => "Spiderling"u8,
				EntityType.Skull4 => "Skull IV"u8,
				EntityType.Centipede => "Centipede"u8,
				EntityType.Gigapede => "Gigapede"u8,
				EntityType.Ghostpede => "Ghostpede"u8,
				EntityType.Spider1 => "Spider I"u8,
				EntityType.Spider2 => "Spider II"u8,
				EntityType.SpiderEgg => "Spider Egg"u8,
				EntityType.Leviathan => "Leviathan"u8,
				EntityType.Thorn => "Thorn"u8,
				EntityType.Zero => "Zero"u8,
				_ => throw new ArgumentOutOfRangeException(nameof(entityType), entityType, null),
			};
		}
	}
}
