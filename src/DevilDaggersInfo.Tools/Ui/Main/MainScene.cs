using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.Engine;
using DevilDaggersInfo.Tools.Scenes;
using Silk.NET.OpenGL;

namespace DevilDaggersInfo.Tools.Ui.Main;

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

		Graphics.Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

		int framebufferWidth = Graphics.WindowWidth;
		int framebufferHeight = Graphics.WindowHeight;

		// Keep track of the original viewport so we can restore it later.
		Span<int> originalViewport = stackalloc int[4];
		Graphics.Gl.GetInteger(GLEnum.Viewport, originalViewport);
		Graphics.Gl.Viewport(0, 0, (uint)framebufferWidth, (uint)framebufferHeight);

		Graphics.Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		Graphics.Gl.Enable(EnableCap.DepthTest);
		Graphics.Gl.Enable(EnableCap.Blend);
		Graphics.Gl.Enable(EnableCap.CullFace);
		Graphics.Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

		_mainMenuScene?.Render(false, framebufferWidth, framebufferHeight);

		Graphics.Gl.Viewport(originalViewport[0], originalViewport[1], (uint)originalViewport[2], (uint)originalViewport[3]);
		Graphics.Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
	}
}
