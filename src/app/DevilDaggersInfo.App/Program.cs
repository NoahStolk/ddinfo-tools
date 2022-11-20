using DevilDaggersInfo.App.Ui.Base;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using Warp.NET.Content.Conversion;
using Warp.NET.RenderImpl.Ui;

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

		GameParameters gameParameters = new("DDINFO TOOLS", Constants.NativeWidth, Constants.NativeHeight, false);
		Bootstrapper.CreateWindow(gameParameters);

#if DEBUG
		const string? ddInfoToolsContentRootDirectory = @"..\..\..\..\..\app-ui\DevilDaggersInfo.App.Ui.Base\Content";
#else
		const string? ddInfoToolsContentRootDirectory = null;
#endif
		DecompiledContentFile ddInfoToolsContent = Bootstrapper.GetDecompiledContent(ddInfoToolsContentRootDirectory, "ddinfo-tools");
		DdInfoToolsBaseModels.Initialize(ddInfoToolsContent.Models);
		DdInfoToolsBaseShaders.Initialize(ddInfoToolsContent.Shaders);
		DdInfoToolsBaseTextures.Initialize(ddInfoToolsContent.Textures);

		DdInfoToolsBaseShaderUniformInitializer.Initialize();

#if DEBUG
		const string? warpRenderImplUiContentRootDirectory = @"C:\Users\NOAH\source\repos\Warp.NET\src\lib\Warp.NET.RenderImpl.Ui\Content"; // TODO: Get files via NuGet package.
#else
		const string? warpRenderImplUiContentRootDirectory = null;
#endif
		DecompiledContentFile warpRenderImplUiContent = Bootstrapper.GetDecompiledContent(warpRenderImplUiContentRootDirectory, "warp-render-impl-ui");
		WarpRenderImplUiCharsets.Initialize(warpRenderImplUiContent.Charsets);
		WarpRenderImplUiShaders.Initialize(warpRenderImplUiContent.Shaders);
		WarpRenderImplUiTextures.Initialize(warpRenderImplUiContent.Textures);

		WarpRenderImplUiShaderUniformInitializer.Initialize();

		_game = Bootstrapper.CreateGame<Game>(gameParameters);
		Root.Game = _game;
		RenderImplUiBase.Game = _game;

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
