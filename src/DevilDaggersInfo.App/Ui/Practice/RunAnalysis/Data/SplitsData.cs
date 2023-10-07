namespace DevilDaggersInfo.App.Ui.Practice.RunAnalysis.Data;

public class SplitsData
{
	private readonly SplitDataEntry[] _homingSplitData = new SplitDataEntry[SplitData.Count + 2]; // Include start and end.

	public IReadOnlyList<SplitDataEntry> HomingSplitData => _homingSplitData;

	public static IReadOnlyList<(int Label, int Seconds)> SplitData { get; } = new List<(int Label, int Seconds)>
	{
		(350, 366),
		(700, 709),
		(800, 800),
		(880, 875),
		(930, 942),
		(1000, 996),
		(1040, 1047),
		(1080, 1091),
		(1130, 1133),
		(1160, 1170),
	};

	public void Populate()
	{
		bool addedTimerStart = false;
		bool addedTimerEnd = false;
		int homingSplitDataIndex = 0;
		for (int i = 0; i < SplitData.Count; i++)
		{
			(int Label, int Seconds) splitEntry = SplitData[i];

			if (!addedTimerStart && splitEntry.Seconds > RunAnalysisWindow.StatsData.TimerStart)
			{
				int? firstHomingStored = RunAnalysisWindow.StatsData.Statistics.Count > 0 ? RunAnalysisWindow.StatsData.Statistics[0].HomingStored : null;
				addedTimerStart = true;
				_homingSplitData[homingSplitDataIndex] = new((int)RunAnalysisWindow.StatsData.TimerStart, SplitDataEntryKind.Start, firstHomingStored, firstHomingStored);
				homingSplitDataIndex++;
			}

			if (!addedTimerEnd && splitEntry.Seconds > RunAnalysisWindow.StatsData.TimerEnd)
			{
				int? lastHomingStored = RunAnalysisWindow.StatsData.Statistics.Count > 0 ? RunAnalysisWindow.StatsData.Statistics[^1].HomingStored : null;
				addedTimerEnd = true;
				_homingSplitData[homingSplitDataIndex] = new((int)RunAnalysisWindow.StatsData.TimerEnd, SplitDataEntryKind.End, lastHomingStored, homingSplitDataIndex == 0 ? null : _homingSplitData[homingSplitDataIndex - 1].Homing);
				homingSplitDataIndex++;
			}

			int actualIndex = splitEntry.Seconds - (int)MathF.Ceiling(RunAnalysisWindow.StatsData.TimerStart); // TODO: Test if ceiling is correct.
			bool hasValue = actualIndex >= 0 && actualIndex < RunAnalysisWindow.StatsData.Statistics.Count;
			int? homing = hasValue ? RunAnalysisWindow.StatsData.Statistics[actualIndex].HomingStored : null;
			int? previousHoming = homingSplitDataIndex > 0 ? _homingSplitData[homingSplitDataIndex - 1].Homing : null;

			_homingSplitData[homingSplitDataIndex] = new(splitEntry.Label, SplitDataEntryKind.Default, homing, previousHoming);
			homingSplitDataIndex++;
		}

		// Clear values before timer start and after timer end.
		for (int i = 0; i < homingSplitDataIndex; i++)
		{
			if (_homingSplitData[i].DisplayTimer < (int)RunAnalysisWindow.StatsData.TimerStart || _homingSplitData[i].DisplayTimer > (int)RunAnalysisWindow.StatsData.TimerEnd)
				_homingSplitData[i] = new(_homingSplitData[i].DisplayTimer, _homingSplitData[i].Kind, null, null);
		}
	}
}
