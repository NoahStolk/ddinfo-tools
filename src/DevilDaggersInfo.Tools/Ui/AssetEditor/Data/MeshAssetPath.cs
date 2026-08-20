namespace DevilDaggersInfo.Tools.Ui.AssetEditor.Data;

internal sealed record MeshAssetPath(string AssetName, string? AbsolutePath) : IAssetPath
{
	public string? AbsolutePath { get; private set; } = AbsolutePath;

	public void SetPath(string? path)
	{
		AbsolutePath = path;
	}
}
