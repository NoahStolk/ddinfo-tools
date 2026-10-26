using DevilDaggersInfo.Core.Asset;
using System.Diagnostics;

namespace DevilDaggersInfo.Tools.Utils;

internal static class PathUtils
{
	public const string FileExtensionReplay = "ddreplay";
	public const string FileExtensionMod = "json";

	private const string _fileExtensionAudio = "wav";
	private const string _fileExtensionMesh = "obj";
	private const string _fileExtensionObjectBinding = "txt";
	private const string _fileExtensionShaderFragment = "frag";
	private const string _fileExtensionShaderVertex = "vert";
	private const string _fileExtensionShaderGeneric = "glsl";
	private const string _fileExtensionTexture = "png";

	public static string GetFileFilter(AssetType assetType)
	{
		return assetType switch
		{
			AssetType.Audio => _fileExtensionAudio,
			AssetType.Mesh => _fileExtensionMesh,
			AssetType.ObjectBinding => _fileExtensionObjectBinding,
			AssetType.Shader => string.Join(',', _fileExtensionShaderFragment, _fileExtensionShaderVertex, _fileExtensionShaderGeneric),
			AssetType.Texture => _fileExtensionTexture,
			_ => throw new UnreachableException($"Unknown asset type {assetType}."),
		};
	}
}
