namespace DevilDaggersInfo.App.Ui.Practice.RunAnalysis.Data;

public struct SplitDataEntry
{
	public SplitDataEntry(int displayTimer, SplitDataEntryKind kind, int? homing, int? homingPrevious)
	{
		DisplayTimer = displayTimer;
		Kind = kind;
		Homing = homing;
		HomingPrevious = homingPrevious;
	}

	public int DisplayTimer;
	public SplitDataEntryKind Kind;
	public int? Homing;
	public int? HomingPrevious;
}
