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
		for (int i = Audio.Count - 1; i >= 0; i--)
		{
			if (Audio[i].AbsolutePath == null && Audio[i].Loudness == null)
				Audio.RemoveAt(i);
		}

		for (int i = Meshes.Count - 1; i >= 0; i--)
		{
			if (Meshes[i].AbsolutePath == null)
				Meshes.RemoveAt(i);
		}

		for (int i = ObjectBindings.Count - 1; i >= 0; i--)
		{
			if (ObjectBindings[i].AbsolutePath == null)
				ObjectBindings.RemoveAt(i);
		}

		for (int i = Shaders.Count - 1; i >= 0; i--)
		{
			if (Shaders[i].AbsoluteVertexPath == null && Shaders[i].AbsoluteFragmentPath == null)
				Shaders.RemoveAt(i);
		}

		for (int i = Textures.Count - 1; i >= 0; i--)
		{
			if (Textures[i].AbsolutePath == null)
				Textures.RemoveAt(i);
		}

		Audio.Sort((a, b) => string.CompareOrdinal(a.AssetName, b.AssetName));
		Meshes.Sort((a, b) => string.CompareOrdinal(a.AssetName, b.AssetName));
		ObjectBindings.Sort((a, b) => string.CompareOrdinal(a.AssetName, b.AssetName));
		Shaders.Sort((a, b) => string.CompareOrdinal(a.AssetName, b.AssetName));
		Textures.Sort((a, b) => string.CompareOrdinal(a.AssetName, b.AssetName));

		return JsonSerializer.SerializeToUtf8Bytes(this, AssetPathsContext.Default.AssetPaths);
	}
}
