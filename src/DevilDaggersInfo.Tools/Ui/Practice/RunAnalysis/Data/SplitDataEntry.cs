namespace DevilDaggersInfo.Tools.Ui.Practice.RunAnalysis.Data;

public record struct SplitDataEntry
{
	public int DisplayTimer;
	public SplitDataEntryKind Kind;
	public int? Homing;
	public int? HomingPrevious;

	public SplitDataEntry(int displayTimer, SplitDataEntryKind kind, int? homing, int? homingPrevious)
	{
		DisplayTimer = displayTimer;
		Kind = kind;
		Homing = homing;
		HomingPrevious = homingPrevious;
	}
}
