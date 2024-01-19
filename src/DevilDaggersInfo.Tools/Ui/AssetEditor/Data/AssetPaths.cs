using DevilDaggersInfo.Tools.JsonSerializerContexts;
using DevilDaggersInfo.Tools.Utils;
using System.Text.Json;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor.Data;

public class AssetPaths
{
	public List<AssetPath> Paths { get; init; } = [];

	public void Sort(uint sorting, bool sortAscending)
	{
		Paths.Sort((a, b) =>
		{
			int result = sorting switch
			{
				0 => string.Compare(EnumUtils.AssetTypeNames[a.AssetType], EnumUtils.AssetTypeNames[b.AssetType], StringComparison.OrdinalIgnoreCase),
				1 => string.Compare(a.AssetName, b.AssetName, StringComparison.OrdinalIgnoreCase),
				2 => string.Compare(a.AbsolutePath, b.AbsolutePath, StringComparison.OrdinalIgnoreCase),
				_ => throw new InvalidOperationException($"Invalid sorting value {sorting}."),
			};

			return sortAscending ? result : -result;
		});
	}

	public byte[] ToJsonBytes()
	{
		return JsonSerializer.SerializeToUtf8Bytes(this, AssetPathsContext.Default.AssetPaths);
	}
}
