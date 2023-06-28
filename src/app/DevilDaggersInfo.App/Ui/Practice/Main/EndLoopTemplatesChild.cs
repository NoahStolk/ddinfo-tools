using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Extensions;
using DevilDaggersInfo.App.Ui.Practice.Main.Data;
using DevilDaggersInfo.Common;
using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Core.Spawnset.View;
using DevilDaggersInfo.Core.Wiki;
using ImGuiNET;
using System.Numerics;

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
		ImGui.Text("The amount of homing for the end loop waves is set to 0. You can fill in your preferred homing count and save it as a template.");
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
		Vector2 buttonSize = new(PracticeWindow.TemplateWidth, 30);
		string waveName = $"Wave {waveIndex + 1}";
		(byte backgroundAlpha, byte textAlpha) = PracticeWindow.GetAlpha(PracticeLogic.IsActive(HandLevel.Level4, 0, timerStart));
		Color color = waveIndex % 3 == 2 ? EnemiesV3_2.Ghostpede.Color.ToEngineColor() : EnemiesV3_2.Gigapede.Color.ToEngineColor();

		ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
		if (ImGui.BeginChild(waveName, buttonSize, true))
		{
			bool hover = ImGui.IsWindowHovered();
			ImGui.PushStyleColor(ImGuiCol.ChildBg, color with { A = (byte)(hover ? backgroundAlpha + 16 : backgroundAlpha) });

			if (ImGui.BeginChild(waveName + " child", buttonSize, false, ImGuiWindowFlags.NoInputs))
			{
				if (hover && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
				{
					PracticeLogic.State = new(HandLevel.Level4, 0, timerStart);
					PracticeLogic.GenerateAndApplyPracticeSpawnset();
				}

				ImGui.SetCursorPos(ImGui.GetCursorPos() + new Vector2(8, 8));

				string topRightTextString = timerStart.ToString(StringFormats.TimeFormat);

				ImGui.TextColored(color with { A = textAlpha }, waveName);
				ImGui.SameLine(ImGui.GetWindowWidth() - ImGui.CalcTextSize(topRightTextString).X - 8);
				ImGui.TextColored(Color.White with { A = textAlpha }, topRightTextString);
			}

			ImGui.EndChild();

			ImGui.PopStyleColor();
		}

		ImGui.PopStyleVar();

		ImGui.EndChild();
	}
}
