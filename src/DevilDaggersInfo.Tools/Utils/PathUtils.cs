using DevilDaggersInfo.Core.Asset;
using System.Diagnostics;

namespace DevilDaggersInfo.Tools.Utils;

public static class PathUtils
{
	public const string FileExtensionReplay = "ddreplay";
	public const string FileExtensionMod = "json";
	public const string FileExtensionAudio = "wav";
	public const string FileExtensionMesh = "obj";
	public const string FileExtensionObjectBinding = "txt";
	public const string FileExtensionShaderFragment = "frag";
	public const string FileExtensionShaderVertex = "vert";
	public const string FileExtensionShaderGeneric = "glsl";
	public const string FileExtensionTexture = "png";

	public static string GetFileFilter(AssetType assetType)
	{
		return assetType switch
		{
			AssetType.Audio => FileExtensionAudio,
			AssetType.Mesh => FileExtensionMesh,
			AssetType.ObjectBinding => FileExtensionObjectBinding,
			AssetType.Shader => string.Join(',', FileExtensionShaderFragment, FileExtensionShaderVertex, FileExtensionShaderGeneric),
			AssetType.Texture => FileExtensionTexture,
			_ => throw new UnreachableException($"Unknown asset type {assetType}."),
		};
	}
}
