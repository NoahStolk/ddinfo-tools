using DevilDaggersInfo.Core.Common;
using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Core.Spawnset.View;
using DevilDaggersInfo.Core.Wiki;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.Practice.Main.Data;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.Practice.Main;

public sealed class EndLoopTemplatesChild
{
	private readonly List<float> _endLoopTimerStarts = CreateEndLoopTimerStarts();

	private static List<float> CreateEndLoopTimerStarts()
	{
		const int endLoopTemplateWaveCount = 33;

		List<float> endLoopTimerStarts = [];
		SpawnsView spawnsView = new(ContentManager.Content.DefaultSpawnset, GameVersion.V3_2, endLoopTemplateWaveCount);
		for (int i = 0; i < endLoopTemplateWaveCount; i++)
		{
			float timerStart;
			if (i == 0)
				timerStart = spawnsView.Waves[i][0].Seconds;
			else
				timerStart = spawnsView.Waves[i - 1][^1].Seconds + 0.1f; // Make sure we don't accidentally include the last enemy of the previous wave.

			endLoopTimerStarts.Add(timerStart);
		}

		return endLoopTimerStarts;
	}

	public void Render(Vector2 templateContainerSize, Vector2 templateListSize, float templateWidth)
	{
		if (ImGui.BeginChild("EndLoopTemplates", templateContainerSize, ImGuiChildFlags.Border))
		{
			ImGui.Text("End loop templates");

			if (ImGui.BeginChild("EndLoopTemplateDescription", templateListSize with { Y = PracticeWindow.TemplateDescriptionHeight }))
			{
				ImGui.PushTextWrapPos(ImGui.GetCursorPos().X + templateWidth);
				ImGui.Text("The amount of homing for the end loop waves is set to 0. You can fill in your preferred homing count and save it as a template.");
				ImGui.PopTextWrapPos();
			}

			ImGui.EndChild();

			if (ImGui.BeginChild("EndLoopTemplateList", templateListSize))
			{
				for (int i = 0; i < _endLoopTimerStarts.Count; i++)
					RenderEndLoopTemplate(i, _endLoopTimerStarts[i], templateWidth);
			}

			ImGui.EndChild();
		}

		ImGui.EndChild();
	}

	private static void RenderEndLoopTemplate(int waveIndex, float timerStart, float templateWidth)
	{
		Vector2 buttonSize = new(templateWidth, 30);
		(byte backgroundAlpha, byte textAlpha) = PracticeWindow.GetAlpha(PracticeLogic.IsActive(HandLevel.Level4, 0, timerStart));
		Color color = waveIndex % 3 == 2 ? EnemiesV3_2.Ghostpede.Color.ToEngineColor() : EnemiesV3_2.Gigapede.Color.ToEngineColor();

		ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
		if (ImGui.BeginChild(Inline.Span($"Wave{waveIndex + 1}"), buttonSize, ImGuiChildFlags.Border))
		{
			bool hover = ImGui.IsWindowHovered();
			ImGui.PushStyleColor(ImGuiCol.ChildBg, color with { A = (byte)(hover ? backgroundAlpha + 16 : backgroundAlpha) });

			if (ImGui.BeginChild(Inline.Span($"WaveChild{waveIndex + 1}"), buttonSize, ImGuiChildFlags.None, ImGuiWindowFlags.NoInputs))
			{
				if (hover && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
				{
					PracticeLogic.State = new PracticeState(HandLevel.Level4, 0, timerStart);
					PracticeLogic.GenerateAndApplyPracticeSpawnset();
				}

				ImGui.SetCursorPos(ImGui.GetCursorPos() + new Vector2(8, 8));

				ImGui.TextColored(color with { A = textAlpha }, Inline.Span($"Wave {waveIndex + 1}"));
				ImGui.SameLine(ImGui.GetWindowWidth() - ImGui.CalcTextSize(Inline.Span(timerStart, StringFormats.TimeFormat)).X - 8);
				ImGui.TextColored(Color.White with { A = textAlpha }, Inline.Span(timerStart, StringFormats.TimeFormat));
			}

			ImGui.EndChild();

			ImGui.PopStyleColor();
		}

		ImGui.PopStyleVar();

		ImGui.EndChild();
	}
}
