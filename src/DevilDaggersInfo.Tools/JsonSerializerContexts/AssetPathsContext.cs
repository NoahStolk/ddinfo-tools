using DevilDaggersInfo.Tools.Ui.AssetEditor.Data;
using System.Text.Json.Serialization;

namespace DevilDaggersInfo.Tools.JsonSerializerContexts;

[JsonSourceGenerationOptions(
	WriteIndented = true,
	PropertyNamingPolicy = JsonKnownNamingPolicy.Unspecified,
	UseStringEnumConverter = true,
	GenerationMode = JsonSourceGenerationMode.Default)]
[JsonSerializable(typeof(AssetPaths))]
internal partial class AssetPathsContext : JsonSerializerContext;
