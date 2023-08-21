using DevilDaggersInfo.App.Scenes;
using DevilDaggersInfo.Core.Replay;
using DevilDaggersInfo.Core.Replay.PostProcessing.ReplaySimulation;
using DevilDaggersInfo.Core.Spawnset;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui.CustomLeaderboards;

public static class CustomLeaderboards3DWindow
{
	private static readonly FramebufferData _framebufferData = new();

	private static float _time;

	private static SpawnsetBinary _spawnset = SpawnsetBinary.CreateDefault();

	private static ArenaScene? _arenaScene;

	private static ArenaScene ArenaScene => _arenaScene ?? throw new InvalidOperationException("Scenes are not initialized.");

	public static void InitializeScene()
	{
		_arenaScene = new(static () => _spawnset, false, false);
	}

	public static void LoadReplay(ReplayBinary<LocalReplayBinaryHeader> replayBinary)
	{
		_time = 0;
		_spawnset = replayBinary.Header.Spawnset;

		ReplaySimulation replaySimulation = ReplaySimulationBuilder.Build(replayBinary);
		ArenaScene.SetPlayerMovement(replaySimulation);
	}

	public static void Update(float delta)
	{
		if (_time < ArenaScene.ReplaySimulation?.InputSnapshots.Count / 60f)
			_time += delta;

		ArenaScene.CurrentTick = (int)MathF.Round(_time * 60);
	}

	public static void Render(float delta)
	{
		ImGui.PushStyleVar(ImGuiStyleVar.WindowMinSize, Constants.MinWindowSize / 2);
		if (ImGui.Begin("3D Replay Viewer"))
		{
			float textHeight = ImGui.CalcTextSize(StringResources.ReplaySimulator3D).Y;

			Vector2 framebufferSize = ImGui.GetWindowSize() - new Vector2(32, 48 + textHeight);
			_framebufferData.ResizeIfNecessary((int)framebufferSize.X, (int)framebufferSize.Y);

			Vector2 cursorScreenPos = ImGui.GetCursorScreenPos() + new Vector2(0, textHeight);
			ArenaScene.Camera.FramebufferOffset = cursorScreenPos;

			_framebufferData.RenderArena(delta, ArenaScene);

			ImDrawListPtr drawList = ImGui.GetWindowDrawList();
			drawList.AddFramebufferImage(_framebufferData, cursorScreenPos, cursorScreenPos + new Vector2(_framebufferData.Width, _framebufferData.Height));

			ImGui.Text(StringResources.ReplaySimulator3D);

			ImGui.End();
		}

		ImGui.PopStyleVar();
	}
}
