using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Scenes;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Utils;
using DevilDaggersInfo.Tools.User.Cache;
using Silk.NET.GLFW;
using Silk.NET.OpenGL;

namespace DevilDaggersInfo.Tools.Ui.Main;

internal sealed unsafe class MainScene(Glfw glfw, GL gl, WindowHandle* window, GlfwInput glfwInput, ResourceManager resourceManager, FileStates fileStates, SpawnsetSaver spawnsetSaver)
{
	private readonly SpawnsetBinary _mainMenuSpawnset = SpawnsetBinary.CreateDefault();

	private ArenaScene? _mainMenuScene;

	public void Initialize()
	{
		_mainMenuScene = new ArenaScene(glfw, gl, window, glfwInput, resourceManager, () => _mainMenuSpawnset, true, false, fileStates, spawnsetSaver);
		_mainMenuScene.AddSkull4();
	}

	public void Render(float delta)
	{
		_mainMenuScene?.Update(false, false, delta);

		gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

		int framebufferWidth = UserCache.Model.WindowWidth;
		int framebufferHeight = UserCache.Model.WindowHeight;

		// Keep track of the original viewport so we can restore it later.
		Span<int> originalViewport = stackalloc int[4];
		gl.GetInteger(GLEnum.Viewport, originalViewport);
		gl.Viewport(0, 0, (uint)framebufferWidth, (uint)framebufferHeight);

		gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		gl.Enable(EnableCap.DepthTest);
		gl.Enable(EnableCap.Blend);
		gl.Enable(EnableCap.CullFace);
		gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

		_mainMenuScene?.Render(false, framebufferWidth, framebufferHeight);

		gl.Viewport(originalViewport[0], originalViewport[1], (uint)originalViewport[2], (uint)originalViewport[3]);
		gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
	}
}
