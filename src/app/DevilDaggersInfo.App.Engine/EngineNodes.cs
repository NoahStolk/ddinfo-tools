namespace DevilDaggersInfo.App.Engine;

public static class EngineNodes
{
	private static bool _isInitialized;

	private static GameBase? _game;

	internal static GameBase Game => _game ?? throw new InvalidOperationException("Engine nodes are not initialized.");

	public static void Initialize(GameBase game)
	{
		if (_isInitialized)
			throw new InvalidOperationException("Engine nodes are already initialized.");

		_game = game;

		_isInitialized = true;
	}
}
