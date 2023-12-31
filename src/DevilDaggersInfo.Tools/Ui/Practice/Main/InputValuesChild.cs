using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Tools.User.Settings;
using DevilDaggersInfo.Tools.User.Settings.Model;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.Practice.Main;

public static class InputValuesChild
{
	public static void Render()
	{
		if (ImGui.BeginChild("InputValues", new(380, 200), ImGuiChildFlags.Border))
		{
			ImGui.SeparatorText("Inputs");

			ImGui.Spacing();
			ImGuiImage.Image(Root.InternalResources.IconHandTexture.Handle, new(16), PracticeLogic.State.HandLevel.GetColor());
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
			ImGuiImage.Image(gemOrHomingTexture.Handle, new(16), tintColor);
			ImGui.SameLine();
			ImGui.InputInt("Added gems", ref PracticeLogic.State.AdditionalGems, 1);

			ImGui.Spacing();
			ImGuiImage.Image(Root.GameResources.IconMaskStopwatchTexture.Handle, new(16));
			ImGui.SameLine();
			ImGui.InputFloat("Timer start", ref PracticeLogic.State.TimerStart, 1, 5, "%.4f");

			ImGui.Spacing();

			const float buttonHeight = 30;
			if (ImGui.Button("Apply inputs", new(0, buttonHeight)))
				PracticeLogic.GenerateAndApplyPracticeSpawnset();

			ImGui.SameLine();
			if (ImGui.Button("Save template", new(0, buttonHeight)))
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
			if (ImGui.Button("Return to normal game", new(0, buttonHeight)))
			{
				PracticeLogic.DeleteModdedSpawnset();
				PopupManager.ShowMessage("Default game restored", "The default game has been restored!\n\nMake sure to press the restart button in-game, or grab the dagger in the lobby, to apply the change.\n\nThere is no need to restart the game itself.");
			}

			if (ImGui.IsItemHovered())
				ImGui.SetTooltip("This will delete the modded spawnset and return you to the normal game.");

			ImGui.EndDisabled();
		}

		ImGui.EndChild(); // End InputValues
	}
}
