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

public sealed class InputValuesChild
{
	private readonly ResourceManager _resourceManager;

	public InputValuesChild(ResourceManager resourceManager)
	{
		_resourceManager = resourceManager;
	}

	public void Render()
	{
		Debug.Assert(_resourceManager.GameResources != null, $"{nameof(_resourceManager.GameResources)} is null, which should never happen in this UI.");

		if (ImGui.BeginChild("InputValues", new Vector2(380, 200), ImGuiChildFlags.Border))
		{
			ImGui.SeparatorText("Inputs");

			ImGui.Spacing();
			ImGuiImage.Image(_resourceManager.InternalResources.IconHandTexture.Id, new Vector2(16), PracticeLogic.State.HandLevel.GetColor());
			ImGui.SameLine();
			foreach (HandLevel level in EnumUtils.HandLevels)
			{
				if (ImGui.RadioButton(Inline.Span($"Lvl {(int)level}"), level == PracticeLogic.State.HandLevel) && PracticeLogic.State.HandLevel != level)
					PracticeLogic.State.HandLevel = level;

				if (level != HandLevel.Level4)
					ImGui.SameLine();
			}

			(Texture gemOrHomingTexture, Color tintColor) = PracticeLogic.State.HandLevel is HandLevel.Level3 or HandLevel.Level4 ? (_resourceManager.GameResources.IconMaskHomingTexture, Color.White) : (_resourceManager.GameResources.IconMaskGemTexture, Color.Red);
			ImGui.Spacing();
			ImGuiImage.Image(gemOrHomingTexture.Id, new Vector2(16), tintColor);
			ImGui.SameLine();
			ImGui.InputInt("Added gems", ref PracticeLogic.State.AdditionalGems, 1);

			ImGui.Spacing();
			ImGuiImage.Image(_resourceManager.GameResources.IconMaskStopwatchTexture.Id, new Vector2(16));
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
