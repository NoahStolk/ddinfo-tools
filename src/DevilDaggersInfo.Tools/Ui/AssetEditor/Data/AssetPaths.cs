using DevilDaggersInfo.Tools.JsonSerializerContexts;
using System.Text.Json;

namespace DevilDaggersInfo.Tools.Ui.AssetEditor.Data;

public class AssetPaths
{
	public List<AssetPath> Paths { get; init; } = [];

	public byte[] ToJsonBytes()
	{
		return JsonSerializer.SerializeToUtf8Bytes(this, AssetPathsContext.Default.AssetPaths);
	}
}
