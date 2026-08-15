using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Scenes;
using DevilDaggersInfo.Tools.Scenes.Rendering;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Utils;
using DevilDaggersInfo.Tools.User.Settings;
using Silk.NET.GLFW;
using Silk.NET.OpenGL;

namespace DevilDaggersInfo.Tools.Ui.Main;

internal sealed unsafe class MainScene(
	Glfw glfw,
	GL gl,
	WindowHandle* window,
	GlfwInput glfwInput,
	ArenaRenderer arenaRenderer,
	FileStates fileStates,
	SpawnsetSaver spawnsetSaver,
	UserSettings userSettings)
{
	private readonly SpawnsetBinary _mainMenuSpawnset = SpawnsetBinary.CreateDefault();

	private ArenaScene? _mainMenuScene;

	public void Initialize()
	{
		_mainMenuScene = new ArenaScene(glfw, window, glfwInput, () => _mainMenuSpawnset, true, false, fileStates, spawnsetSaver, userSettings);
		_mainMenuScene.AddSkull4();
	}

	public void Render(float delta)
	{
		gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

		// Use physical pixels for the GL viewport.
		// UserCache stores logical window size, which differs on HiDPI Wayland.
		// Must be queried before Update, which needs the size to build the camera matrices.
		glfw.GetFramebufferSize(window, out int framebufferWidth, out int framebufferHeight);

		_mainMenuScene?.Update(false, false, delta, framebufferWidth, framebufferHeight);

		// Keep track of the original viewport so we can restore it later.
		Span<int> originalViewport = stackalloc int[4];
		gl.GetInteger(GLEnum.Viewport, originalViewport);
		gl.Viewport(0, 0, (uint)framebufferWidth, (uint)framebufferHeight);

		gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		gl.Enable(EnableCap.DepthTest);
		gl.Enable(EnableCap.Blend);
		gl.Enable(EnableCap.CullFace);
		gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

		if (_mainMenuScene != null)
			arenaRenderer.Render(_mainMenuScene, false);

		gl.Viewport(originalViewport[0], originalViewport[1], (uint)originalViewport[2], (uint)originalViewport[3]);
		gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
	}
}
