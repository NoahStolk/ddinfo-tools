using DevilDaggersInfo.Core.Asset;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor.Data;

public record ShaderAssetPath(AssetType AssetType, string AssetName, string? AbsoluteVertexPath, string? AbsoluteFragmentPath) : IAssetPath
{
	public string? AbsoluteVertexPath { get; private set; } = AbsoluteVertexPath;

	public string? AbsoluteFragmentPath { get; private set; } = AbsoluteFragmentPath;

	public void SetVertexPath(string? path)
	{
		if (path != null)
			AbsoluteVertexPath = path;
	}

	public void SetFragmentPath(string? path)
	{
		if (path != null)
			AbsoluteFragmentPath = path;
	}
}
