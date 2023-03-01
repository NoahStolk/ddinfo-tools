namespace DevilDaggersInfo.App.Ui.Base.Utils;

public static class StringResources
{
	private const string _movement3d = "Use WASD, space, and left shift to move around.";
	private const string _camera3d = "Hold right click to look around.";
	private const string _tileEditor3d = "Use the scroll wheel to raise individual tiles.";
	private const string _escape3d = "Press escape to exit.";

	public const string MainMenu = """
		This is an alpha version of the rewritten tools.
		It is still very much a work in progress.

		I also do not have a deadline or schedule for these developments,
		and there will not be an official release date any time soon.

		If you encounter any problems, please report them on Discord/GitHub.

		Thank you for testing.

		For more information, go to:
		""";

	public const string ReplaySimulator = $"""
		{_movement3d}
		{_camera3d}
		{_escape3d}

		NOTE: This feature is VERY EXPERIMENTAL and may never fully work.

		For now, it only tries to make an approximation for the player movement.
		Tile collisions, bhops, air control, and dagger jumps are still missing however.

		Enemy movement is not simulated at all and this most likely won't be implemented.
		""";

	public const string SurvivalEditor3d = $"""
		{_movement3d}
		{_camera3d}
		{_tileEditor3d}
		{_escape3d}

		The 3D editor is still a work in progress.
		""";
}
