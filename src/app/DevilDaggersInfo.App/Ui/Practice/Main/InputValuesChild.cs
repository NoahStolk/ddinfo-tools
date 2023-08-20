using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Extensions;
using DevilDaggersInfo.App.User.Settings;
using DevilDaggersInfo.App.User.Settings.Model;
using DevilDaggersInfo.App.Utils;
using DevilDaggersInfo.App.ZeroAllocation;
using DevilDaggersInfo.Core.Spawnset;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui.Practice.Main;

public static class InputValuesChild
{
	private static readonly IdBuffer _handLevelIdBuffer = new(16);

	public static void Render()
	{
		ImGui.BeginChild("Input values", new(400, 160), true);

		ImGui.Spacing();
		ImGui.Image((IntPtr)Root.InternalResources.IconHandTexture.Handle, new(16), Vector2.Zero, Vector2.One, PracticeLogic.State.HandLevel.GetColor());
		ImGui.SameLine();
		foreach (HandLevel level in EnumUtils.HandLevels)
		{
			_handLevelIdBuffer.Overwrite("Lvl ", (int)level);
			if (ImGui.RadioButton(_handLevelIdBuffer, level == PracticeLogic.State.HandLevel) && PracticeLogic.State.HandLevel != level)
				PracticeLogic.State.HandLevel = level;

			if (level != HandLevel.Level4)
				ImGui.SameLine();
		}

		(Texture gemOrHomingTexture, Color tintColor) = PracticeLogic.State.HandLevel is HandLevel.Level3 or HandLevel.Level4 ? (Root.GameResources.IconMaskHomingTexture, Color.White) : (Root.GameResources.IconMaskGemTexture, Color.Red);
		ImGui.Spacing();
		ImGui.Image((IntPtr)gemOrHomingTexture.Handle, new(16), Vector2.UnitY, Vector2.UnitX, tintColor);
		ImGui.SameLine();
		ImGui.InputInt("Added gems", ref PracticeLogic.State.AdditionalGems, 1);

		ImGui.Spacing();
		ImGui.Image((IntPtr)Root.GameResources.IconMaskStopwatchTexture.Handle, new(16), Vector2.UnitY, Vector2.UnitX);
		ImGui.SameLine();
		ImGui.InputFloat("Timer start", ref PracticeLogic.State.TimerStart, 1, 5, "%.4f");

		for (int i = 0; i < 8; i++)
			ImGui.Spacing();

		if (ImGui.Button("Apply", new(80, 30)))
			PracticeLogic.GenerateAndApplyPracticeSpawnset();

		ImGui.SameLine();
		if (ImGui.Button("Save", new(80, 30)))
		{
			UserSettingsModel.UserSettingsPracticeTemplate newTemplate = new(null, PracticeLogic.State.HandLevel, PracticeLogic.State.AdditionalGems, PracticeLogic.State.TimerStart);
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

		ImGui.EndChild();
	}
}
