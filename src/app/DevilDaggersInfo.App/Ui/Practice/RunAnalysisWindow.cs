using ImGuiNET;

namespace DevilDaggersInfo.App.Ui.Practice;

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
		GemCollectionChild.Update(StatsData.Statistics);
	}

	public static void Render()
	{
		ImGui.SetNextWindowSize(new(512, 512), ImGuiCond.Always);
		if (ImGui.Begin("Run Analysis", ImGuiWindowFlags.NoResize))
		{
			SplitsChild.Render();
			GemCollectionChild.Render();

			ImGui.End();
		}
	}
}
