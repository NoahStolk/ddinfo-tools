using DevilDaggersInfo.Core.Asset;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Extensions;

public static class AssetTypeExtensions
{
	public static Vector4 GetColor(this AssetType assetType)
	{
		return assetType switch
		{
			AssetType.Audio => new Vector4(1, 0.25f, 1, 1),
			AssetType.ObjectBinding => new Vector4(0.25f, 1, 1, 1),
			AssetType.Mesh => new Vector4(1, 0.25f, 0.25f, 1),
			AssetType.Shader => new Vector4(0.25f, 1, 0.25f, 1),
			AssetType.Texture => new Vector4(1, 0.66f, 0.25f, 1),
			_ => Vector4.One,
		};
	}
}
