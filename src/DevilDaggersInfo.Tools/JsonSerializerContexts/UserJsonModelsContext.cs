using DevilDaggersInfo.Tools.User.Cache.Model;
using DevilDaggersInfo.Tools.User.Settings.Model;
using System.Text.Json.Serialization;

namespace DevilDaggersInfo.Tools.JsonSerializerContexts;

[JsonSourceGenerationOptions(
	WriteIndented = false,
	PropertyNamingPolicy = JsonKnownNamingPolicy.Unspecified,
	GenerationMode = JsonSourceGenerationMode.Default)]
[JsonSerializable(typeof(UserCacheModel))]
[JsonSerializable(typeof(UserSettingsModel))]
public partial class UserJsonModelsContext : JsonSerializerContext;
