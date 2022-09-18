using DevilDaggersInfo.App.Tools.States;
using Warp.Ui;
using Warp.Ui.Components;

namespace DevilDaggersInfo.App.Tools.Components.SpawnsetSpawns;

// TODO: Implement generic ScrollViewer.
public class SpawnsWrapper : AbstractComponent
{
	private readonly Scrollbar _scrollbar;
	private readonly Spawns _spawns;

	public SpawnsWrapper(Rectangle metric)
		: base(metric)
	{
		Rectangle spawnsMetric = Rectangle.At(0, 0, 512, 768);

		_spawns = new(spawnsMetric, this);
		_scrollbar = new(spawnsMetric with { X1 = spawnsMetric.X2, X2 = spawnsMetric.X2 + 32 }, SetScroll);

		NestingContext.Add(_spawns);
		NestingContext.Add(_scrollbar);

		void SetScroll(float percentage)
		{
			_spawns.SetScrollOffset(new(0, (int)MathF.Round(percentage * -(StateManager.SpawnsetState.Spawnset.Spawns.Length * Spawns.SpawnEntryHeight))));
		}
	}

	public void SetSpawnset()
	{
		_scrollbar.ThumbPercentageSize = StateManager.SpawnsetState.Spawnset.Spawns.Length == 0 ? 0 : Math.Clamp(_spawns.Metric.Size.Y / (float)(StateManager.SpawnsetState.Spawnset.Spawns.Length * Spawns.SpawnEntryHeight), 0, 1);
		_scrollbar.TopPercentage = 0;
		_spawns.SetSpawnset();
	}

	public void SetScroll(int relativeScrollPixels)
	{
		_spawns.SetScrollOffset(_spawns.NestingContext.ScrollOffset + new Vector2i<int>(0, relativeScrollPixels));
		float topPercentage = -_spawns.NestingContext.ScrollOffset.Y / (float)(StateManager.SpawnsetState.Spawnset.Spawns.Length * Spawns.SpawnEntryHeight);
		_scrollbar.TopPercentage = topPercentage;
	}
}
