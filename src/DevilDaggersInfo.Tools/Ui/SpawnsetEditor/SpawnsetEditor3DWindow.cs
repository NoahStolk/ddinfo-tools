using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Scenes;
using ImGuiGlfw;
using ImGuiNET;
using Silk.NET.GLFW;
using Silk.NET.OpenGL;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.SpawnsetEditor;

public sealed unsafe class SpawnsetEditor3DWindow
{
	private readonly Glfw _glfw;
	private readonly GL _gl;
	private readonly WindowHandle* _window;
	private readonly GlfwInput _glfwInput;
	private readonly ResourceManager _resourceManager;
	private readonly FramebufferData _framebufferData;

	private ArenaScene? _arenaScene;

	public SpawnsetEditor3DWindow(Glfw glfw, GL gl, WindowHandle* window, GlfwInput glfwInput, ResourceManager resourceManager)
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
		_arenaScene = new ArenaScene(_glfw, _gl, _window, _glfwInput, _resourceManager, static () => FileStates.Spawnset.Object, false, true);
	}

	public void Render(float delta)
	{
		ImGuiUtils.SetNextWindowMinSize(Constants.MinWindowSize / 2);
		if (ImGui.Begin("3D Arena Editor"))
		{
			if (ImGui.IsMouseDown(ImGuiMouseButton.Right) && ImGui.IsWindowHovered())
				ImGui.SetWindowFocus();

			float textHeight = ImGui.CalcTextSize(StringResources.SpawnsetEditor3D).Y;
			ImGui.Text(StringResources.SpawnsetEditor3D);

			Vector2 framebufferSize = ImGui.GetWindowSize() - new Vector2(16, 48 + textHeight);
			_framebufferData.ResizeIfNecessary((int)framebufferSize.X, (int)framebufferSize.Y);

			Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
			ArenaScene.Camera.FramebufferOffset = cursorScreenPos;

			bool isWindowFocused = ImGui.IsWindowFocused();
			bool isMouseOverFramebuffer = ImGui.IsMouseHoveringRect(cursorScreenPos, cursorScreenPos + framebufferSize);
			_framebufferData.RenderArena(isWindowFocused && isMouseOverFramebuffer, isWindowFocused, delta, ArenaScene);

			ImDrawListPtr drawList = ImGui.GetWindowDrawList();
			drawList.AddFramebufferImage(_framebufferData, cursorScreenPos, cursorScreenPos + new Vector2(_framebufferData.Width, _framebufferData.Height), isWindowFocused ? Color.White : Color.Gray(0.5f));

			if (ArenaScene.CurrentTick != 0)
			{
				const int padding = 8;
				drawList.AddText(ImGui.GetCursorScreenPos() + new Vector2(padding, padding), ImGui.GetColorU32(Color.Yellow), "(!) Editing is disabled because the shrink preview is active.");
			}

			// Prevent the window from being dragged when clicking on the 3D editor.
			ImGui.InvisibleButton("invisible", new Vector2(_framebufferData.Width, _framebufferData.Height));
		}

		ImGui.End();
	}
}
