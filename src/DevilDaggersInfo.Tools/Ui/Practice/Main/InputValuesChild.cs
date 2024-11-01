using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.User.Settings;
using DevilDaggersInfo.Tools.User.Settings.Model;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.Practice.Main;

public static class InputValuesChild
{
	public static void Render()
	{
		if (ImGui.BeginChild("InputValues", new Vector2(400, 200), ImGuiChildFlags.Border))
		{
			ImGui.SeparatorText("Inputs");

			ImGui.Spacing();
			ImGuiImage.Image(Root.InternalResources.IconHandTexture.Id, new Vector2(16), PracticeLogic.State.HandLevel.GetColor());
			ImGui.SameLine();
			foreach (HandLevel level in EnumUtils.HandLevels)
			{
				if (ImGui.RadioButton(Inline.Span($"Lvl {(int)level}"), level == PracticeLogic.State.HandLevel) && PracticeLogic.State.HandLevel != level)
					PracticeLogic.State.HandLevel = level;

				if (level != HandLevel.Level4)
					ImGui.SameLine();
			}

			(Texture gemOrHomingTexture, Color tintColor) = PracticeLogic.State.HandLevel is HandLevel.Level3 or HandLevel.Level4 ? (Root.GameResources.IconMaskHomingTexture, Color.White) : (Root.GameResources.IconMaskGemTexture, Color.Red);
			ImGui.Spacing();
			ImGuiImage.Image(gemOrHomingTexture.Id, new Vector2(16), tintColor);
			ImGui.SameLine();
			ImGui.InputInt((PracticeLogic.State.HandLevel is HandLevel.Level2)? Inline.Span($"Added gems({PracticeLogic.State.AdditionalGems+10})") : "Added gems", ref PracticeLogic.State.AdditionalGems, 1);

			ImGui.Spacing();
			ImGuiImage.Image(Root.GameResources.IconMaskStopwatchTexture.Id, new Vector2(16));
			ImGui.SameLine();
			ImGui.InputFloat("Timer start", ref PracticeLogic.State.TimerStart, 1, 5, "%.4f");

			ImGui.Spacing();

			const float buttonHeight = 30;
			if (ImGui.Button("Apply inputs", new Vector2(0, buttonHeight)))
				PracticeLogic.GenerateAndApplyPracticeSpawnset();

			ImGui.SameLine();
			if (ImGui.Button("Save template", new Vector2(0, buttonHeight)))
			{
				UserSettingsPracticeTemplate newTemplate = new(null, PracticeLogic.State.HandLevel, PracticeLogic.State.AdditionalGems, PracticeLogic.State.TimerStart);
				if (!UserSettings.Model.PracticeTemplates.Contains(newTemplate))
				{
					UserSettings.Model = UserSettings.Model with
					{
						PracticeTemplates = UserSettings.Model.PracticeTemplates
							.Append(newTemplate)
							.ToList(),
					};
				}
			}

			if (ImGui.IsItemHovered())
				ImGui.SetTooltip("Save the current configuration as a custom template. Custom templates can be accessed on the right side of the window.");

			ImGui.SameLine();
			ImGui.BeginDisabled(!SurvivalFileWatcher.Exists);
			if (ImGui.Button("Return to normal game", new Vector2(0, buttonHeight)))
				PracticeLogic.DeleteModdedSpawnset();

			if (ImGui.IsItemHovered())
				ImGui.SetTooltip("This will delete the modded spawnset and return you to the normal game.");

			ImGui.EndDisabled();
		}

		ImGui.EndChild();
	}
}
