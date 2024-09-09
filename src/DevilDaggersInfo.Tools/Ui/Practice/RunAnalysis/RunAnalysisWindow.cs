using DevilDaggersInfo.Tools.Ui.Practice.RunAnalysis.Data;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.Practice.RunAnalysis;

public static class RunAnalysisWindow
{
	private static float _recordingTimer;

	public static PracticeStatsData StatsData { get; } = new();

	public static void Update(float delta)
	{
		_recordingTimer += delta;
		if (_recordingTimer < 0.5f)
			return;

		_recordingTimer = 0;
		if (!GameMemoryServiceWrapper.Scan() || !Root.GameMemoryService.IsInitialized)
			return;

		StatsData.Populate();
		GraphsChild.Update(StatsData.Statistics);
	}

	public static void Render()
	{
		ImGuiUtils.SetNextWindowMinSize(512, 1024);
		if (ImGui.Begin("Run Analysis"))
		{
			SplitsChild.Render();
			GraphsChild.Render();
		}

		ImGui.End();
	}
}
