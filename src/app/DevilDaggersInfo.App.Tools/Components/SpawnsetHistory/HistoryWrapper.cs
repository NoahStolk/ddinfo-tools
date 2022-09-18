using DevilDaggersInfo.App.Tools.States;
using Warp.Ui;
using Warp.Ui.Components;

namespace DevilDaggersInfo.App.Tools.Components.SpawnsetHistory;

// TODO: Implement generic ScrollViewer.
public class HistoryWrapper : AbstractComponent
{
	private readonly Scrollbar _scrollbar;
	private readonly History _history;

	public HistoryWrapper(Rectangle metric)
		: base(metric)
	{
		Rectangle historyMetric = Rectangle.At(0, 0, 480, 256);

		_history = new(historyMetric, this);
		_scrollbar = new(historyMetric with { X1 = historyMetric.X2, X2 = historyMetric.X2 + 32 }, SetScroll);

		NestingContext.Add(_history);
		NestingContext.Add(_scrollbar);

		void SetScroll(float percentage)
		{
			_history.SetScrollOffset(new(0, (int)MathF.Round(percentage * -(SpawnsetHistoryManager.History.Count * History.HistoryEntryHeight))));
		}
	}

	public void SetHistory()
	{
		_scrollbar.ThumbPercentageSize = SpawnsetHistoryManager.History.Count == 0 ? 0 : Math.Clamp(_history.Metric.Size.Y / (float)(SpawnsetHistoryManager.History.Count * History.HistoryEntryHeight), 0, 1);
		_scrollbar.TopPercentage = 0;
		_history.SetHistory();
	}

	public void SetScroll(int relativeScrollPixels)
	{
		_history.SetScrollOffset(_history.NestingContext.ScrollOffset + new Vector2i<int>(0, relativeScrollPixels));
		float topPercentage = -_history.NestingContext.ScrollOffset.Y / (float)(SpawnsetHistoryManager.History.Count * History.HistoryEntryHeight);
		_scrollbar.TopPercentage = topPercentage;
	}
}
