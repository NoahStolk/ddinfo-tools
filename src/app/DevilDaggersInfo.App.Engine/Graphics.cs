using Silk.NET.GLFW;
using Silk.NET.OpenGL;
using ErrorCode = Silk.NET.GLFW.ErrorCode;
using Monitor = Silk.NET.GLFW.Monitor;

namespace DevilDaggersInfo.App.Engine;

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

	public static WindowState InitialWindowState { get; private set; }
	public static WindowState CurrentWindowState { get; private set; }
	public static bool WindowIsActive
	{
		get => _windowIsActive;
		private set
		{
			_windowIsActive = value;
			OnChangeWindowIsActive?.Invoke(_windowIsActive);
		}
	}

	public static int PrimaryMonitorWidth { get; private set; }
	public static int PrimaryMonitorHeight { get; private set; }

	public static unsafe void CreateWindow(WindowState initialWindowState)
	{
		if (_windowIsCreated)
			throw new InvalidOperationException("Window is already created. Cannot create window again.");

		InitialWindowState = initialWindowState;
		CurrentWindowState = initialWindowState with
		{
			Width = InitialWindowState.IsFullScreen ? PrimaryMonitorWidth : InitialWindowState.Width,
			Height = InitialWindowState.IsFullScreen ? PrimaryMonitorHeight : InitialWindowState.Height,
		};

		_glfw = Glfw.GetApi();
		_glfw.Init();
		CheckGlfwError(_glfw);

		_glfw.WindowHint(WindowHintInt.ContextVersionMajor, 3);
		_glfw.WindowHint(WindowHintInt.ContextVersionMinor, 3);
		_glfw.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);

		_glfw.WindowHint(WindowHintBool.Focused, true);
		_glfw.WindowHint(WindowHintBool.Resizable, true);
		CheckGlfwError(_glfw);

		Monitor* primaryMonitor = _glfw.GetPrimaryMonitor();
		if (primaryMonitor == (Monitor*)0)
		{
			// TODO: Log warning with Serilog.
			PrimaryMonitorWidth = 1024;
			PrimaryMonitorHeight = 768;
		}
		else
		{
			_glfw.GetMonitorWorkarea(primaryMonitor, out _, out _, out int primaryMonitorWidth, out int primaryMonitorHeight);
			PrimaryMonitorWidth = primaryMonitorWidth;
			PrimaryMonitorHeight = primaryMonitorHeight;
		}

		Window = _glfw.CreateWindow(CurrentWindowState.Width, CurrentWindowState.Height, CurrentWindowState.Title, CurrentWindowState.IsFullScreen ? primaryMonitor : (Monitor*)0, (WindowHandle*)0);
		CheckGlfwError(_glfw);
		if (Window == (WindowHandle*)0)
			throw new InvalidOperationException("Could not create window.");

		_glfw.SetKeyCallback(Window, (_, keys, _, state, _) => Input.KeyCallback(keys, state));
		_glfw.SetMouseButtonCallback(Window, (_, button, state, _) => Input.ButtonCallback(button, state));
		_glfw.SetScrollCallback(Window, (_, _, y) => Input.MouseWheelCallback(y));
		_glfw.SetFramebufferSizeCallback(Window, (_, w, h) => SetWindowSize(w, h));
		_glfw.SetWindowFocusCallback(Window, (_, focusing) => WindowIsActive = focusing);

		int x = (PrimaryMonitorWidth - CurrentWindowState.Width) / 2;
		int y = (PrimaryMonitorHeight - CurrentWindowState.Height) / 2;

		_glfw.SetWindowPos(Window, x, y);

		_glfw.MakeContextCurrent(Window);
		_gl = GL.GetApi(_glfw.GetProcAddress);

		SetWindowSize(CurrentWindowState.Width, CurrentWindowState.Height);

		_glfw.SwapInterval(0); // Turns VSync off.

		_windowIsCreated = true;
	}

	public static unsafe void SetWindowSizeLimits(int minWidth, int minHeight, int maxWidth, int maxHeight)
	{
		Glfw.SetWindowSizeLimits(Window, minWidth, minHeight, maxWidth, maxHeight);
	}

	private static void SetWindowSize(int width, int height)
	{
		CurrentWindowState = CurrentWindowState with
		{
			Width = width,
			Height = height,
		};
		OnChangeWindowSize?.Invoke(width, height);
	}

	private static unsafe void CheckGlfwError(Glfw glfw)
	{
		ErrorCode errorCode = glfw.GetError(out byte* c);
		if (errorCode == ErrorCode.NoError || c == (byte*)0)
			return;

		StringBuilder errorBuilder = new();
		while (*c != 0x00)
			errorBuilder.Append((char)*c++);

		throw new InvalidOperationException($"GLFW {errorCode}: {errorBuilder}");
	}
}
