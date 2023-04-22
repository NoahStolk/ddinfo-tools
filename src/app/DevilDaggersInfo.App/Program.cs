using DevilDaggersInfo.App.Engine;
using DevilDaggersInfo.App.Engine.Content;
using DevilDaggersInfo.App.Engine.Debugging;
using DevilDaggersInfo.App.Ui.Base;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.Rendering;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.StateManagement.Base.Actions;
using Silk.NET.GLFW;
using System.Diagnostics;

namespace DevilDaggersInfo.App;

public static class Program
{
	public static unsafe void Main()
	{
		const int debugTimeout = 3;
		Stopwatch sw = Stopwatch.StartNew();

		OnChangeWindowSize = ViewportState.UpdateViewports;
		CreateWindow(new("DDINFO TOOLS", Constants.NativeWidth, Constants.NativeHeight, false));
		SetWindowSizeLimits(Constants.NativeWidth, Constants.NativeHeight, -1, -1);
		DebugStack.Add(sw.ElapsedMilliseconds, debugTimeout, "init window");

#if DEBUG
		const string? ddInfoToolsContentRootDirectory = @"..\..\..\..\..\app\DevilDaggersInfo.App.Ui.Base\Content";
#else
		const string? ddInfoToolsContentRootDirectory = null;
#endif
		DecompiledContentFile ddInfoToolsContent = Bootstrapper.GetDecompiledContent(ddInfoToolsContentRootDirectory, "ddinfo");
#if !SKIP_VALUE
		Blobs.Initialize(ddInfoToolsContent.Blobs);
#endif
		Charsets.Initialize(ddInfoToolsContent.Charsets);
		Models.Initialize(ddInfoToolsContent.Models);
		Shaders.Initialize(ddInfoToolsContent.Shaders);
		Textures.Initialize(ddInfoToolsContent.Textures);

		fixed (byte* ptr = &Textures.ApplicationIcon.Pixels.ToArray()[0])
		{
			Image image = new()
			{
				Width = Textures.ApplicationIcon.Width,
				Height = Textures.ApplicationIcon.Height,
				Pixels = ptr,
			};
			Graphics.Glfw.SetWindowIcon(Window, 1, &image);
		}

		DebugStack.Add(sw.ElapsedMilliseconds, debugTimeout, "init content");

		Root.Dependencies = new DependencyContainer();
		DebugStack.Add(sw.ElapsedMilliseconds, debugTimeout, "init deps and ui");

		Game game = new();
		EngineNodes.Initialize(game);
		Root.Game = game;
		DebugStack.Add(sw.ElapsedMilliseconds, debugTimeout, "init game");
		sw.Stop();

		StateManager.Dispatch(new SetLayout(Root.Dependencies.ConfigLayout));
		game.Run();
	}
}
