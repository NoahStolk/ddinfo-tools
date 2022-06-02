using DevilDaggersInfo.Common.Exceptions;
using DevilDaggersInfo.Core.Asset.Enums;
using DevilDaggersInfo.Core.Asset.Extensions;
using DevilDaggersInfo.Razor.Core.AssetEditor.Services;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System.IO;

namespace DevilDaggersInfo.AssetEditor.Wpf.Services;

/// <summary>
/// Platform-specific code for interacting with the OS file system.
/// </summary>
public class FileSystemService : IFileSystemService
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

	public IFileSystemService.FileResult? Open(string extensionFilter)
	{
		OpenFileDialog dialog = new()
		{
			Filter = extensionFilter,
		};
		bool? openResult = dialog.ShowDialog();
		if (!openResult.HasValue || !openResult.Value)
			return null;

		return new(dialog.FileName, File.ReadAllBytes(dialog.FileName));
	}

	public void Save(byte[] buffer)
	{
		SaveFileDialog dialog = new();
		bool? saveResult = dialog.ShowDialog();
		if (!saveResult.HasValue || !saveResult.Value)
			return;

		File.WriteAllBytes(dialog.FileName, buffer);
	}

	public string? SelectDirectory()
	{
		VistaFolderBrowserDialog folderDialog = new();
		if (folderDialog.ShowDialog() == true)
			return folderDialog.SelectedPath;

		return null;
	}
}
