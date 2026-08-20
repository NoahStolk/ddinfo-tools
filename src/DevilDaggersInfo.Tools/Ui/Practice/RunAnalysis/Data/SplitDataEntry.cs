namespace DevilDaggersInfo.Tools.Ui.Practice.RunAnalysis.Data;

internal record struct SplitDataEntry(int DisplayTimer, SplitDataEntryKind Kind, int? Homing, int? HomingPrevious)
{
	public int DisplayTimer = DisplayTimer;
	public SplitDataEntryKind Kind = Kind;
	public int? Homing = Homing;
	public int? HomingPrevious = HomingPrevious;
}
