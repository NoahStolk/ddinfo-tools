using DevilDaggersInfo.Core.Asset;
using System.Text.Json.Serialization;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor.Data;

public record ShaderAssetPath(string AssetName, string? AbsoluteVertexPath, string? AbsoluteFragmentPath) : IAssetPath
{
	[JsonIgnore]
	public AssetType AssetType => AssetType.Shader;

	public string? AbsoluteVertexPath { get; private set; } = AbsoluteVertexPath;

	public string? AbsoluteFragmentPath { get; private set; } = AbsoluteFragmentPath;

	public void SetVertexPath(string? path)
	{
		AbsoluteVertexPath = path;
	}

	public void SetFragmentPath(string? path)
	{
		AbsoluteFragmentPath = path;
	}
}
