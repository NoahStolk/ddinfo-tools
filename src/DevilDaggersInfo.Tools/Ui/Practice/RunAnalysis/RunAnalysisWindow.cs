using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.GameMemory;
using DevilDaggersInfo.Tools.Ui.Practice.RunAnalysis.Data;
using Hexa.NET.ImGui;

namespace DevilDaggersInfo.Tools.Ui.Practice.RunAnalysis;

internal sealed class RunAnalysisWindow(GameMemoryServiceWrapper gameMemoryServiceWrapper, FontService fontService, GameMemoryService gameMemoryService)
{
	private readonly GraphsChild _graphsChild = new(fontService);
	private readonly SplitsChild _splitsChild = new(fontService);

	private readonly PracticeStatsData _statsData = new();

	private float _recordingTimer;

	public void Update(float delta)
	{
		_recordingTimer += delta;
		if (_recordingTimer < 0.5f)
			return;

		_recordingTimer = 0;
		if (!gameMemoryServiceWrapper.Scan() || !gameMemoryService.IsInitialized)
			return;

		_statsData.Populate(gameMemoryService);
		_graphsChild.Update(_statsData.Statistics);
	}

	public void Render()
	{
		ImGui.SetNextWindowMinSize(512, 1024);
		if (ImGui.Begin("Run Analysis"))
		{
			_splitsChild.Render(_statsData);
			_graphsChild.Render(_statsData);
		}

		ImGui.End();
	}
}
