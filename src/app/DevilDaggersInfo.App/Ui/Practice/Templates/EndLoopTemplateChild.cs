using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Extensions;
using DevilDaggersInfo.Common;
using DevilDaggersInfo.Core.Wiki;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui.Practice.Templates;

public static class EndLoopTemplateChild
{
	public static void Render(
		int waveIndex,
		float timerStart,
		bool isActive,
		Vector2 buttonSize,
		Action onClick)
	{
		string waveName = $"Wave {waveIndex + 1}";
		(byte backgroundAlpha, byte textAlpha) = PracticeWindow.GetAlpha(isActive);
		Color color = waveIndex % 3 == 2 ? EnemiesV3_2.Ghostpede.Color.ToEngineColor() : EnemiesV3_2.Gigapede.Color.ToEngineColor();

		ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
		if (ImGui.BeginChild(waveName, buttonSize, true))
		{
			bool hover = ImGui.IsWindowHovered();
			ImGui.PushStyleColor(ImGuiCol.ChildBg, color with { A = (byte)(hover ? backgroundAlpha + 16 : backgroundAlpha) });

			if (ImGui.BeginChild(waveName + " child", buttonSize, false, ImGuiWindowFlags.NoInputs))
			{
				if (hover && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
					onClick.Invoke();

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
