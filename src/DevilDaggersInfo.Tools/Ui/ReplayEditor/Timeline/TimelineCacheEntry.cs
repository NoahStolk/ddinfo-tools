namespace DevilDaggersInfo.Tools.Ui.ReplayEditor.Timeline;

public sealed record TimelineCacheEntry
{
	public int BoidSpawnEventCount { get; set; }
	public int LeviathanSpawnEventCount { get; set; }
	public int PedeSpawnEventCount { get; set; }
	public int SpiderEggSpawnEventCount { get; set; }
	public int SpiderSpawnEventCount { get; set; }
	public int SquidSpawnEventCount { get; set; }
	public int ThornSpawnEventCount { get; set; }
	public int DaggerSpawnEventCount { get; set; }
	public int EntityOrientationEventCount { get; set; }
	public int EntityPositionEventCount { get; set; }
	public int EntityTargetEventCount { get; set; }
	public int GemEventCount { get; set; }
	public int HitEventCount { get; set; }
	public int TransmuteEventCount { get; set; }
}
