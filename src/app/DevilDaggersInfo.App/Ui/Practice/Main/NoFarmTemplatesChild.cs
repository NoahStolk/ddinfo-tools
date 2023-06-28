using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Extensions;
using DevilDaggersInfo.App.Ui.Practice.Main.Data;
using DevilDaggersInfo.Common;
using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Core.Wiki;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui.Practice.Main;

public static class NoFarmTemplatesChild
{
	private static readonly List<NoFarmTemplate> _noFarmTemplates = new()
	{
		new("First Spider I & Squid II", EnemiesV3_2.Squid2.Color.ToEngineColor(), HandLevel.Level1, 8, 39),
		new("First Centipede", EnemiesV3_2.Centipede.Color.ToEngineColor(), HandLevel.Level2, 25, 114),
		new("Centipede & first triple Spider Is", EnemiesV3_2.Spider1.Color.ToEngineColor(), HandLevel.Level3, 11, 174),
		new("Six Squid Is", EnemiesV3_2.Squid3.Color.ToEngineColor(), HandLevel.Level3, 57, 229),
		new("Gigapedes", EnemiesV3_2.Gigapede.Color.ToEngineColor(), HandLevel.Level3, 81, 259),
		new("Leviathan", EnemiesV3_2.Leviathan.Color.ToEngineColor(), HandLevel.Level4, 105, 350),
		new("Post Orb", EnemiesV3_2.TheOrb.Color.ToEngineColor(), HandLevel.Level4, 129, 397),
	};

	public static void Render()
	{
		ImGui.BeginChild("No farm templates", PracticeWindow.TemplateContainerSize, true);
		ImGui.Text("No farm templates");

		ImGui.BeginChild("No farm template description", PracticeWindow.TemplateListSize with { Y = PracticeWindow.TemplateDescriptionHeight });
		ImGui.PushTextWrapPos(ImGui.GetCursorPos().X + PracticeWindow.TemplateWidth);
		ImGui.Text("The amount of gems is based on how many gems you would have at that point, without farming, without losing gems, and without any homing usage.");
		ImGui.PopTextWrapPos();
		ImGui.EndChild();

		ImGui.BeginChild("No farm template list", PracticeWindow.TemplateListSize);
		foreach (NoFarmTemplate template in _noFarmTemplates)
		{
			Render(
				template,
				template.IsEqual(PracticeLogic.State),
				new(PracticeWindow.TemplateWidth, 48),
				() =>
				{
					PracticeLogic.State = new(template.HandLevel, template.AdditionalGems, template.TimerStart);
					PracticeLogic.Apply();
				});
		}

		ImGui.EndChild();
		ImGui.EndChild();
	}

	private static void Render(
		NoFarmTemplate noFarmTemplate,
		bool isActive,
		Vector2 buttonSize,
		Action onClick)
	{
		(byte backgroundAlpha, byte textAlpha) = PracticeWindow.GetAlpha(isActive);

		string timerText = noFarmTemplate.TimerStart.ToString(StringFormats.TimeFormat);

		(string gemsOrHomingText, Color gemColor) = PracticeWindow.GetGemsOrHomingText(noFarmTemplate.HandLevel, noFarmTemplate.AdditionalGems);

		ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
		if (ImGui.BeginChild(noFarmTemplate.Name, buttonSize, true))
		{
			bool hover = ImGui.IsWindowHovered();
			ImGui.PushStyleColor(ImGuiCol.ChildBg, noFarmTemplate.Color with { A = (byte)(hover ? backgroundAlpha + 16 : backgroundAlpha) });

			if (ImGui.BeginChild(noFarmTemplate.Name + " child", buttonSize, false, ImGuiWindowFlags.NoInputs))
			{
				if (hover && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
					onClick.Invoke();

				float windowWidth = ImGui.GetWindowWidth();

				ImGui.SetCursorPos(ImGui.GetCursorPos() + new Vector2(8, 8));

				ImGui.TextColored(noFarmTemplate.Color with { A = textAlpha }, noFarmTemplate.Name);
				ImGui.SameLine(windowWidth - ImGui.CalcTextSize(timerText).X - 8);
				ImGui.TextColored(Color.White with { A = textAlpha }, timerText);

				ImGui.SetCursorPos(ImGui.GetCursorPos() + new Vector2(8, 0));

				ImGui.TextColored(noFarmTemplate.HandLevel.GetColor() with { A = textAlpha }, noFarmTemplate.HandLevel.ToString());
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
