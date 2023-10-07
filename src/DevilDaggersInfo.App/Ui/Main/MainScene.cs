using DevilDaggersInfo.App.Scenes;
using DevilDaggersInfo.Core.Spawnset;
using Silk.NET.OpenGL;

namespace DevilDaggersInfo.App.Ui.Main;

public static class MainScene
{
	private static readonly SpawnsetBinary _mainMenuSpawnset = SpawnsetBinary.CreateDefault();

	private static ArenaScene? _mainMenuScene;

	public static void Initialize()
	{
		_mainMenuScene = new(static () => _mainMenuSpawnset, true, false);
		_mainMenuScene.AddSkull4();
	}

	public static void Render(float delta)
	{
		_mainMenuScene?.Update(false, false, delta);

		Root.Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

		int framebufferWidth = Root.Window.Size.X;
		int framebufferHeight = Root.Window.Size.Y;

		// Keep track of the original viewport so we can restore it later.
		Span<int> originalViewport = stackalloc int[4];
		Root.Gl.GetInteger(GLEnum.Viewport, originalViewport);
		Root.Gl.Viewport(0, 0, (uint)framebufferWidth, (uint)framebufferHeight);

		Root.Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		Root.Gl.Enable(EnableCap.DepthTest);
		Root.Gl.Enable(EnableCap.Blend);
		Root.Gl.Enable(EnableCap.CullFace);
		Root.Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

		_mainMenuScene?.Render(false, framebufferWidth, framebufferHeight);

		Root.Gl.Viewport(originalViewport[0], originalViewport[1], (uint)originalViewport[2], (uint)originalViewport[3]);
		Root.Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
	}
}
