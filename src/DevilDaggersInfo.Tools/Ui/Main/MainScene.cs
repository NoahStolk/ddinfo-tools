using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.Scenes;
using DevilDaggersInfo.Tools.User.Cache;
using ImGuiGlfw;
using Silk.NET.GLFW;
using Silk.NET.OpenGL;

namespace DevilDaggersInfo.Tools.Ui.Main;

public sealed unsafe class MainScene
{
	private readonly Glfw _glfw;
	private readonly GL _gl;
	private readonly WindowHandle* _window;
	private readonly GlfwInput _glfwInput;
	private readonly ResourceManager _resourceManager;
	private readonly SpawnsetBinary _mainMenuSpawnset = SpawnsetBinary.CreateDefault();

	private ArenaScene? _mainMenuScene;

	public MainScene(Glfw glfw, GL gl, WindowHandle* window, GlfwInput glfwInput, ResourceManager resourceManager)
	{
		_glfw = glfw;
		_gl = gl;
		_window = window;
		_glfwInput = glfwInput;
		_resourceManager = resourceManager;
	}

	public void Initialize()
	{
		_mainMenuScene = new ArenaScene(_glfw, _gl, _window, _glfwInput, _resourceManager, () => _mainMenuSpawnset, true, false);
		_mainMenuScene.AddSkull4();
	}

	public void Render(float delta)
	{
		_mainMenuScene?.Update(false, false, delta);

		_gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

		int framebufferWidth = UserCache.Model.WindowWidth;
		int framebufferHeight = UserCache.Model.WindowHeight;

		// Keep track of the original viewport so we can restore it later.
		Span<int> originalViewport = stackalloc int[4];
		_gl.GetInteger(GLEnum.Viewport, originalViewport);
		_gl.Viewport(0, 0, (uint)framebufferWidth, (uint)framebufferHeight);

		_gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		_gl.Enable(EnableCap.DepthTest);
		_gl.Enable(EnableCap.Blend);
		_gl.Enable(EnableCap.CullFace);
		_gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

		_mainMenuScene?.Render(false, framebufferWidth, framebufferHeight);

		_gl.Viewport(originalViewport[0], originalViewport[1], (uint)originalViewport[2], (uint)originalViewport[3]);
		_gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
	}
}
