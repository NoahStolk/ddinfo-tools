using DevilDaggersInfo.Core.Wiki;
using DevilDaggersInfo.Core.Wiki.Objects;
using DevilDaggersInfo.Types.Core.Spawnsets;

namespace DevilDaggersInfo.App.Ui.Base.Extensions;

public static class EnemyTypeExtensions
{
	public static Enemy? GetEnemy(this EnemyType enemyType) => enemyType switch
	{
		EnemyType.Squid1 => EnemiesV3_2.Squid1,
		EnemyType.Squid2 => EnemiesV3_2.Squid2,
		EnemyType.Centipede => EnemiesV3_2.Centipede,
		EnemyType.Spider1 => EnemiesV3_2.Spider1,
		EnemyType.Leviathan => EnemiesV3_2.Leviathan,
		EnemyType.Gigapede => EnemiesV3_2.Gigapede,
		EnemyType.Squid3 => EnemiesV3_2.Squid3,
		EnemyType.Thorn => EnemiesV3_2.Thorn,
		EnemyType.Spider2 => EnemiesV3_2.Spider2,
		EnemyType.Ghostpede => EnemiesV3_2.Ghostpede,
		_ => null,
	};

	public static string GetShortName(this EnemyType enemyType) => enemyType switch
	{
		EnemyType.Squid1 => "SQUID1",
		EnemyType.Squid2 => "SQUID2",
		EnemyType.Centipede => "CENTI",
		EnemyType.Spider1 => "SPIDER1",
		EnemyType.Leviathan => "LEVI",
		EnemyType.Gigapede => "GIGA",
		EnemyType.Squid3 => "SQUID3",
		EnemyType.Thorn => "THORN",
		EnemyType.Spider2 => "SPIDER2",
		EnemyType.Ghostpede => "GHOST",
		_ => "EMPTY",
	};
}
