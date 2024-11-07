using DevilDaggersInfo.Core.Replay.PostProcessing.ReplaySimulation;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Scenes;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Utils;
using ImGuiGlfw;
using ImGuiNET;
using Silk.NET.GLFW;
using Silk.NET.OpenGL;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor;

public sealed unsafe class ReplayEditor3DWindow
{
	private readonly Glfw _glfw;
	private readonly GL _gl;
	private readonly WindowHandle* _window;
	private readonly GlfwInput _glfwInput;
	private readonly ResourceManager _resourceManager;
	private readonly FramebufferData _framebufferData;

	private float _time;

	private ArenaScene? _arenaScene;

	public ReplayEditor3DWindow(Glfw glfw, GL gl, WindowHandle* window, GlfwInput glfwInput, ResourceManager resourceManager)
	{
		_glfw = glfw;
		_gl = gl;
		_window = window;
		_glfwInput = glfwInput;
		_resourceManager = resourceManager;
		_framebufferData = new FramebufferData(gl);
	}

	public ArenaScene ArenaScene => _arenaScene ?? throw new InvalidOperationException("Scenes are not initialized.");

	public void InitializeScene()
	{
		_arenaScene = new ArenaScene(_glfw, _gl, _window, _glfwInput, _resourceManager, static () => FileStates.Replay.Object.Spawnset, false, false);
	}

	public void Reset()
	{
		_time = 0;
	}

	public void Update(float delta)
	{
		if (_time < FileStates.Replay.Object.Time)
			_time += delta;

		ArenaScene.CurrentTick = TimeUtils.TimeToTick(_time, 0);
	}

	public void Render(float delta)
	{
		ImGuiUtils.SetNextWindowMinSize(Constants.MinWindowSize / 2);
		if (ImGui.Begin("3D Replay Viewer"))
		{
			if (ImGui.IsMouseDown(ImGuiMouseButton.Right) && ImGui.IsWindowHovered())
				ImGui.SetWindowFocus();

			ImGui.Text(StringResources.ReplaySimulator3D);
			ImGui.SliderFloat("Time", ref _time, 0, FileStates.Replay.Object.Time, "%.4f", ImGuiSliderFlags.NoInput);

			PlayerInputSnapshot snapshot = default;
			if (ArenaScene.CurrentTick < ArenaScene.ReplaySimulation?.InputSnapshots.Count)
				snapshot = ArenaScene.ReplaySimulation.InputSnapshots[ArenaScene.CurrentTick];

			Vector2 origin = ImGui.GetCursorScreenPos();
			ReplayInputs.Render(origin, snapshot);

			const float framebufferYOffset = 96;
			const float framebufferYSizeDecrement = 276;

			Vector2 framebufferSize = ImGui.GetWindowSize() - new Vector2(16, framebufferYSizeDecrement);
			_framebufferData.ResizeIfNecessary((int)framebufferSize.X, (int)framebufferSize.Y);

			Vector2 cursorScreenPos = ImGui.GetCursorScreenPos() + new Vector2(0, framebufferYOffset);
			ArenaScene.Camera.FramebufferOffset = cursorScreenPos;

			bool isWindowFocused = ImGui.IsWindowFocused();
			bool isMouseOverFramebuffer = isWindowFocused && ImGui.IsWindowHovered() && ImGui.IsMouseHoveringRect(cursorScreenPos, cursorScreenPos + framebufferSize);
			_framebufferData.RenderArena(isMouseOverFramebuffer, isWindowFocused, delta, ArenaScene);

			ImDrawListPtr drawList = ImGui.GetWindowDrawList();
			drawList.AddFramebufferImage(_framebufferData, cursorScreenPos, cursorScreenPos + new Vector2(_framebufferData.Width, _framebufferData.Height));
		}

		ImGui.End();
	}
}
