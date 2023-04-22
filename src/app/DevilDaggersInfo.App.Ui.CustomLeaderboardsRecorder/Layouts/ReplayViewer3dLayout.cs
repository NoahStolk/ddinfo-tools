using DevilDaggersInfo.App.Engine;
using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Engine.Text;
using DevilDaggersInfo.App.Engine.Ui;
using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.StateManagement.Base.Actions;
using DevilDaggersInfo.App.Ui.Base.StateManagement.CustomLeaderboardsRecorder.Actions;
using DevilDaggersInfo.App.Ui.Base.Styling;
using DevilDaggersInfo.App.Ui.Base.Utils;
using DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Scenes;
using DevilDaggersInfo.Core.Replay.PostProcessing.ReplaySimulation;
using DevilDaggersInfo.Core.Spawnset;
using Silk.NET.GLFW;

namespace DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Layouts;

public class ReplayViewer3dLayout : Layout, IExtendedLayout
{
	private readonly Slider _timeSlider;
	private readonly GhostsArenaScene _arenaScene = new();

	private int _currentTick;

	public ReplayViewer3dLayout()
	{
		_timeSlider = new(new PixelBounds(0, 752, 1024, 16), f => _currentTick = (int)(f * 60), true, 0, 0, 0.001f, 0, SliderStyles.Default);
		NestingContext.Add(_timeSlider);

		StateManager.Subscribe<BuildReplayScene>(BuildScene);
	}

	private void BuildScene()
	{
		if (StateManager.ReplaySceneState.ReplayBinaries.Length == 0)
			throw new InvalidOperationException("Cannot build replay scene without replay binaries.");

		if (!SpawnsetBinary.TryParse(StateManager.ReplaySceneState.ReplayBinaries[0].Header.SpawnsetBuffer, out SpawnsetBinary? spawnset))
			throw new InvalidOperationException("Spawnset inside replay is invalid.");

		_currentTick = 0;

		_timeSlider.Max = StateManager.ReplaySceneState.ReplayBinaries.Max(rb => rb.EventsData.TickCount / 60f);
		_timeSlider.CurrentValue = Math.Clamp(_timeSlider.CurrentValue, 0, _timeSlider.Max);

		_arenaScene.BuildSpawnset(spawnset);

		ReplaySimulation replaySimulation = ReplaySimulationBuilder.Build(spawnset, StateManager.ReplaySceneState.ReplayBinaries[0]);

		_arenaScene.BuildPlayerMovement(replaySimulation);
	}

	public unsafe void Update()
	{
		_currentTick++;
		_timeSlider.CurrentValue = _currentTick / 60f;

		_arenaScene.Update(_currentTick);

		if (Input.IsKeyPressed(Keys.Escape))
		{
			Graphics.Glfw.SetInputMode(Window, CursorStateAttribute.Cursor, CursorModeValue.CursorNormal);
			StateManager.Dispatch(new SetLayout(Root.Dependencies.CustomLeaderboardsRecorderMainLayout));
		}
	}

	public void Render3d()
	{
		_arenaScene.Render();
	}

	public void Render()
	{
		Root.Game.MonoSpaceFontRenderer24.Schedule(Vector2i<int>.One, new(8), 1000, Color.HalfTransparentWhite, StringResources.ReplaySimulator, TextAlign.Left);
	}
}
