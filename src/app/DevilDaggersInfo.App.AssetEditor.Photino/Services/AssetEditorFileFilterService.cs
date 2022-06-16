using DevilDaggersInfo.Common.Exceptions;
using DevilDaggersInfo.Core.Asset.Enums;
using DevilDaggersInfo.Core.Asset.Extensions;
using DevilDaggersInfo.Razor.AssetEditor.Services;

namespace DevilDaggersInfo.App.AssetEditor.Photino.Services;

public class AssetEditorFileFilterService : IAssetEditorFileFilterService
{
	public string GetAssetTypeFilter(AssetType assetType)
	{
		if (assetType == AssetType.Shader)
			throw new NotSupportedException($"Asset type '{AssetType.Shader}' has multiple file extensions.");

		string fileTypeName = assetType switch
		{
			AssetType.Audio => "Audio",
			AssetType.Mesh => "Mesh",
			AssetType.ObjectBinding => "Text",
			AssetType.Texture => "Texture",
			_ => throw new InvalidEnumConversionException(assetType),
		};

		return BuildFilter(fileTypeName, false, $"*{assetType.GetFileExtension()}");
	}

	public string GetVertexShaderFilter() => BuildFilter("Vertex shader", true, "*.vert", "*.glsl");

	public string GetFragmentShaderFilter() => BuildFilter("Fragment shader", true, "*.frag", "*.glsl");

	private static string BuildFilter(string fileTypeName, bool includeAllFiles, params string[] fileExtensionPatterns)
	{
		string patterns = string.Join(";", fileExtensionPatterns);
		return $"{fileTypeName} files ({patterns})|{patterns}{(includeAllFiles ? "|All files (*.*)|*.*" : null)}";
	}
}
