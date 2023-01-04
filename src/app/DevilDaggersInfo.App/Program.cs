using DevilDaggersInfo.App.Ui.Base;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using Warp.NET.Content.Conversion;
using Warp.NET.RenderImpl.Ui;
using Warp.NET.RenderImpl.Ui.Rendering;

namespace DevilDaggersInfo.App;

public static class Program
{
	public static Viewport Viewport3d { get; private set; }

	public static void Main()
	{
		Graphics.OnChangeWindowSize = (w, h) =>
		{
			Viewport3d = new(0, 0, w, h);
			OnChangeWindowSize(w, h);
		};
		CreateWindow(new("DDINFO TOOLS", Constants.NativeWidth, Constants.NativeHeight, false));
		SetWindowSizeLimits(Constants.NativeWidth, Constants.NativeHeight, -1, -1);

#if DEBUG
		const string? ddInfoToolsContentRootDirectory = @"..\..\..\..\..\app-ui\DevilDaggersInfo.App.Ui.Base\Content";
#else
		const string? ddInfoToolsContentRootDirectory = null;
#endif
		DecompiledContentFile ddInfoToolsContent = Bootstrapper.GetDecompiledContent(ddInfoToolsContentRootDirectory, "ddinfo");
		DdInfoToolsBaseBlobs.Initialize(ddInfoToolsContent.Blobs);
		DdInfoToolsBaseModels.Initialize(ddInfoToolsContent.Models);
		DdInfoToolsBaseShaders.Initialize(ddInfoToolsContent.Shaders);
		DdInfoToolsBaseTextures.Initialize(ddInfoToolsContent.Textures);

		DdInfoToolsBaseShaderUniformInitializer.Initialize();

#if DEBUG
		const string? warpRenderImplUiContentRootDirectory = @"C:\Users\NOAH\source\repos\Warp.NET\src\lib\Warp.NET.RenderImpl.Ui\Content"; // TODO: Get files via NuGet package.
#else
		const string? warpRenderImplUiContentRootDirectory = null;
#endif
		DecompiledContentFile warpRenderImplUiContent = Bootstrapper.GetDecompiledContent(warpRenderImplUiContentRootDirectory, "ddinfo-ui");
		WarpRenderImplUiCharsets.Initialize(warpRenderImplUiContent.Charsets);
		WarpRenderImplUiShaders.Initialize(warpRenderImplUiContent.Shaders);
		WarpRenderImplUiTextures.Initialize(warpRenderImplUiContent.Textures);

		WarpRenderImplUiShaderUniformInitializer.Initialize();

		Root.Dependencies = new DependencyContainer();

		Game game = Bootstrapper.CreateGame<Game>();
		Root.Game = game;
		RenderImplUiBase.Game = game;

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
