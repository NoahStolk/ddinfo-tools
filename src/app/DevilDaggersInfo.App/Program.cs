using DevilDaggersInfo.App.Ui.Base;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.Rendering;
using System.Diagnostics;
using Warp.NET.Content;
using Warp.NET.Debugging;

namespace DevilDaggersInfo.App;

public static class Program
{
	public static Viewport Viewport3d { get; private set; }

	public static void Main()
	{
		const int debugTimeout = 5;
		Stopwatch sw = Stopwatch.StartNew();

		Graphics.OnChangeWindowSize = (w, h) =>
		{
			Viewport3d = new(0, 0, w, h);
			OnChangeWindowSize(w, h);
		};
		CreateWindow(new("DDINFO TOOLS", Constants.NativeWidth, Constants.NativeHeight, false));
		SetWindowSizeLimits(Constants.NativeWidth, Constants.NativeHeight, -1, -1);
		DebugStack.Add(sw.ElapsedMilliseconds, debugTimeout, "init window");

#if DEBUG
		const string? ddInfoToolsContentRootDirectory = @"..\..\..\..\..\app-ui\DevilDaggersInfo.App.Ui.Base\Content";
#else
		const string? ddInfoToolsContentRootDirectory = null;
#endif
		DecompiledContentFile ddInfoToolsContent = Bootstrapper.GetDecompiledContent(ddInfoToolsContentRootDirectory, "ddinfo");
		DdInfoToolsBaseBlobs.Initialize(ddInfoToolsContent.Blobs);
		DdInfoToolsBaseCharsets.Initialize(ddInfoToolsContent.Charsets);
		DdInfoToolsBaseModels.Initialize(ddInfoToolsContent.Models);
		DdInfoToolsBaseShaders.Initialize(ddInfoToolsContent.Shaders);
		DdInfoToolsBaseTextures.Initialize(ddInfoToolsContent.Textures);
		DdInfoToolsBaseShaderUniformInitializer.Initialize();
		DebugStack.Add(sw.ElapsedMilliseconds, debugTimeout, "init content");

		Root.Dependencies = new DependencyContainer();
		DebugStack.Add(sw.ElapsedMilliseconds, debugTimeout, "init deps and ui");

		Game game = Bootstrapper.CreateGame<Game>();
		Root.Game = game;
		DebugStack.Add(sw.ElapsedMilliseconds, debugTimeout, "init game");
		sw.Stop();

		game.Initialize();
		game.Run();

		static void OnChangeWindowSize(int width, int height)
		{
			Viewport3d = new(0, 0, width, height);

			const float nativeAspectRatio = Constants.NativeWidth / (float)Constants.NativeHeight;
			int minDimension = (int)Math.Min(height, width / nativeAspectRatio);
			int clampedHeight = Math.Max(Constants.NativeHeight, minDimension / Constants.NativeHeight * Constants.NativeHeight);

			const float originalAspectRatio = Constants.NativeWidth / (float)Constants.NativeHeight;
			float adjustedWidth = clampedHeight * originalAspectRatio; // Adjusted for aspect ratio

			int leftOffset = (int)((width - adjustedWidth) / 2);
			int bottomOffset = (height - clampedHeight) / 2;
			ViewportState.Offset = new(leftOffset, bottomOffset);
			ViewportState.Viewport = new(leftOffset, bottomOffset, (int)adjustedWidth, clampedHeight); // Fix viewport to maintain aspect ratio
			ViewportState.Scale = new(ViewportState.Viewport.Width / (float)Constants.NativeWidth, ViewportState.Viewport.Height / (float)Constants.NativeHeight);
		}
	}
}
