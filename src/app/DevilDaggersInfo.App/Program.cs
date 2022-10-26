using DevilDaggersInfo.App.Ui.Base;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using System.Reflection;
using Warp;

namespace DevilDaggersInfo.App;

public static class Program
{
	private const float _nativeAspectRatio = Constants.NativeWidth / (float)Constants.NativeHeight;

	private static Game _game = null!;

	public static Viewport ViewportUi { get; private set; }
	public static int LeftOffset { get; private set; }
	public static int BottomOffset { get; private set; }

	public static Viewport Viewport3d { get; private set; }

	public static Vector2 UiScale { get; private set; }

	public static void Main()
	{
		Graphics.OnChangeWindowSize = OnChangeWindowSize;

#if DEBUG
		const string? contentRootDirectory = @"..\..\..\..\..\app-ui\DevilDaggersInfo.App.Ui.Base\Content";
#else
		const string? contentRootDirectory = null;
#endif
		_game = Bootstrapper.CreateGame<Game, ShaderUniformInitializer>("DDINFO TOOLS", Constants.NativeWidth, Constants.NativeHeight, false, contentRootDirectory, "ddinfo-tools", Assembly.GetAssembly(typeof(Root)));
		CoreBase.Game = _game;
		Root.Game = _game;

		Graphics.OnChangeWindowIsActive = OnChangeWindowIsActive;

		_game.Initialize();
		_game.Run();
	}

	private static void OnChangeWindowSize(int width, int height)
	{
		Viewport3d = new(0, 0, width, height);

		int minDimension = (int)Math.Min(height, width / _nativeAspectRatio);
		int clampedHeight = Math.Max(Constants.NativeHeight, minDimension / Constants.NativeHeight * Constants.NativeHeight);

		const float originalAspectRatio = Constants.NativeWidth / (float)Constants.NativeHeight;
		float adjustedWidth = clampedHeight * originalAspectRatio; // Adjusted for aspect ratio
		LeftOffset = (int)((width - adjustedWidth) / 2);
		BottomOffset = (height - clampedHeight) / 2;
		ViewportUi = new(LeftOffset, BottomOffset, (int)adjustedWidth, clampedHeight); // Fix viewport to maintain aspect ratio

		UiScale = new(ViewportUi.Width / (float)Constants.NativeWidth, ViewportUi.Height / (float)Constants.NativeHeight);
	}

	private static void OnChangeWindowIsActive(bool isActive)
	{
		if (_game.IsPaused)
			_game.TogglePause();
	}
}
