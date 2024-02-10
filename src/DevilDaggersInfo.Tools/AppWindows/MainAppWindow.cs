using DevilDaggersInfo.Tools.Engine;
using DevilDaggersInfo.Tools.Ui;
using DevilDaggersInfo.Tools.Ui.Config;
using DevilDaggersInfo.Tools.User.Cache;
using DevilDaggersInfo.Tools.Utils;
using ImGuiGlfw;
using ImGuiNET;
using Silk.NET.GLFW;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System.Runtime.InteropServices;

namespace DevilDaggersInfo.Tools.AppWindows;

public class MainAppWindow
{
	private readonly ImGuiController _imGuiController;

	public unsafe MainAppWindow()
	{
		Graphics.CreateWindow(new($"ddinfo tools {AssemblyUtils.EntryAssemblyVersionString}", UserCache.Model.WindowWidth, UserCache.Model.WindowHeight, false));

		// Always keep V-sync on to prevent missing inputs.
		// WindowInstance = Window.Create(WindowOptions.Default with { VSync = true });

		_imGuiController = new(Graphics.Gl, Input.GlfwInput, UserCache.Model.WindowWidth, UserCache.Model.WindowHeight);

		// {
		// 	ImGuiIOPtr io = ImGui.GetIO();
		//
		// 	// Add the default font first so it is actually used by default.
		// 	io.Fonts.AddFontDefault();
		//
		// 	string fontPath = Path.Combine(AssemblyUtils.InstallationDirectory, "goethebold.ttf");
		// 	Root.FontGoetheBold20 = io.Fonts.AddFontFromFileTTF(fontPath, 20);
		// 	Root.FontGoetheBold30 = io.Fonts.AddFontFromFileTTF(fontPath, 30);
		// 	Root.FontGoetheBold60 = io.Fonts.AddFontFromFileTTF(fontPath, 60);
		// }

		Graphics.Gl.ClearColor(0, 0, 0, 1);

		ConfigureImGui();
		Root.InternalResources = InternalResources.Create(Graphics.Gl);

		ConfigLayout.ValidateInstallation();

		int iconWidth = Root.InternalResources.ApplicationIconTexture.Width;
		int iconHeight = Root.InternalResources.ApplicationIconTexture.Height;
		IntPtr iconPtr = Marshal.AllocHGlobal(iconWidth * iconHeight * 4);
		Marshal.Copy(Root.InternalResources.ApplicationIconTexture.Pixels, 0, iconPtr, iconWidth * iconHeight * 4);
		Image image = new()
		{
			Width = iconWidth,
			Height = iconHeight,
			Pixels = (byte*)iconPtr,
		};
		Graphics.Glfw.SetWindowIcon(Graphics.Window, 1, &image);
	}

	private static void ConfigureImGui()
	{
		ImGuiStylePtr style = ImGui.GetStyle();
		style.ScrollbarSize = 16;
		style.ScrollbarRounding = 0;

		Colors.SetColors(Colors.Main);
	}

	// TODO
	private void OnWindowOnFramebufferResize(Vector2D<int> size)
	{
		Graphics.Gl.Viewport(size);

		UserCache.Model = UserCache.Model with
		{
			WindowWidth = size.X,
			WindowHeight = size.Y,
		};
	}

	// TODO
	public void Render(double delta)
	{
		float deltaF = (float)delta;

		Root.Application.RenderCounter.Increment();
		Root.Application.LastRenderDelta = deltaF;

		_imGuiController.Update(deltaF);

		Graphics.Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		UiRenderer.Render(deltaF);

		_imGuiController.Render();

		if (Ui.Main.MainWindow.ShouldClose)
		{
			_imGuiController.Destroy();
			Graphics.Gl.Dispose(); // TODO: Ok?
			Graphics.Glfw.Terminate();
		}
	}
}
