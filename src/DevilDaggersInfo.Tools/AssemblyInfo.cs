using DevilDaggersInfo.Core.Asset;
using DevilDaggersInfo.Core.Mod;
using DevilDaggersInfo.Core.Replay.Events.Enums;
using DevilDaggersInfo.Core.Spawnset;
using EnumGenerator;
using Silk.NET.GLFW;

// Core.Asset
[assembly: GenerateEnumUtilities<AssetType>]

// Core.Mod
[assembly: GenerateEnumUtilities<ModBinaryType>]

// Core.Replay
[assembly: GenerateEnumUtilities<BoidType>]
[assembly: GenerateEnumUtilities<DaggerType>]
[assembly: GenerateEnumUtilities<PedeType>]
[assembly: GenerateEnumUtilities<JumpType>]
[assembly: GenerateEnumUtilities<ShootType>]
[assembly: GenerateEnumUtilities<SpiderType>]
[assembly: GenerateEnumUtilities<SquidType>]

// Core.Spawnset
[assembly: GenerateEnumUtilities<EnemyType>]
[assembly: GenerateEnumUtilities<GameMode>]
[assembly: GenerateEnumUtilities<HandLevel>]
[assembly: GenerateEnumUtilities<SpawnsetSupportedGameVersion>]

// GLFW
[assembly: GenerateEnumUtilities<Keys>]
