using DevilDaggersInfo.Tools.Engine.Extensions;
using Silk.NET.GLFW;
using Silk.NET.OpenGL;
using Monitor = Silk.NET.GLFW.Monitor;

namespace DevilDaggersInfo.Tools.Engine;

public static class Graphics
{
	private static bool _windowIsCreated;

	private static bool _windowIsActive = true;
	private static Glfw? _glfw;
	private static GL? _gl;

	public static Glfw Glfw => _glfw ?? throw new InvalidOperationException("GLFW is not initialized.");
	public static GL Gl => _gl ?? throw new InvalidOperationException("OpenGL is not initialized.");

	public static Action<bool>? OnChangeWindowIsActive { get; set; }
	public static Action<int, int>? OnChangeWindowSize { get; set; }

	public static unsafe WindowHandle* Window { get; private set; }

	public static int WindowWidth { get; private set; }
	public static int WindowHeight { get; private set; }
	public static bool WindowIsActive
	{
		get => _windowIsActive;
		private set
		{
			_windowIsActive = value;
			OnChangeWindowIsActive?.Invoke(_windowIsActive);
		}
	}

	public static unsafe void CreateWindow(string title, int width, int height, bool isMaximized)
	{
		if (_windowIsCreated)
			throw new InvalidOperationException("Window is already created. Cannot create window again.");

		_glfw = Glfw.GetApi();
		_glfw.Init();
		_glfw.CheckError();

		_glfw.WindowHint(WindowHintInt.ContextVersionMajor, 3);
		_glfw.WindowHint(WindowHintInt.ContextVersionMinor, 3);
		_glfw.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);

		_glfw.WindowHint(WindowHintBool.Focused, true);
		_glfw.WindowHint(WindowHintBool.Resizable, true);

		_glfw.CheckError();

		Window = _glfw.CreateWindow(width, height, title, null, null);
		_glfw.CheckError();
		if (Window == (WindowHandle*)0)
			throw new InvalidOperationException("Could not create window.");

		_glfw.SetFramebufferSizeCallback(Window, (_, w, h) => SetWindowSize(w, h));
		_glfw.SetWindowFocusCallback(Window, (_, focusing) => WindowIsActive = focusing);
		_glfw.SetCursorPosCallback(Window, (_, x, y) => Input.GlfwInput.CursorPosCallback(x, y));
		_glfw.SetScrollCallback(Window, (_, _, y) => Input.GlfwInput.MouseWheelCallback(y));
		_glfw.SetMouseButtonCallback(Window, (_, button, state, _) => Input.GlfwInput.MouseButtonCallback(button, state));
		_glfw.SetKeyCallback(Window, (_, keys, _, state, _) => Input.GlfwInput.KeyCallback(keys, state));
		_glfw.SetCharCallback(Window, (_, codepoint) => Input.GlfwInput.CharCallback(codepoint));

		if (isMaximized)
		{
			_glfw.MaximizeWindow(Window);
		}
		else
		{
			Monitor* primaryMonitor = _glfw.GetPrimaryMonitor();
			int primaryMonitorWidth, primaryMonitorHeight;
			if (primaryMonitor != null)
				_glfw.GetMonitorWorkarea(primaryMonitor, out _, out _, out primaryMonitorWidth, out primaryMonitorHeight);
			else
				(primaryMonitorWidth, primaryMonitorHeight) = (1024, 768);

			_glfw.SetWindowPos(Window, (primaryMonitorWidth - width) / 2, (primaryMonitorHeight - height) / 2);
		}

		_glfw.MakeContextCurrent(Window);
		_gl = GL.GetApi(_glfw.GetProcAddress);

		SetWindowSize(width, height);

		// Turn VSync on. There's really no need to turn it off for this application.
		_glfw.SwapInterval(1);

		_windowIsCreated = true;
	}

	public static unsafe void SetWindowSizeLimits(int minWidth, int minHeight, int maxWidth, int maxHeight)
	{
		Glfw.SetWindowSizeLimits(Window, minWidth, minHeight, maxWidth, maxHeight);
	}

	private static void SetWindowSize(int width, int height)
	{
		WindowWidth = width;
		WindowHeight = height;
		OnChangeWindowSize?.Invoke(width, height);
	}
}
