using DevilDaggersInfo.App.Ui.Practice.Main.Data;
using DevilDaggersInfo.App.Ui.Practice.Main.Templates;
using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Core.Spawnset.View;
using DevilDaggersInfo.Core.Wiki;
using ImGuiNET;

namespace DevilDaggersInfo.App.Ui.Practice.Main;

public static class EndLoopTemplatesChild
{
	private static readonly List<float> _endLoopTimerStarts = new();

	static EndLoopTemplatesChild()
	{
		const int endLoopTemplateWaveCount = 33;
		SpawnsView spawnsView = new(ContentManager.Content.DefaultSpawnset, GameVersion.V3_2, endLoopTemplateWaveCount);
		for (int i = 0; i < endLoopTemplateWaveCount; i++)
		{
			float timerStart;
			if (i == 0)
				timerStart = spawnsView.Waves[i][0].Seconds;
			else
				timerStart = spawnsView.Waves[i - 1][^1].Seconds + 0.1f; // Make sure we don't accidentally include the last enemy of the previous wave.

			_endLoopTimerStarts.Add(timerStart);
		}
	}

	public static void Render()
	{
		ImGui.BeginChild("End loop templates", PracticeWindow.TemplateContainerSize, true);
		ImGui.Text("End loop templates");

		ImGui.BeginChild("End loop template description", PracticeWindow.TemplateListSize with { Y = PracticeWindow.TemplateDescriptionHeight });
		ImGui.PushTextWrapPos(ImGui.GetCursorPos().X + PracticeWindow.TemplateWidth);
		ImGui.Text("The amount of homing for the end loop waves is set to 0. Use one that is realistic for you.");
		ImGui.PopTextWrapPos();
		ImGui.EndChild();

		ImGui.BeginChild("End loop template list", PracticeWindow.TemplateListSize);
		for (int i = 0; i < _endLoopTimerStarts.Count; i++)
			RenderEndLoopTemplate(i, _endLoopTimerStarts[i]);

		ImGui.EndChild();
		ImGui.EndChild();
	}

	private static void RenderEndLoopTemplate(int waveIndex, float timerStart)
	{
		EndLoopTemplateChild.Render(
			waveIndex: waveIndex,
			timerStart: timerStart,
			isActive: IsEqual(PracticeLogic.State, timerStart),
			buttonSize: new(PracticeWindow.TemplateWidth, 30),
			onClick: () =>
			{
				PracticeLogic.State = new(HandLevel.Level4, 0, timerStart);
				PracticeLogic.Apply();
			});

		static bool IsEqual(PracticeState state, float timerStart)
		{
			return state is { HandLevel: HandLevel.Level4, AdditionalGems: 0 } && Math.Abs(state.TimerStart - timerStart) < PracticeDataConstants.TimerStartTolerance;
		}
	}
}
