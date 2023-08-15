using DevilDaggersInfo.App.Ui.SpawnsetEditor.Arena;
using DevilDaggersInfo.Core.Spawnset;

namespace DevilDaggersInfo.App;

public static class EnumUtils
{
	public static readonly IReadOnlyList<HandLevel> HandLevels = Enum.GetValues<HandLevel>();
	public static readonly IReadOnlyDictionary<HandLevel, string> HandLevelNames = HandLevels.ToDictionary(h => h, h => h.ToString());

	public static readonly IReadOnlyList<ArenaTool> ArenaTools = Enum.GetValues<ArenaTool>();
	public static readonly IReadOnlyDictionary<ArenaTool, string> ArenaToolNames = ArenaTools.ToDictionary(at => at, at => at.ToString());

	public static readonly IReadOnlyList<EnemyType> EnemyTypes = Enum.GetValues<EnemyType>();
	public static readonly IReadOnlyDictionary<EnemyType, string> EnemyTypeNames = EnemyTypes.ToDictionary(et => et, et => et.ToString());
}
