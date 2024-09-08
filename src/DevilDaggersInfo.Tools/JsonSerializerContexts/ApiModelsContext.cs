using DevilDaggersInfo.Web.ApiSpec.Tools.CustomLeaderboards;
using DevilDaggersInfo.Web.ApiSpec.Tools.ProcessMemory;
using DevilDaggersInfo.Web.ApiSpec.Tools.Spawnsets;
using System.Text.Json.Serialization;

namespace DevilDaggersInfo.Tools.JsonSerializerContexts;

[JsonSourceGenerationOptions(
	WriteIndented = false,
	PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
	GenerationMode = JsonSourceGenerationMode.Default)]
[JsonSerializable(typeof(AddUploadRequest))]
[JsonSerializable(typeof(GetCustomEntryReplayBuffer))]
[JsonSerializable(typeof(GetCustomLeaderboard))]
[JsonSerializable(typeof(GetMarker))]
[JsonSerializable(typeof(GetSpawnset))]
[JsonSerializable(typeof(GetSpawnsetByHash))]
[JsonSerializable(typeof(GetUploadResponse))]
[JsonSerializable(typeof(List<GetCustomLeaderboardAllowedCategory>))]
[JsonSerializable(typeof(List<GetCustomLeaderboardForOverview>))]
public partial class ApiModelsContext : JsonSerializerContext;
