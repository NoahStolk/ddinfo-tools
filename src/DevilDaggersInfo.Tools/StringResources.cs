namespace DevilDaggersInfo.Tools;

internal static class StringResources
{
	private const string _movement3D = "Use WASD, space, and left shift to move around.";
	private const string _camera3D = "Hold right click to look around.";

	private const string _tileEditor3D =
		"""
		Hold LMB to select multiple tiles.
		Hold LMB+CTRL to (de)select a single tile without clearing the selection.
		Use the scroll wheel to change the height of the selected tiles.
		""";

	public const string ReplaySimulator3D =
		$"""
		 {_movement3D}
		 {_camera3D}

		 NOTE: This feature is VERY EXPERIMENTAL and may never fully work.

		 For now, it only tries to make an approximation for the player movement.
		 Tile collisions, bhops, air control, and dagger jumps are still missing however.

		 Enemy movement is not simulated at all and this most likely won't be implemented.
		 """;

	public const string SpawnsetEditor3D =
		$"""
		 {_movement3D}
		 {_camera3D}
		 {_tileEditor3D}

		 The 3D editor is still a work in progress.
		 """;

	public const string DescriptionSpawnsetEditor =
		"""
		WORK IN PROGRESS

		Create and edit custom spawnsets (levels) for Devil Daggers.

		Some things you can do:
		- Create your own set of enemy spawns.
		- Create a custom arena.
		- Start with any hand upgrade.
		- Give yourself 10,000 homing daggers.
		- Use the Time Attack game mode, where the goal is to kill all enemies as fast as possible.
		- Use the Race game mode, where the goal is to reach the dagger as fast as possible.

		Be sure to check out the custom leaderboards to see what's possible.

		Note that using custom spawnsets will not submit your score to the official leaderboards.

		Spawnsets can only be used to practice the main game and play custom levels. They cannot be used to cheat and are completely safe to use.
		""";

	public const string DescriptionAssetEditor =
		"""
		WORK IN PROGRESS

		Create and edit mods for Devil Daggers.

		The following assets can be changed:
		- Audio
		- Meshes
		- Object bindings
		- Shaders
		- Textures

		Some mods are prohibited, meaning that you cannot submit scores over 1000 seconds with them.
		""";

	public const string DescriptionReplayEditor =
		"""
		WORK IN PROGRESS

		Create, analyze, and edit replays for Devil Daggers.

		You can download replays from the official leaderboards and save them as a local replay.

		This tool will likely not be useful for most players; it is mostly intended to figure out how some things in the game work.

		It could be used to:
		- Figure out how homing daggers, gems, or certain enemies behave under certain conditions, since their behavior is implicit and cannot reliably be modified using replays.
		- Figure out how certain movement techniques work in more detail (for optimizing race spawnsets).
		- Detect cheated replays (for example, shotgun tech intervals could be analyzed to detect if a player is using a macro).
		""";

	public const string DescriptionCustomLeaderboards =
		"""
		WINDOWS ONLY (FOR NOW)

		Custom leaderboards are leaderboards for custom spawnsets.

		All game modes are supported:
		- Survival
		- Time Attack
		- Race

		Leaderboards can be sorted by:
		- Time
		- Gems collected
		- Gems despawned
		- Gems eaten
		- Enemies killed
		- Enemies alive
		- Homing stored
		- Homing eaten

		Criteria can also be set for custom leaderboards, meaning that in order to submit a score, it has to meet all the criteria.

		This applies to almost every stat in the game, including specific enemy kill counts.

		Examples:
		- Gems eaten must be less than 30
		- Squid I kills must be equal to 0
		- Skull II kills must be greater than or equal to 3
		- Daggers fired must be less than gems collected + 2
		- Skull III kills must be greater than Skull I kills + Skull II kills
		""";

	public const string DescriptionPractice =
		"""
		Practice the main game by starting at any point in time with any hand upgrade and amount of gems/homing, using custom spawnsets that are automatically generated.

		Save templates to quickly load your desired practice settings.

		View live data from the current run:
		- Splits
		- Homing usage
		- Gem collection

		Note that using practice will not submit your score to the official leaderboards.

		Spawnsets can only be used to practice the main game and play custom levels. They cannot be used to cheat and are completely safe to use.
		""";

	public const string DescriptionModManager =
		"""
		WORK IN PROGRESS

		View all currently installed mods.

		Enable/disable mods and change their load order to further customize the game.

		Browse and find new mods to install directly from the devildaggers.info website.

		View which assets are contained in each mod, and which ones are prohibited for 1000+ scores.

		Enable/disable prohibited assets for each mod.

		View all effective game assets and their source mod, including whether they're being overridden by another mod.
		""";
}
