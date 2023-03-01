namespace DevilDaggersInfo.App.Ui.Base.DependencyPattern;

public static class Root
{
	private static IGame? _game;
	private static IDependencyContainer? _dependencies;

	public static IGame Game
	{
		get => _game ?? throw new InvalidOperationException("Game is not initialized.");
		set => _game = value;
	}

	public static IDependencyContainer Dependencies
	{
		get => _dependencies ?? throw new InvalidOperationException("DependencyContainer is not initialized.");
		set => _dependencies = value;
	}
}
