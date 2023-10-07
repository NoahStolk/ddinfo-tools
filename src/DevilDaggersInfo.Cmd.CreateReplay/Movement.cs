namespace DevilDaggersInfo.Cmd.CreateReplay;

/// <summary>
/// Helper enum to assign multiple movement keys simultaneously.
/// </summary>
[Flags]
public enum Movement
{
	None = 0,
	Left = 1,
	Right = 2,
	Forward = 4,
	Backward = 8,
}
