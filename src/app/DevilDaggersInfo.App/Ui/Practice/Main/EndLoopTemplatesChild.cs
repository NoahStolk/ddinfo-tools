using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Extensions;
using DevilDaggersInfo.App.ZeroAllocation;
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
	private static readonly IdBuffer _idBuffer = new(32);
	private static readonly IdBuffer _idBufferChild = new(32);

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
		if (ImGui.BeginChild("EndLoopTemplates", PracticeWindow.TemplateContainerSize, true))
		{
			ImGui.Text("End loop templates");

			if (ImGui.BeginChild("EndLoopTemplateDescription", PracticeWindow.TemplateListSize with { Y = PracticeWindow.TemplateDescriptionHeight }))
			{
				ImGui.PushTextWrapPos(ImGui.GetCursorPos().X + PracticeWindow.TemplateWidth);
				ImGui.Text("The amount of homing for the end loop waves is set to 0. You can fill in your preferred homing count and save it as a template.");
				ImGui.PopTextWrapPos();
			}

			ImGui.EndChild(); // End EndLoopTemplateDescription

			if (ImGui.BeginChild("EndLoopTemplateList", PracticeWindow.TemplateListSize))
			{
				for (int i = 0; i < _endLoopTimerStarts.Count; i++)
					RenderEndLoopTemplate(i, _endLoopTimerStarts[i]);
			}

			ImGui.EndChild(); // End EndLoopTemplateList
		}

		ImGui.EndChild(); // End EndLoopTemplates
	}

	private static void RenderEndLoopTemplate(int waveIndex, float timerStart)
	{
		Vector2 buttonSize = new(PracticeWindow.TemplateWidth, 30);
		_idBuffer.Overwrite("Wave", waveIndex + 1);
		(byte backgroundAlpha, byte textAlpha) = PracticeWindow.GetAlpha(PracticeLogic.IsActive(HandLevel.Level4, 0, timerStart));
		Color color = waveIndex % 3 == 2 ? EnemiesV3_2.Ghostpede.Color.ToEngineColor() : EnemiesV3_2.Gigapede.Color.ToEngineColor();

		ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
		if (ImGui.BeginChild(_idBuffer, buttonSize, true))
		{
			bool hover = ImGui.IsWindowHovered();
			ImGui.PushStyleColor(ImGuiCol.ChildBg, color with { A = (byte)(hover ? backgroundAlpha + 16 : backgroundAlpha) });

			_idBufferChild.Overwrite("WaveChild", waveIndex + 1);
			if (ImGui.BeginChild(_idBufferChild, buttonSize, false, ImGuiWindowFlags.NoInputs))
			{
				if (hover && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
				{
					PracticeLogic.State = new(HandLevel.Level4, 0, timerStart);
					PracticeLogic.GenerateAndApplyPracticeSpawnset();
				}

				ImGui.SetCursorPos(ImGui.GetCursorPos() + new Vector2(8, 8));

				ReadOnlySpan<char> topRightTextString = UnsafeSpan.Get(timerStart, StringFormats.TimeFormat);

				ImGui.TextColored(color with { A = textAlpha }, _idBuffer);
				ImGui.SameLine(ImGui.GetWindowWidth() - ImGui.CalcTextSize(topRightTextString).X - 8);
				ImGui.TextColored(Color.White with { A = textAlpha }, topRightTextString);
			}

			ImGui.EndChild(); // End WaveChild{waveIndex + 1}

			ImGui.PopStyleColor();
		}

		ImGui.PopStyleVar();

		ImGui.EndChild(); // End Wave{waveIndex + 1}
	}
}
