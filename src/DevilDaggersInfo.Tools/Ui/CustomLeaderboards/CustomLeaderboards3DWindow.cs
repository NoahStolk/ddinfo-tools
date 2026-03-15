using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.PostProcessing.ReplaySimulation;
using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Scenes;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Utils;
using DevilDaggersInfo.Tools.User.Settings;
using ImGuiNET;
using Serilog.Core;
using Silk.NET.GLFW;
using Silk.NET.OpenGL;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.CustomLeaderboards;

internal sealed unsafe class CustomLeaderboards3DWindow(
	Glfw glfw,
	GL gl,
	WindowHandle* window,
	GlfwInput glfwInput,
	ResourceManager resourceManager,
	FileStates fileStates,
	SpawnsetSaver spawnsetSaver,
	UserSettings userSettings,
	Logger logger,
	ContentManager contentManager)
{
	private readonly FramebufferData _framebufferData = new(gl, logger);

	private float _time;

	private SpawnsetBinary _spawnset = SpawnsetBinary.CreateDefault();

	private ArenaScene? _arenaScene;

	private ArenaScene ArenaScene => _arenaScene ?? throw new InvalidOperationException("Scenes are not initialized.");

	public void InitializeScene()
	{
		_arenaScene = new ArenaScene(gl, resourceManager, contentManager, glfw, window, glfwInput, () => _spawnset, false, false, fileStates, spawnsetSaver, userSettings);
	}

	public void LoadReplay(ReplayBinary<LocalReplayBinaryHeader> replayBinary)
	{
		_time = 0;
		_spawnset = replayBinary.Header.Spawnset;

		ReplaySimulation replaySimulation = ReplaySimulationBuilder.Build(replayBinary);
		ArenaScene.SetPlayerMovement(replaySimulation);
	}

	public void Update(float delta)
	{
		if (_time < ArenaScene.ReplaySimulation?.InputSnapshots.Count / 60f)
			_time += delta;

		ArenaScene.CurrentTick = (int)MathF.Round(_time * 60);
	}

	public void Render(float delta)
	{
		ImGuiUtils.SetNextWindowMinSize(Constants.MinWindowSize / 2);
		if (ImGui.Begin("3D Replay Viewer"))
		{
			if (ImGui.IsMouseDown(ImGuiMouseButton.Right) && ImGui.IsWindowHovered())
				ImGui.SetWindowFocus();

			float textHeight = ImGui.CalcTextSize(StringResources.ReplaySimulator3D).Y;

			Vector2 framebufferSize = ImGui.GetWindowSize() - new Vector2(16, 48 + textHeight);
			_framebufferData.ResizeIfNecessary((int)framebufferSize.X, (int)framebufferSize.Y);

			Vector2 cursorScreenPos = ImGui.GetCursorScreenPos() + new Vector2(0, textHeight);
			ArenaScene.Camera.FramebufferOffset = cursorScreenPos;

			bool isWindowFocused = ImGui.IsWindowFocused();
			bool isMouseOverFramebuffer = isWindowFocused && ImGui.IsWindowHovered() && ImGui.IsMouseHoveringRect(cursorScreenPos, cursorScreenPos + framebufferSize);
			_framebufferData.RenderArena(isMouseOverFramebuffer, isWindowFocused, delta, ArenaScene);

			ImDrawListPtr drawList = ImGui.GetWindowDrawList();
			drawList.AddFramebufferImage(_framebufferData, cursorScreenPos, cursorScreenPos + new Vector2(_framebufferData.Width, _framebufferData.Height));

			ImGui.Text(StringResources.ReplaySimulator3D);
		}

		ImGui.End();
	}
}
