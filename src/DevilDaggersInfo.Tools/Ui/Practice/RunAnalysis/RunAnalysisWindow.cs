using DevilDaggersInfo.Tools.GameMemory;
using DevilDaggersInfo.Tools.NativeInterface.Services;
using DevilDaggersInfo.Tools.Ui.Practice.RunAnalysis.Data;
using Hexa.NET.ImGui;

namespace DevilDaggersInfo.Tools.Ui.Practice.RunAnalysis;

internal sealed class RunAnalysisWindow(GameMemoryServiceWrapper gameMemoryServiceWrapper, FontService fontService, GameMemoryService gameMemoryService)
{
	private readonly GraphsChild _graphsChild = new(fontService);
	private readonly SplitsChild _splitsChild = new(fontService);

	private float _recordingTimer;

	public PracticeStatsData StatsData { get; } = new();

	public void Update(float delta)
	{
		_recordingTimer += delta;
		if (_recordingTimer < 0.5f)
			return;

		_recordingTimer = 0;
		if (!gameMemoryServiceWrapper.Scan() || !gameMemoryService.IsInitialized)
			return;

		StatsData.Populate(gameMemoryService);
		_graphsChild.Update(StatsData.Statistics);
	}

	public void Render()
	{
		ImGuiUtils.SetNextWindowMinSize(512, 1024);
		if (ImGui.Begin("Run Analysis"))
		{
			// Windows and Linux only ever resolve to Unresolved or Resolved, so this only ever substitutes the
			// splits/graphs children for a status message on macOS - their "game not running" screen (Unresolved,
			// same as before this branch existed) is untouched.
			if (gameMemoryService.BlockAddressStatus is BlockAddressStatus.MemoryUnreadable or BlockAddressStatus.BlockNotFound)
			{
				ImGui.TextWrapped(Inline.Utf8(gameMemoryServiceWrapper.DescribeUnavailability()));
			}
			else
			{
				_splitsChild.Render(StatsData);
				_graphsChild.Render(StatsData);
			}
		}

		ImGui.End();
	}
}
