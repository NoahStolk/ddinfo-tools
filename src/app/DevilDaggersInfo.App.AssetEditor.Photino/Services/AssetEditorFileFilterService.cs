using DevilDaggersInfo.App.Core.NativeInterface.Utils;
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

		return FileDialogFilterUtils.BuildFilter(fileTypeName, false, $"*{assetType.GetFileExtension()}");
	}

	public string GetVertexShaderFilter() => FileDialogFilterUtils.BuildFilter("Vertex shader", true, "*.vert", "*.glsl");

	public string GetFragmentShaderFilter() => FileDialogFilterUtils.BuildFilter("Fragment shader", true, "*.frag", "*.glsl");
}
