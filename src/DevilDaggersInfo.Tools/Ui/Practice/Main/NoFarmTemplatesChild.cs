using DevilDaggersInfo.Core.Common;
using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Core.Wiki;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.Practice.Main.Data;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.Practice.Main;

public static class NoFarmTemplatesChild
{
	private static readonly List<NoFarmTemplate> _noFarmTemplates =
	[
		new("First Spider I & Squid II", EnemiesV3_2.Squid2.Color.ToEngineColor(), HandLevel.Level1, 8, 39),
		new("First Centipede", EnemiesV3_2.Centipede.Color.ToEngineColor(), HandLevel.Level2, 25, 114),
		new("Centipede & first triple Spider Is", EnemiesV3_2.Spider1.Color.ToEngineColor(), HandLevel.Level3, 11, 174),
		new("Six Squid Is", EnemiesV3_2.Squid3.Color.ToEngineColor(), HandLevel.Level3, 57, 229),
		new("Triple Gigapedes", EnemiesV3_2.Gigapede.Color.ToEngineColor(), HandLevel.Level3, 81, 259),
		new("Leviathan", EnemiesV3_2.Leviathan.Color.ToEngineColor(), HandLevel.Level4, 105, 350),
		new("Post Orb", EnemiesV3_2.TheOrb.Color.ToEngineColor(), HandLevel.Level4, 129, 397),
	];

	public static void Render()
	{
		if (ImGui.BeginChild("NoFarmTemplates", PracticeWindow.TemplateContainerSize, ImGuiChildFlags.Border))
		{
			ImGui.Text("No farm templates");

			if (ImGui.BeginChild("NoFarmTemplateDescription", PracticeWindow.TemplateListSize with { Y = PracticeWindow.TemplateDescriptionHeight }))
			{
				ImGui.PushTextWrapPos(ImGui.GetCursorPos().X + PracticeWindow.TemplateWidth);
				ImGui.Text("The amount of gems is based on how many gems you would have at that point, without farming, without losing gems, and without any homing usage.");
				ImGui.PopTextWrapPos();
			}

			ImGui.EndChild(); // End NoFarmTemplateDescription

			if (ImGui.BeginChild("NoFarmTemplateList", PracticeWindow.TemplateListSize))
			{
				for (int i = 0; i < _noFarmTemplates.Count; i++)
				{
					NoFarmTemplate template = _noFarmTemplates[i];
					RenderNoFarmTemplate(template);
				}
			}

			ImGui.EndChild(); // End NoFarmTemplateList
		}

		ImGui.EndChild(); // End NoFarmTemplates
	}

	private static void RenderNoFarmTemplate(NoFarmTemplate noFarmTemplate)
	{
		(byte backgroundAlpha, byte textAlpha) = PracticeWindow.GetAlpha(PracticeLogic.IsActive(noFarmTemplate));

		const int bufferLength = 32;
		Span<char> gemsOrHomingText = stackalloc char[bufferLength];
		PracticeWindow.GetGemsOrHomingText(noFarmTemplate.HandLevel, noFarmTemplate.AdditionalGems, gemsOrHomingText, out Color gemColor);
		gemsOrHomingText = gemsOrHomingText.SliceUntilNull(bufferLength);

		Vector2 buttonSize = new(PracticeWindow.TemplateWidth, 48);
		ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
		if (ImGui.BeginChild(noFarmTemplate.Name, buttonSize, ImGuiChildFlags.Border))
		{
			bool hover = ImGui.IsWindowHovered();
			ImGui.PushStyleColor(ImGuiCol.ChildBg, noFarmTemplate.Color with { A = (byte)(hover ? backgroundAlpha + 16 : backgroundAlpha) });

			if (ImGui.BeginChild(Inline.Span($"{noFarmTemplate.Name}Child"), buttonSize, ImGuiChildFlags.None, ImGuiWindowFlags.NoInputs))
			{
				if (hover && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
				{
					PracticeLogic.State = new(noFarmTemplate.HandLevel, noFarmTemplate.AdditionalGems, noFarmTemplate.TimerStart);
					PracticeLogic.GenerateAndApplyPracticeSpawnset();
				}

				float windowWidth = ImGui.GetWindowWidth();

				ImGui.SetCursorPos(ImGui.GetCursorPos() + new Vector2(8, 8));

				ImGui.TextColored(noFarmTemplate.Color with { A = textAlpha }, noFarmTemplate.Name);
				ImGui.SameLine(windowWidth - ImGui.CalcTextSize(Inline.Span(noFarmTemplate.TimerStart, StringFormats.TimeFormat)).X - 8);
				ImGui.TextColored(Color.White with { A = textAlpha }, Inline.Span(noFarmTemplate.TimerStart, StringFormats.TimeFormat));

				ImGui.SetCursorPos(ImGui.GetCursorPos() + new Vector2(8, 0));

				ImGui.TextColored(noFarmTemplate.HandLevel.GetColor() with { A = textAlpha }, EnumUtils.HandLevelNames[noFarmTemplate.HandLevel]);
				ImGui.SameLine(windowWidth - ImGui.CalcTextSize(gemsOrHomingText).X - 8);
				ImGui.TextColored(gemColor with { A = textAlpha }, gemsOrHomingText);
			}

			ImGui.EndChild(); // End {NoFarmTemplate.Name}Child

			ImGui.PopStyleColor();
		}

		ImGui.PopStyleVar();

		ImGui.EndChild(); // End {NoFarmTemplate.Name}
	}
}
