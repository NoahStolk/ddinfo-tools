using DevilDaggersInfo.Core.Asset;
using DevilDaggersInfo.Core.Mod;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Core.Spawnset.Extensions;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Events;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Arena;

namespace DevilDaggersInfo.Tools.Utils;

public static class EnumUtils
{
	public static readonly IReadOnlyList<HandLevel> HandLevels = Enum.GetValues<HandLevel>();
	public static readonly IReadOnlyDictionary<HandLevel, string> HandLevelNames = HandLevels.ToDictionary(h => h, h => h.ToString());

	public static readonly IReadOnlyList<ArenaTool> ArenaTools = Enum.GetValues<ArenaTool>();
	public static readonly IReadOnlyDictionary<ArenaTool, string> ArenaToolNames = ArenaTools.ToDictionary(at => at, at => at.ToString());

	public static readonly IReadOnlyList<EnemyType> EnemyTypes = Enum.GetValues<EnemyType>();
	public static readonly IReadOnlyDictionary<EnemyType, string> EnemyTypeNames = EnemyTypes.ToDictionary(et => et, et => et.ToString());

	public static readonly IReadOnlyList<SpawnsetSupportedGameVersion> SpawnsetSupportedGameVersions = Enum.GetValues<SpawnsetSupportedGameVersion>();
	public static readonly IReadOnlyDictionary<SpawnsetSupportedGameVersion, string> SpawnsetSupportedGameVersionNames = SpawnsetSupportedGameVersions.ToDictionary(gv => gv, gv => gv.ToDisplayString());

	public static readonly IReadOnlyList<GameMode> GameModes = Enum.GetValues<GameMode>();
	public static readonly IReadOnlyDictionary<GameMode, string> GameModeNames = GameModes.ToDictionary(gm => gm, gm => gm.ToString());

	public static readonly IReadOnlyList<EntityType> EntityTypes = Enum.GetValues<EntityType>();
	public static readonly IReadOnlyDictionary<EntityType, string> EntityTypeShortNames = new Dictionary<EntityType, string>
	{
		[EntityType.Level1Dagger] = "Lvl1",
		[EntityType.Level2Dagger] = "Lvl2",
		[EntityType.Level3Dagger] = "Lvl3",
		[EntityType.Level3HomingDagger] = "Lvl3 Homing",
		[EntityType.Level4Dagger] = "Lvl4",
		[EntityType.Level4HomingDagger] = "Lvl4 Homing",
		[EntityType.Level4HomingSplash] = "Lvl4 Splash",
		[EntityType.Squid1] = "Squid I",
		[EntityType.Squid2] = "Squid II",
		[EntityType.Squid3] = "Squid III",
		[EntityType.Skull1] = "Skull I",
		[EntityType.Skull2] = "Skull II",
		[EntityType.Skull3] = "Skull III",
		[EntityType.Spiderling] = "Spiderling",
		[EntityType.Skull4] = "Skull IV",
		[EntityType.Centipede] = "Centipede",
		[EntityType.Gigapede] = "Gigapede",
		[EntityType.Ghostpede] = "Ghostpede",
		[EntityType.Spider1] = "Spider I",
		[EntityType.Spider2] = "Spider II",
		[EntityType.SpiderEgg] = "Spider Egg",
		[EntityType.Leviathan] = "Leviathan",
		[EntityType.Thorn] = "Thorn",
		[EntityType.Zero] = "Zero",
	};

	public static readonly IReadOnlyList<DaggerType> DaggerTypes = Enum.GetValues<DaggerType>();
	public static readonly IReadOnlyDictionary<DaggerType, string> DaggerTypeNames = DaggerTypes.ToDictionary(dt => dt, dt => dt.ToString());

	public static readonly IReadOnlyList<EventType> EventTypes = Enum.GetValues<EventType>();
	public static readonly IReadOnlyDictionary<EventType, string> EventTypeNames = EventTypes.ToDictionary(et => et, et => et.ToString());

	public static readonly IReadOnlyList<ModBinaryType> ModBinaryTypes = Enum.GetValues<ModBinaryType>();
	public static readonly IReadOnlyDictionary<ModBinaryType, string> ModBinaryTypeNames = ModBinaryTypes.ToDictionary(mbt => mbt, mbt => mbt.ToString().ToLower());

	public static readonly IReadOnlyList<AssetType> AssetTypes = Enum.GetValues<AssetType>();
	public static readonly IReadOnlyDictionary<AssetType, string> AssetTypeNames = AssetTypes.ToDictionary(at => at, at => at.ToString());

	public static readonly IReadOnlyList<BoidType> BoidTypes = Enum.GetValues<BoidType>();
	public static readonly IReadOnlyDictionary<BoidType, string> BoidTypeNames = BoidTypes.ToDictionary(bt => bt, bt => bt.ToString());

	public static readonly IReadOnlyList<PedeType> PedeTypes = Enum.GetValues<PedeType>();
	public static readonly IReadOnlyDictionary<PedeType, string> PedeTypeNames = PedeTypes.ToDictionary(pt => pt, pt => pt.ToString());

	public static readonly IReadOnlyList<SpiderType> SpiderTypes = Enum.GetValues<SpiderType>();
	public static readonly IReadOnlyDictionary<SpiderType, string> SpiderTypeNames = SpiderTypes.ToDictionary(st => st, st => st.ToString());

	public static readonly IReadOnlyList<SquidType> SquidTypes = Enum.GetValues<SquidType>();
	public static readonly IReadOnlyDictionary<SquidType, string> SquidTypeNames = SquidTypes.ToDictionary(st => st, st => st.ToString());

	public static readonly IReadOnlyList<JumpType> JumpTypes = Enum.GetValues<JumpType>();
	public static readonly IReadOnlyDictionary<JumpType, string> JumpTypeNames = JumpTypes.ToDictionary(jt => jt, jt => jt.ToString());

	public static readonly IReadOnlyList<ShootType> ShootTypes = Enum.GetValues<ShootType>();
	public static readonly IReadOnlyDictionary<ShootType, string> ShootTypeNames = ShootTypes.ToDictionary(st => st, st => st.ToString());
}
