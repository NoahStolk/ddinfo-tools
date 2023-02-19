using DevilDaggersInfo.App.Ui.Base;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.Rendering;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.StateManagement.Base.Actions;
using Silk.NET.GLFW;
using System.Diagnostics;
using Warp.NET.Content;
using Warp.NET.Debugging;

namespace DevilDaggersInfo.App;

public static class Program
{
	public static unsafe void Main()
	{
		const int debugTimeout = 3;
		Stopwatch sw = Stopwatch.StartNew();

		OnChangeWindowSize = (w, h) =>
		{
			ViewportState.Viewport3d = new(0, 0, w, h);
			ViewportState.UpdateViewports(w, h);
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
		WarpBlobs.Initialize(ddInfoToolsContent.Blobs);
		WarpCharsets.Initialize(ddInfoToolsContent.Charsets);
		WarpModels.Initialize(ddInfoToolsContent.Models);
		WarpShaders.Initialize(ddInfoToolsContent.Shaders);
		WarpTextures.Initialize(ddInfoToolsContent.Textures);
		WarpShaderUniformInitializer.Initialize();

		fixed (byte* ptr = &WarpTextures.ApplicationIcon.Pixels.ToArray()[0])
		{
			Image image = new()
			{
				Width = WarpTextures.ApplicationIcon.Width,
				Height = WarpTextures.ApplicationIcon.Height,
				Pixels = ptr,
			};
			Graphics.Glfw.SetWindowIcon(Window, 1, &image);
		}

		DebugStack.Add(sw.ElapsedMilliseconds, debugTimeout, "init content");

		Root.Dependencies = new DependencyContainer();
		DebugStack.Add(sw.ElapsedMilliseconds, debugTimeout, "init deps and ui");

		Game game = Bootstrapper.CreateGame<Game>();
		Root.Game = game;
		DebugStack.Add(sw.ElapsedMilliseconds, debugTimeout, "init game");
		sw.Stop();

		StateManager.Dispatch(new SetLayout(Root.Dependencies.ConfigLayout));
		game.Run();
	}
}
