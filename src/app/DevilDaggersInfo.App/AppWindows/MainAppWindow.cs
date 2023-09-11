using DevilDaggersInfo.App.Engine.ImGui;
using DevilDaggersInfo.App.Networking;
using DevilDaggersInfo.App.Networking.TaskHandlers;
using DevilDaggersInfo.App.Ui;
using DevilDaggersInfo.App.Ui.Config;
using DevilDaggersInfo.App.User.Cache;
using DevilDaggersInfo.App.Utils;
using ImGuiNET;
using Silk.NET.Core;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Runtime.InteropServices;

namespace DevilDaggersInfo.App.AppWindows;

public class MainAppWindow
{
	private ImGuiController? _imGuiController;
	private GL? _gl;
	private IInputContext? _inputContext;

	public MainAppWindow()
	{
		// Always keep V-sync on to prevent missing inputs.
		WindowInstance = Window.Create(WindowOptions.Default with { VSync = true });

		Vector2D<int> windowSize = new(UserCache.Model.WindowWidth, UserCache.Model.WindowHeight);
		Vector2D<int> monitorSize = Silk.NET.Windowing.Monitor.GetMainMonitor(WindowInstance).Bounds.Size;
		WindowInstance.Size = windowSize;
		WindowInstance.Position = monitorSize / 2 - windowSize / 2;
		WindowInstance.Title = $"ddinfo tools {AssemblyUtils.EntryAssemblyVersion}";

		WindowInstance.Load += OnWindowOnLoad;
		WindowInstance.FramebufferResize += OnWindowOnFramebufferResize;
		WindowInstance.Render += OnWindowOnRender;
		WindowInstance.Closing += OnWindowOnClosing;
	}

	public IWindow WindowInstance { get; }

	private void OnWindowOnLoad()
	{
		_gl = WindowInstance.CreateOpenGL();
		_inputContext = WindowInstance.CreateInput();
		_imGuiController = new(_gl, WindowInstance, _inputContext, () =>
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
		Root.Window = WindowInstance;

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
			Root.Keyboard.KeyDown += (keyboard, key, _) => Shortcuts.OnKeyPressed(keyboard, key);
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
		WindowInstance.SetWindowIcon(rawImages);
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

		Root.Application.RenderCounter.Increment();
		Root.Application.LastRenderDelta = deltaF;

		_imGuiController.Update(deltaF);

		_gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		UiRenderer.Render(deltaF);

		_imGuiController.Render();

		if (Ui.Main.MainWindow.ShouldClose)
			WindowInstance.Close();
	}

	private void OnWindowOnClosing()
	{
		_imGuiController?.Dispose();
		_inputContext?.Dispose();
		_gl?.Dispose();
	}
}
