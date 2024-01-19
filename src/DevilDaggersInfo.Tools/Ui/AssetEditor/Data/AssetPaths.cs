using DevilDaggersInfo.Tools.JsonSerializerContexts;
using DevilDaggersInfo.Tools.Utils;
using System.Text.Json;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor.Data;

public class AssetPaths
{
	public List<AudioAssetPath> Audio { get; init; } = [];
	public List<AssetPath> Meshes { get; init; } = [];
	public List<AssetPath> ObjectBindings { get; init; } = [];
	public List<ShaderAssetPath> Shaders { get; init; } = [];
	public List<AssetPath> Textures { get; init; } = [];

	public void SortAudio(uint sorting, bool sortAscending)
	{
		Audio.Sort((a, b) => CompareAudioAssets(a, b, sorting, sortAscending));
	}

	public void SortMeshes(uint sorting, bool sortAscending)
	{
		Meshes.Sort((a, b) => CompareAssets(a, b, sorting, sortAscending));
	}

	public void SortObjectBindings(uint sorting, bool sortAscending)
	{
		ObjectBindings.Sort((a, b) => CompareAssets(a, b, sorting, sortAscending));
	}

	public void SortShaders(uint sorting, bool sortAscending)
	{
		Shaders.Sort((a, b) => CompareShaderAssets(a, b, sorting, sortAscending));
	}

	public void SortTextures(uint sorting, bool sortAscending)
	{
		Textures.Sort((a, b) => CompareAssets(a, b, sorting, sortAscending));
	}

	private static int CompareAudioAssets(AudioAssetPath a, AudioAssetPath b, uint sorting, bool sortAscending)
	{
		int result = sorting switch
		{
			0 => string.Compare(EnumUtils.AssetTypeNames[a.AssetType], EnumUtils.AssetTypeNames[b.AssetType], StringComparison.OrdinalIgnoreCase),
			1 => string.Compare(a.AssetName, b.AssetName, StringComparison.OrdinalIgnoreCase),
			2 => string.Compare(a.AbsolutePath, b.AbsolutePath, StringComparison.OrdinalIgnoreCase),
			3 => a.Loudness.HasValue && b.Loudness.HasValue ? a.Loudness.Value.CompareTo(b.Loudness.Value) : 0, // TODO: Test this.
			_ => throw new InvalidOperationException($"Invalid sorting value {sorting}."),
		};

		return sortAscending ? result : -result;
	}

	private static int CompareShaderAssets(ShaderAssetPath a, ShaderAssetPath b, uint sorting, bool sortAscending)
	{
		int result = sorting switch
		{
			0 => string.Compare(EnumUtils.AssetTypeNames[a.AssetType], EnumUtils.AssetTypeNames[b.AssetType], StringComparison.OrdinalIgnoreCase),
			1 => string.Compare(a.AssetName, b.AssetName, StringComparison.OrdinalIgnoreCase),
			2 => string.Compare(a.AbsoluteVertexPath, b.AbsoluteVertexPath, StringComparison.OrdinalIgnoreCase),
			3 => string.Compare(a.AbsoluteFragmentPath, b.AbsoluteFragmentPath, StringComparison.OrdinalIgnoreCase),
			_ => throw new InvalidOperationException($"Invalid sorting value {sorting}."),
		};

		return sortAscending ? result : -result;
	}

	private static int CompareAssets(AssetPath a, AssetPath b, uint sorting, bool sortAscending)
	{
		int result = sorting switch
		{
			0 => string.Compare(EnumUtils.AssetTypeNames[a.AssetType], EnumUtils.AssetTypeNames[b.AssetType], StringComparison.OrdinalIgnoreCase),
			1 => string.Compare(a.AssetName, b.AssetName, StringComparison.OrdinalIgnoreCase),
			2 => string.Compare(a.AbsolutePath, b.AbsolutePath, StringComparison.OrdinalIgnoreCase),
			_ => throw new InvalidOperationException($"Invalid sorting value {sorting}."),
		};

		return sortAscending ? result : -result;
	}

	public byte[] ToJsonBytes()
	{
		return JsonSerializer.SerializeToUtf8Bytes(this, AssetPathsContext.Default.AssetPaths);
	}
}
