using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.User.Settings;
using DevilDaggersInfo.Tools.User.Settings.Model;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using System.Diagnostics;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.Practice.Main;

internal sealed class InputValuesChild(ResourceManager resourceManager, PracticeLogic practiceLogic, UserSettings userSettings, SurvivalFileWatcher survivalFileWatcher)
{
	public void Render()
	{
		Debug.Assert(resourceManager.GameResources != null, $"{nameof(resourceManager.GameResources)} is null, which should never happen in this UI.");

		if (ImGui.BeginChild("InputValues", new Vector2(380, 200), ImGuiChildFlags.Border)) // TODO: Borders in ImGui update.
		{
			ImGui.SeparatorText("Inputs");

			ImGui.Spacing();
			ImGuiImage.Image(resourceManager.InternalResources.IconHandTexture.Id, new Vector2(16), practiceLogic.State.HandLevel.GetColor());
			ImGui.SameLine();
			foreach (HandLevel level in EnumUtils.HandLevels)
			{
				if (ImGui.RadioButton(Inline.Span($"Lvl {(int)level}"), level == practiceLogic.State.HandLevel) && practiceLogic.State.HandLevel != level)
					practiceLogic.State.HandLevel = level;

				if (level != HandLevel.Level4)
					ImGui.SameLine();
			}

			(Texture gemOrHomingTexture, Color tintColor) = practiceLogic.State.HandLevel is HandLevel.Level3 or HandLevel.Level4 ? (resourceManager.GameResources.IconMaskHomingTexture, Color.White) : (resourceManager.GameResources.IconMaskGemTexture, Color.Red);
			ImGui.Spacing();
			ImGuiImage.Image(gemOrHomingTexture.Id, new Vector2(16), tintColor);
			ImGui.SameLine();
			ImGui.InputInt("Added gems", ref practiceLogic.State.AdditionalGems, 1);

			ImGui.Spacing();
			ImGuiImage.Image(resourceManager.GameResources.IconMaskStopwatchTexture.Id, new Vector2(16));
			ImGui.SameLine();
			ImGui.InputFloat("Timer start", ref practiceLogic.State.TimerStart, 1, 5, "%.4f");

			ImGui.Spacing();

			const float buttonHeight = 30;
			if (ImGui.Button("Apply inputs", new Vector2(0, buttonHeight)))
				practiceLogic.GenerateAndApplyPracticeSpawnset();

			ImGui.SameLine();
			if (ImGui.Button("Save template", new Vector2(0, buttonHeight)))
			{
				UserSettingsPracticeTemplate newTemplate = new(null, practiceLogic.State.HandLevel, practiceLogic.State.AdditionalGems, practiceLogic.State.TimerStart);
				if (!userSettings.Model.PracticeTemplates.Contains(newTemplate))
				{
					userSettings.Model = userSettings.Model with
					{
						PracticeTemplates = userSettings.Model.PracticeTemplates
							.Append(newTemplate)
							.ToList(),
					};
				}
			}

			if (ImGui.IsItemHovered())
				ImGui.SetTooltip("Save the current configuration as a custom template. Custom templates can be accessed on the right side of the window.");

			ImGui.SameLine();
			ImGui.BeginDisabled(!survivalFileWatcher.Exists);
			if (ImGui.Button("Return to normal game", new Vector2(0, buttonHeight)))
				practiceLogic.DeleteModdedSpawnset();

			if (ImGui.IsItemHovered())
				ImGui.SetTooltip("This will delete the modded spawnset and return you to the normal game.");

			ImGui.EndDisabled();
		}

		ImGui.EndChild();
	}
}
