using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Core.Wiki;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Extensions;

internal static class EntityTypeExtensions
{
	public static Vector4 GetColor(this EntityType? entityType)
	{
		return entityType?.GetColor() ?? new Vector4(0.75f, 0, 1, 1);
	}

	extension(EntityType entityType)
	{
		public Vector4 GetColor()
		{
			Vector3 color = entityType switch
			{
				EntityType.Level1Dagger => UpgradesV3_2.Level1.Color,
				EntityType.Level2Dagger => UpgradesV3_2.Level2.Color,
				EntityType.Level3Dagger => new Vector3(1, 0xDD / 255f, 0),
				EntityType.Level3HomingDagger => UpgradesV3_2.Level3.Color,
				EntityType.Level4Dagger => new Vector3(0xBB / 255f, 0, 0x66 / 255f),
				EntityType.Level4HomingDagger => UpgradesV3_2.Level4.Color,
				EntityType.Level4HomingSplash => new Vector3(1, 0x77 / 255f, 1),
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
				_ => new Vector3(191 / 255f, 0, 1),
			};
			return color.ToVector4();
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
