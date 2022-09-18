using DevilDaggersInfo.App.Tools.Components.SpawnsetArena;
using DevilDaggersInfo.App.Tools.Components.SpawnsetHistory;
using DevilDaggersInfo.App.Tools.Components.SpawnsetMenu;
using DevilDaggersInfo.App.Tools.Components.SpawnsetSpawns;
using Warp.Ui;

namespace DevilDaggersInfo.App.Tools.Layouts;

public class MainLayout : Layout
{
	public MainLayout()
		: base(new(0, 0, 1920, 1080))
	{
		Menu menu = new(new(0, 0, 1920, 24));
		ArenaWrapper arenaWrapper = new(Rectangle.At(1024, 64, 864, 512));
		SpawnsWrapper = new(Rectangle.At(0, 64, 544, 512));
		HistoryWrapper = new(Rectangle.At(1408, 824, 512, 256));

		NestingContext.Add(menu);
		NestingContext.Add(arenaWrapper);
		NestingContext.Add(SpawnsWrapper);
		NestingContext.Add(HistoryWrapper);
	}

	public SpawnsWrapper SpawnsWrapper { get; }
	public HistoryWrapper HistoryWrapper { get; }
}
