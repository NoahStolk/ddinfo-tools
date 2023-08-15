using DevilDaggersInfo.App.Engine.ImGui;
using DevilDaggersInfo.App.Networking;
using DevilDaggersInfo.App.Networking.TaskHandlers;
using DevilDaggersInfo.App.Ui;
using DevilDaggersInfo.App.Ui.Config;
using DevilDaggersInfo.App.Ui.Main;
using DevilDaggersInfo.App.User.Cache;
using DevilDaggersInfo.App.User.Settings;
using DevilDaggersInfo.Core.Versioning;
using ImGuiNET;
using Silk.NET.Core;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Runtime.InteropServices;

namespace DevilDaggersInfo.App;

public class Application
{
	private readonly IWindow _window;

	private ImGuiController? _imGuiController;
	private GL? _gl;
	private IInputContext? _inputContext;

	public Application()
	{
		// Always keep V-sync on to prevent missing inputs.
		_window = Window.Create(WindowOptions.Default with { VSync = true });

		UserSettings.Load();
		UserCache.Load();

		Vector2D<int> windowSize = new(UserCache.Model.WindowWidth, UserCache.Model.WindowHeight);
		Vector2D<int> monitorSize = Silk.NET.Windowing.Monitor.GetMainMonitor(_window).Bounds.Size;
		_window.Size = windowSize;
		_window.Position = monitorSize / 2 - windowSize / 2;
		_window.Title = $"ddinfo tools {AssemblyUtils.EntryAssemblyVersion}";

		_window.Load += OnWindowOnLoad;
		_window.FramebufferResize += OnWindowOnFramebufferResize;
		_window.Render += OnWindowOnRender;
		_window.Closing += OnWindowOnClosing;

		if (!AppVersion.TryParse(AssemblyUtils.EntryAssemblyVersion, out AppVersion? appVersion))
			throw new InvalidOperationException("The current version number is invalid.");

		AppVersion = appVersion;
	}

	public static PerSecondCounter RenderCounter { get; } = new();
	public static float LastRenderDelta { get; private set; }

	public AppVersion AppVersion { get; }

	public void Run()
	{
		_window.Run();
	}

	public void Destroy()
	{
		_window.Dispose();
	}

	private void OnWindowOnLoad()
	{
		_gl = _window.CreateOpenGL();
		_inputContext = _window.CreateInput();
		_imGuiController = new(_gl, _window, _inputContext, () =>
		{
			ImGuiIOPtr io = ImGui.GetIO();

			// Add the default font first so it is actually used by default.
			io.Fonts.AddFontDefault();
			Root.FontGoetheBold20 = io.Fonts.AddFontFromFileTTF("goethebold.ttf", 20);
			Root.FontGoetheBold30 = io.Fonts.AddFontFromFileTTF("goethebold.ttf", 30);
			Root.FontGoetheBold60 = io.Fonts.AddFontFromFileTTF("goethebold.ttf", 60);
		});

		_gl.ClearColor(0, 0, 0, 1);

		ConfigureImGui();

		Root.InternalResources = InternalResources.Create(_gl);
		Root.Gl = _gl;
		Root.Mouse = _inputContext.Mice.Count == 0 ? null : _inputContext.Mice[0];
		Root.Keyboard = _inputContext.Keyboards.Count == 0 ? null : _inputContext.Keyboards[0];
		Root.Window = _window;
		Root.Application = this;

		if (Root.Mouse == null)
		{
			Modals.ShowError("No mouse available!");
			Root.Log.Error("No mouse available!");
		}

		if (Root.Keyboard == null)
		{
			Modals.ShowError("No keyboard available!");
			Root.Log.Error("No keyboard available!");
		}
		else
		{
			Root.Keyboard.KeyDown += Shortcuts.OnKeyPressed;
		}

		ConfigLayout.ValidateInstallation();
		AsyncHandler.Run(
			static newVersion =>
			{
				if (newVersion == null)
					return;

				UiRenderer.ShowUpdateAvailable();
				UpdateWindow.AvailableUpdateVersion = newVersion;
			},
			() => FetchLatestVersion.HandleAsync(Root.Application.AppVersion, Root.PlatformSpecificValues.AppOperatingSystem));

		RawImage rawImage = new(Root.InternalResources.ApplicationIconTexture.Width, Root.InternalResources.ApplicationIconTexture.Height, Root.InternalResources.ApplicationIconTexture.Pixels);
		Span<RawImage> rawImages = MemoryMarshal.CreateSpan(ref rawImage, 1);
		_window.SetWindowIcon(rawImages);
	}

	private static void ConfigureImGui()
	{
		ImGuiStylePtr style = ImGui.GetStyle();
		style.ScrollbarSize = 16;
		style.ScrollbarRounding = 0;

		Colors.SetColors(Colors.Main);
	}

	private void OnWindowOnFramebufferResize(Vector2D<int> size)
	{
		if (_gl == null)
			throw new InvalidOperationException("Window has not loaded.");

		_gl.Viewport(size);

		UserCache.Model = UserCache.Model with
		{
			WindowWidth = size.X,
			WindowHeight = size.Y,
		};
	}

	private void OnWindowOnRender(double delta)
	{
		if (_imGuiController == null || _gl == null)
			throw new InvalidOperationException("Window has not loaded.");

		float deltaF = (float)delta;

		RenderCounter.Increment();
		LastRenderDelta = deltaF;

		_imGuiController.Update(deltaF);

		_gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		UiRenderer.Render(deltaF);

		_imGuiController.Render();

		if (MainWindow.ShouldClose)
			_window.Close();
	}

	private void OnWindowOnClosing()
	{
		_imGuiController?.Dispose();
		_inputContext?.Dispose();
		_gl?.Dispose();
	}
}
