using DevilDaggersInfo.App.Ui.Practice.RunAnalysis.Data;
using ImGuiNET;

namespace DevilDaggersInfo.App.Ui.Practice.RunAnalysis;

public static class GemCollectionChild
{
	private static readonly List<int> _gemsCollectedDelta = new();
	private static readonly List<int> _gemsDespawnedDelta = new();
	private static readonly List<int> _gemsEatenDelta = new();
	private static readonly List<int> _gemsTotalDelta = new();

	private static int _maxGemsCollectedDelta;
	private static int _maxGemsDespawnedDelta;
	private static int _maxGemsEatenDelta;
	private static int _maxGemsTotalDelta;

	public static void Update(IReadOnlyList<StatisticEntry> data)
	{
		_gemsCollectedDelta.Clear();
		_gemsDespawnedDelta.Clear();
		_gemsEatenDelta.Clear();
		_gemsTotalDelta.Clear();

		for (int i = 0; i < data.Count; i++)
		{
			_gemsCollectedDelta.Add(data[i].GemsCollected - data[Math.Max(i - 1, 0)].GemsCollected);
			_gemsDespawnedDelta.Add(data[i].GemsDespawned - data[Math.Max(i - 1, 0)].GemsDespawned);
			_gemsEatenDelta.Add(data[i].GemsEaten - data[Math.Max(i - 1, 0)].GemsEaten);
			_gemsTotalDelta.Add(data[i].GemsTotal - data[Math.Max(i - 1, 0)].GemsTotal);
		}

		_maxGemsCollectedDelta = _gemsCollectedDelta.Count > 0 ? _gemsCollectedDelta.Max() : 0;
		_maxGemsDespawnedDelta = _gemsDespawnedDelta.Count > 0 ? _gemsDespawnedDelta.Max() : 0;
		_maxGemsEatenDelta = _gemsEatenDelta.Count > 0 ? _gemsEatenDelta.Max() : 0;
		_maxGemsTotalDelta = _gemsTotalDelta.Count > 0 ? _gemsTotalDelta.Max() : 0;
	}

	public static unsafe void Render()
	{
		const float height = 120;
		if (ImGui.BeginChild("Gem Collection"))
		{
			fixed (float* f = _gemsCollectedDelta.Select(i => (float)i).ToArray())
				ImGui.PlotLines("Gems collected", ref f[0], _gemsCollectedDelta.Count, 0, null, 0, _maxGemsCollectedDelta, new(0, height));

			fixed (float* f = _gemsDespawnedDelta.Select(i => -(float)i).ToArray())
				ImGui.PlotLines("Gems despawned", ref f[0], _gemsDespawnedDelta.Count, 0, null, -_maxGemsDespawnedDelta, 0, new(0, height));

			fixed (float* f = _gemsEatenDelta.Select(i => -(float)i).ToArray())
				ImGui.PlotLines("Gems eaten", ref f[0], _gemsEatenDelta.Count, 0, null, -_maxGemsEatenDelta, 0, new(0, height));

			fixed (float* f = _gemsTotalDelta.Select(i => (float)i).ToArray())
				ImGui.PlotLines("Gems total", ref f[0], _gemsTotalDelta.Count, 0, null, 0, _maxGemsTotalDelta, new(0, height));

			ImGui.EndChild();
		}
	}
}
