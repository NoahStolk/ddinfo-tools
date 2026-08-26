namespace DevilDaggersInfo.Tools.NativeInterface.Services;

/// <summary>
/// The outcome of resolving the address of the ddstats block in the running game. The failure cases are deliberately
/// separate: "the game's memory is off limits" and "the game's memory holds no block" need different answers from the
/// user, and collapsing them into "no game connected" hides both.
/// </summary>
internal enum BlockAddressStatus
{
	/// <summary>
	/// Nothing has been resolved. The game is not running, or its main module is not available yet.
	/// </summary>
	Unresolved,

	/// <summary>
	/// The block was located.
	/// </summary>
	Resolved,

	/// <summary>
	/// The game's memory could not be read at all. On macOS this is almost always a launch without sudo.
	/// </summary>
	MemoryUnreadable,

	/// <summary>
	/// The game's memory was read, but no ddstats block was found in it.
	/// </summary>
	BlockNotFound,
}
