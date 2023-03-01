using DevilDaggersInfo.App.Core.GameMemory;
using DevilDaggersInfo.App.Ui.Base.Components;
using DevilDaggersInfo.App.Ui.Base.DependencyPattern;
using DevilDaggersInfo.App.Ui.Base.StateManagement;
using DevilDaggersInfo.App.Ui.Base.StateManagement.Base.Actions;
using DevilDaggersInfo.App.Ui.Base.StateManagement.CustomLeaderboardsRecorder.Actions;
using DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Components.Leaderboard;
using DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Components.LeaderboardList;
using DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Components.Recording;
using DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Components.Recording.Results;
using DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Components.State;
using Warp.NET.Ui;

namespace DevilDaggersInfo.App.Ui.CustomLeaderboardsRecorder.Layouts;

public class CustomLeaderboardsRecorderMainLayout : Layout, IExtendedLayout
{
	private readonly StateWrapper _stateWrapper;
	private readonly RecordingScrollArea _recordingScrollArea;
	private readonly RecordingResultScrollArea _recordingResultScrollArea;

	private int _recordingInterval;

	public CustomLeaderboardsRecorderMainLayout()
	{
		const int headerHeight = 24;
		const int stateWrapperBottom = 96;
		const int recordingScrollAreaBottom = 444;

		MainLayoutBackButton backButton = new(new PixelBounds(0, 0, 24, headerHeight), () => StateManager.Dispatch(new SetLayout(Root.Dependencies.MainLayout)));
		_stateWrapper = new(new PixelBounds(0, headerHeight, 256, stateWrapperBottom - headerHeight));
		_recordingScrollArea = new(new PixelBounds(0, stateWrapperBottom, 256, recordingScrollAreaBottom - stateWrapperBottom));
		_recordingResultScrollArea = new(new PixelBounds(0, stateWrapperBottom, 256, recordingScrollAreaBottom - stateWrapperBottom));
		LeaderboardListWrapper leaderboardListWrapper = new(new PixelBounds(256, headerHeight, 768, recordingScrollAreaBottom - headerHeight));
		LeaderboardWrapper leaderboardWrapper = new(new PixelBounds(0, recordingScrollAreaBottom, 1024, 768 - recordingScrollAreaBottom));

		NestingContext.Add(backButton);
		NestingContext.Add(_stateWrapper);
		NestingContext.Add(_recordingScrollArea);
		NestingContext.Add(_recordingResultScrollArea);
		NestingContext.Add(leaderboardListWrapper);
		NestingContext.Add(leaderboardWrapper);

		StateManager.Subscribe<SetLayout>(Initialize);
	}

	private void Initialize()
	{
		if (StateManager.LayoutState.CurrentLayout != Root.Dependencies.CustomLeaderboardsRecorderMainLayout)
			return;

		_stateWrapper.Initialize();

		StateManager.Dispatch(new LoadLeaderboardList());
	}

	public void Update()
	{
		_recordingInterval++;
		if (_recordingInterval < 5)
			return;

		_recordingInterval = 0;
		if (!RecordingLogic.Scan())
			return;

		_stateWrapper.SetState();

		bool gameMemoryInitialized = Root.Dependencies.GameMemoryService.IsInitialized;
		_recordingScrollArea.IsActive = gameMemoryInitialized && !StateManager.RecordingState.ShowUploadResponse && (GameStatus)Root.Dependencies.GameMemoryService.MainBlock.Status is not (GameStatus.Title or GameStatus.Menu or GameStatus.Lobby);
		if (_recordingScrollArea.IsActive)
			_recordingScrollArea.SetState();

		_recordingResultScrollArea.IsActive = gameMemoryInitialized && StateManager.RecordingState.ShowUploadResponse;

		RecordingLogic.Handle();
	}

	public void Render3d()
	{
	}

	public void Render()
	{
		const int border = 1;
		Root.Game.RectangleRenderer.Schedule(_recordingScrollArea.Bounds.Size, _recordingScrollArea.Bounds.Center, -2, Color.Purple);
		Root.Game.RectangleRenderer.Schedule(_recordingScrollArea.Bounds.Size - new Vector2i<int>(border * 2), _recordingScrollArea.Bounds.Center, -1, Color.Gray(0.1f));
	}
}
