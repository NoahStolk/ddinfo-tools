using DevilDaggersInfo.Core.Asset;
using System.Text.Json.Serialization;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor.Data;

public record TextureAssetPath(string AssetName, string? AbsolutePath) : IAssetPath
{
	[JsonIgnore]
	public AssetType AssetType => AssetType.Texture;

	public string? AbsolutePath { get; private set; } = AbsolutePath;

	public void SetPath(string? path)
	{
		AbsolutePath = path;
	}
}
