using DevilDaggersInfo.Core.Asset;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor.Data;

public record ShaderAssetPath(AssetType AssetType, string AssetName, string? AbsoluteVertexPath, string? AbsoluteFragmentPath) : IAssetPath
{
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
