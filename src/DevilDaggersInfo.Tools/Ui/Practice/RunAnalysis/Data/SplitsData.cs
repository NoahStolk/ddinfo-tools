namespace DevilDaggersInfo.Tools.Ui.Practice.RunAnalysis.Data;

internal sealed class SplitsData
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

	public void Populate(PracticeStatsData statsData)
	{
		bool addedTimerStart = false;
		bool addedTimerEnd = false;
		int homingSplitDataIndex = 0;
		for (int i = 0; i < SplitData.Count; i++)
		{
			(int Label, int Seconds) splitEntry = SplitData[i];

			if (!addedTimerStart && splitEntry.Seconds > statsData.TimerStart)
			{
				int? firstHomingStored = statsData.Statistics.Count > 0 ? statsData.Statistics[0].HomingStored : null;
				addedTimerStart = true;
				_homingSplitData[homingSplitDataIndex] = new SplitDataEntry((int)statsData.TimerStart, SplitDataEntryKind.Start, firstHomingStored, firstHomingStored);
				homingSplitDataIndex++;
			}

			if (!addedTimerEnd && splitEntry.Seconds > statsData.TimerEnd)
			{
				int? lastHomingStored = statsData.Statistics.Count > 0 ? statsData.Statistics[^1].HomingStored : null;
				addedTimerEnd = true;
				_homingSplitData[homingSplitDataIndex] = new SplitDataEntry((int)statsData.TimerEnd, SplitDataEntryKind.End, lastHomingStored, homingSplitDataIndex == 0 ? null : _homingSplitData[homingSplitDataIndex - 1].Homing);
				homingSplitDataIndex++;
			}

			int actualIndex = splitEntry.Seconds - (int)MathF.Ceiling(statsData.TimerStart); // TODO: Test if ceiling is correct.
			bool hasValue = actualIndex >= 0 && actualIndex < statsData.Statistics.Count;
			int? homing = hasValue ? statsData.Statistics[actualIndex].HomingStored : null;
			int? previousHoming = homingSplitDataIndex > 0 ? _homingSplitData[homingSplitDataIndex - 1].Homing : null;

			_homingSplitData[homingSplitDataIndex] = new SplitDataEntry(splitEntry.Label, SplitDataEntryKind.Default, homing, previousHoming);
			homingSplitDataIndex++;
		}

		// Clear values before timer start and after timer end.
		for (int i = 0; i < homingSplitDataIndex; i++)
		{
			if (_homingSplitData[i].DisplayTimer < (int)statsData.TimerStart || _homingSplitData[i].DisplayTimer > (int)statsData.TimerEnd)
				_homingSplitData[i] = new SplitDataEntry(_homingSplitData[i].DisplayTimer, _homingSplitData[i].Kind, null, null);
		}
	}
}
