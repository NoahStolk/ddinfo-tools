using DevilDaggersInfo.Core.Replay.PostProcessing.ReplaySimulation;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Scenes;
using DevilDaggersInfo.Tools.Ui.ReplayEditor.Utils;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.ReplayEditor;

public static class ReplayEditor3DWindow
{
	private static readonly FramebufferData _framebufferData = new();

	private static float _time;

	private static ArenaScene? _arenaScene;

	public static ArenaScene ArenaScene => _arenaScene ?? throw new InvalidOperationException("Scenes are not initialized.");

	public static void InitializeScene()
	{
		_arenaScene = new(static () => FileStates.Replay.Object.Header.Spawnset, false, false);
	}

	public static void Reset()
	{
		_time = 0;
	}

	public static void Update(float delta)
	{
		if (_time < FileStates.Replay.Object.Header.Time)
			_time += delta;

		ArenaScene.CurrentTick = TimeUtils.TimeToTick(_time, 0);
	}

	public static void Render(float delta)
	{
		ImGuiUtils.SetNextWindowMinSize(Constants.MinWindowSize / 2);
		if (ImGui.Begin("3D Replay Viewer"))
		{
			if (ImGui.IsMouseDown(ImGuiMouseButton.Right) && ImGui.IsWindowHovered())
				ImGui.SetWindowFocus();

			ImGui.Text(StringResources.ReplaySimulator3D);
			ImGui.SliderFloat("Time", ref _time, 0, FileStates.Replay.Object.Header.Time, "%.4f", ImGuiSliderFlags.NoInput);

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

		ImGui.End(); // End 3D Replay Viewer
	}
}
