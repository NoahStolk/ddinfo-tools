namespace DevilDaggersInfo.Tools.User.Cache.Model;

public record UserCacheModel
{
	public int PlayerId { get; init; }
	public int WindowWidth { get; init; }
	public int WindowHeight { get; init; }
	public bool WindowIsMaximized { get; init; }

	public static UserCacheModel Default { get; } = new()
	{
		PlayerId = 0,
		WindowWidth = MinWindowWidth,
		WindowHeight = MinWindowHeight,
		WindowIsMaximized = false,
	};

	public static int MinWindowWidth => 1366;
	public static int MinWindowHeight => 768;

	public UserCacheModel Sanitize()
	{
		return this with
		{
			WindowWidth = Math.Max(WindowWidth, MinWindowWidth),
			WindowHeight = Math.Max(WindowHeight, MinWindowHeight),
		};
	}
}
