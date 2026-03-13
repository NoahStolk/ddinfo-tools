using DevilDaggersInfo.Core.Asset;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor.Data;

internal interface IAssetPath
{
	AssetType AssetType { get; }

	string AssetName { get; }
}
