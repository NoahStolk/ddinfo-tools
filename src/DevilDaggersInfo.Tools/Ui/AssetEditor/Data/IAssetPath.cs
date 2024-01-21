using DevilDaggersInfo.Core.Asset;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor.Data;

public interface IAssetPath
{
	AssetType AssetType { get; }

	string AssetName { get; }
}
