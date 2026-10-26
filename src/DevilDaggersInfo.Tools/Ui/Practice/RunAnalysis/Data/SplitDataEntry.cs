namespace DevilDaggersInfo.Tools.Ui.Practice.RunAnalysis.Data;

internal readonly record struct SplitDataEntry(int DisplayTimer, SplitDataEntryKind Kind, int? Homing, int? HomingPrevious)
{
	public readonly int DisplayTimer = DisplayTimer;
	public readonly SplitDataEntryKind Kind = Kind;
	public readonly int? Homing = Homing;
	public readonly int? HomingPrevious = HomingPrevious;
}
