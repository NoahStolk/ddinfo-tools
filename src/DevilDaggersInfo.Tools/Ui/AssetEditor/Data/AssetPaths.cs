using DevilDaggersInfo.Tools.JsonSerializerContexts;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor.Data;

public class AssetPaths
{
	[JsonRequired]
	public List<AudioAssetPath> Audio { get; init; } = [];

	[JsonRequired]
	public List<MeshAssetPath> Meshes { get; init; } = [];

	[JsonRequired]
	public List<ObjectBindingAssetPath> ObjectBindings { get; init; } = [];

	[JsonRequired]
	public List<ShaderAssetPath> Shaders { get; init; } = [];

	[JsonRequired]
	public List<TextureAssetPath> Textures { get; init; } = [];

	public byte[] ToJsonBytes()
	{
		return JsonSerializer.SerializeToUtf8Bytes(this, AssetPathsContext.Default.AssetPaths);
	}
}
