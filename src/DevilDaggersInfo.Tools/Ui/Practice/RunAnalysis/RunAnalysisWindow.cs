using DevilDaggersInfo.Tools.Ui.Practice.RunAnalysis.Data;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.Practice.RunAnalysis;

internal sealed class RunAnalysisWindow(GameMemoryServiceWrapper gameMemoryServiceWrapper)
{
	private float _recordingTimer;

	public PracticeStatsData StatsData { get; } = new();

	public void Update(float delta)
	{
		_recordingTimer += delta;
		if (_recordingTimer < 0.5f)
			return;

		_recordingTimer = 0;
		if (!gameMemoryServiceWrapper.Scan() || !Root.GameMemoryService.IsInitialized)
			return;

		StatsData.Populate();
		GraphsChild.Update(StatsData.Statistics);
	}

	public void Render()
	{
		ImGuiUtils.SetNextWindowMinSize(512, 1024);
		if (ImGui.Begin("Run Analysis"))
		{
			SplitsChild.Render(StatsData);
			GraphsChild.Render(StatsData);
		}

		ImGui.End();
	}
}
