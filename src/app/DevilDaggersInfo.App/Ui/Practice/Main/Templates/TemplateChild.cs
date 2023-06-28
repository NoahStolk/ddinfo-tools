using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Extensions;
using DevilDaggersInfo.Common;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui.Practice.Main.Templates;

public static class TemplateChild
{
	public static void Render(
		PracticeWindow.Template template,
		bool isActive,
		Vector2 buttonSize,
		Action onClick)
	{
		(byte backgroundAlpha, byte textAlpha) = PracticeWindow.GetAlpha(isActive);

		string timerText = template.TimerStart.ToString(StringFormats.TimeFormat);

		(string gemsOrHomingText, Color gemColor) = PracticeWindow.GetGemsOrHomingText(template.HandLevel, template.AdditionalGems);

		ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
		if (ImGui.BeginChild(template.Name, buttonSize, true))
		{
			bool hover = ImGui.IsWindowHovered();
			ImGui.PushStyleColor(ImGuiCol.ChildBg, template.Color with { A = (byte)(hover ? backgroundAlpha + 16 : backgroundAlpha) });

			if (ImGui.BeginChild(template.Name + " child", buttonSize, false, ImGuiWindowFlags.NoInputs))
			{
				if (hover && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
					onClick.Invoke();

				float windowWidth = ImGui.GetWindowWidth();

				ImGui.SetCursorPos(ImGui.GetCursorPos() + new Vector2(8, 8));

				ImGui.TextColored(template.Color with { A = textAlpha }, template.Name);
				ImGui.SameLine(windowWidth - ImGui.CalcTextSize(timerText).X - 8);
				ImGui.TextColored(Color.White with { A = textAlpha }, timerText);

				ImGui.SetCursorPos(ImGui.GetCursorPos() + new Vector2(8, 0));

				ImGui.TextColored(template.HandLevel.GetColor() with { A = textAlpha }, template.HandLevel.ToString());
				ImGui.SameLine(windowWidth - ImGui.CalcTextSize(gemsOrHomingText).X - 8);
				ImGui.TextColored(gemColor with { A = textAlpha }, gemsOrHomingText);
			}

			ImGui.EndChild();

			ImGui.PopStyleColor();
		}

		ImGui.PopStyleVar();

		ImGui.EndChild();
	}
}
