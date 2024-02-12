using DevilDaggersInfo.Tools.Engine;
using DevilDaggersInfo.Tools.Ui.Config;
using ImGuiGlfw;
using Silk.NET.GLFW;
using Silk.NET.OpenGL;
using System.Runtime.InteropServices;

namespace DevilDaggersInfo.Tools;

public class Application
{
	private const double _maxMainDelta = 0.25;
	private const double _mainLoopLength = 1 / 300.0;

	private readonly ImGuiController _imGuiController;

	private double _currentTime = Graphics.Glfw.GetTime();
	private double _frameTime;

	private int _currentSecond;
	private int _renders;

	public unsafe Application(ImGuiController imGuiController)
	{
		_imGuiController = imGuiController;

		Root.InternalResources = InternalResources.Create(Graphics.Gl);

		ConfigLayout.ValidateInstallation();

		int iconWidth = Root.InternalResources.ApplicationIconTexture.Width;
		int iconHeight = Root.InternalResources.ApplicationIconTexture.Height;

		// TODO: Free iconPtr when no longer needed.
		IntPtr iconPtr = Marshal.AllocHGlobal(iconWidth * iconHeight * 4);
		Marshal.Copy(Root.InternalResources.ApplicationIconTexture.Pixels, 0, iconPtr, iconWidth * iconHeight * 4);
		Image image = new()
		{
			Width = iconWidth,
			Height = iconHeight,
			Pixels = (byte*)iconPtr,
		};
		Graphics.Glfw.SetWindowIcon(Graphics.Window, 1, &image);

		Root.Application = this;
	}

	public int Fps { get; private set; }
	public float FrameTime => (float)_frameTime;
	public float TotalTime { get; private set; }

	public PerSecondCounter RenderCounter { get; } = new();
	public float LastRenderDelta { get; set; }

	public unsafe void Run()
	{
		while (!Graphics.Glfw.WindowShouldClose(Graphics.Window))
		{
			double expectedNextFrame = Graphics.Glfw.GetTime() + _mainLoopLength;
			Main();

			while (Graphics.Glfw.GetTime() < expectedNextFrame)
				Thread.Yield();
		}

		_imGuiController.Destroy();
		Graphics.Gl.Dispose(); // TODO: Ok?
		Graphics.Glfw.Terminate();
	}

	private unsafe void Main()
	{
		double mainStartTime = Graphics.Glfw.GetTime();
		if (_currentSecond != (int)mainStartTime)
		{
			Fps = _renders;
			_renders = 0;
			_currentSecond = (int)mainStartTime;
		}

		_frameTime = mainStartTime - _currentTime;
		if (_frameTime > _maxMainDelta)
			_frameTime = _maxMainDelta;

		TotalTime += FrameTime;

		_currentTime = mainStartTime;

		Graphics.Glfw.PollEvents();

		Render();
		_renders++;

		Graphics.Glfw.SwapBuffers(Graphics.Window);
	}

	private void Render()
	{
		float deltaF = (float)_frameTime;

		Root.Application.RenderCounter.Increment();
		Root.Application.LastRenderDelta = deltaF;

		_imGuiController.Update(deltaF);

		// TODO: ImGui.DockSpaceOverViewport(null, ImGuiDockNodeFlags.PassthruCentralNode);

		// TODO: Test.
		Graphics.Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		UiRenderer.Render(deltaF);

		_imGuiController.Render();

		Input.GlfwInput.PostRender();

		if (Ui.Main.MainWindow.ShouldClose)
		{
			// TODO: Test.
			unsafe
			{
				Graphics.Glfw.SetWindowShouldClose(Graphics.Window, true);
			}
		}
	}
}
