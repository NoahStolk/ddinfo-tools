using DevilDaggersInfo.App.Core.GameMemory;

namespace DevilDaggersInfo.App.Ui.Practice.RunAnalysis.Data;

public class PracticeStatsData
{
	private readonly byte[] _statsBuffer = new byte[GameMemoryService.StatsBufferSize * 60 * 60]; // Allow up to an hour of data (roughly 3600 seconds in game).

	private readonly List<StatisticEntry> _statistics = new();

	public SplitsData SplitsData { get; } = new();
	public IReadOnlyList<StatisticEntry> Statistics => _statistics;
	public float TimerStart { get; private set; }
	public float TimerEnd { get; private set; }

	public void Populate()
	{
		_statistics.Clear();

		Array.Clear(_statsBuffer);
		Root.GameMemoryService.GetStatsBuffer(_statsBuffer);
		using MemoryStream ms = new(_statsBuffer);
		using BinaryReader br = new(ms);
		for (int i = 0; i < Root.GameMemoryService.MainBlock.StatsCount; i++)
		{
			int gemsCollected = br.ReadInt32();
			br.BaseStream.Seek(sizeof(int) * 5, SeekOrigin.Current); // Skip 5 int stats.
			int homingStored = br.ReadInt32();
			int gemsDespawned = br.ReadInt32();
			int gemsEaten = br.ReadInt32();
			int gemsTotal = br.ReadInt32();
			int homingEaten = br.ReadInt32();
			br.BaseStream.Seek(sizeof(ushort) * 17 * 2, SeekOrigin.Current); // Skip 17 ushort stats (two for each enemy).

			_statistics.Add(new()
			{
				GemsCollected = gemsCollected,
				HomingStored = homingStored,
				GemsDespawned = gemsDespawned,
				GemsEaten = gemsEaten,
				GemsTotal = gemsTotal,
				HomingEaten = homingEaten,
			});
		}

		TimerStart = Root.GameMemoryService.MainBlock.StartTimer;
		TimerEnd = Root.GameMemoryService.MainBlock.StartTimer + Root.GameMemoryService.MainBlock.Time;

		SplitsData.Populate();
	}
}
