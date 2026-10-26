namespace DevilDaggersInfo.Tools.Ui.CustomLeaderboards.Results;

internal sealed class CustomLeaderboardsResultState
{
	private List<UploadResult> _results = [];

	public int Count => _results.Count;

	public UploadResult this[int index] => _results[index];

	public void AddResult(UploadResult newResult)
	{
		foreach (UploadResult result in _results)
			result.IsExpanded = false;

		_results.Add(newResult);
		_results = [.. _results.OrderByDescending(ur => ur.SubmittedAt)];
	}

	public void ClearResults()
	{
		_results.Clear();
	}

	public void SetExpanded(bool expanded)
	{
		foreach (UploadResult result in _results)
			result.IsExpanded = expanded;
	}
}
