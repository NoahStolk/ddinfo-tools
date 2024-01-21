using DevilDaggersInfo.Core.Asset;
using System.Text.Json.Serialization;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor.Data;

public record MeshAssetPath(string AssetName, string? AbsolutePath) : IAssetPath
{
	[JsonIgnore]
	public AssetType AssetType => AssetType.Mesh;

	public string? AbsolutePath { get; private set; } = AbsolutePath;

	public void SetPath(string? path)
	{
		AbsolutePath = path;
	}
}
