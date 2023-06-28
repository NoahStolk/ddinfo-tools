using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.Common;
using ImGuiNET;

namespace DevilDaggersInfo.App.Ui.Practice.Main;

public static class CurrentSpawnsetChild
{
	public static void Render()
	{
		ImGui.BeginChild("Current spawnset", new(400, 192), true);

		ImGui.BeginChild("Current practice values", new(400, 64));
		if (SurvivalFileWatcher.Exists)
		{
			string timerStart = SurvivalFileWatcher.TimerStart.ToString(StringFormats.TimeFormat);

			if (ImGui.BeginChild("Current practice values left", new(160, 64)))
			{
				ImGui.TextColored(Color.Yellow, "Effective values");

				if (SurvivalFileWatcher.EffectivePlayerSettings.HandLevel != SurvivalFileWatcher.EffectivePlayerSettings.HandMesh)
					ImGui.Text($"{SurvivalFileWatcher.EffectivePlayerSettings.HandLevel} ({SurvivalFileWatcher.EffectivePlayerSettings.HandMesh} mesh)");
				else
					ImGui.Text(SurvivalFileWatcher.EffectivePlayerSettings.HandLevel.ToString());

				ImGui.Text(SurvivalFileWatcher.EffectivePlayerSettings.GemsOrHoming.ToString());
				ImGui.Text(timerStart);
			}

			ImGui.EndChild();

			ImGui.SameLine();

			if (ImGui.BeginChild("Current practice values right", new(160, 64)))
			{
				ImGui.TextColored(Color.Yellow, "Spawnset values");

				ImGui.Text(SurvivalFileWatcher.HandLevel.ToString());
				ImGui.Text(SurvivalFileWatcher.AdditionalGems.ToString());
				ImGui.Text(timerStart);
			}

			ImGui.EndChild();
		}
		else
		{
			ImGui.Text("<No spawnset enabled>");
		}

		ImGui.EndChild();

		ImGui.BeginDisabled(!SurvivalFileWatcher.Exists);
		if (ImGui.Button("Delete spawnset (restore default)", new(0, 30)))
			PracticeLogic.DeleteModdedSpawnset();

		ImGui.EndDisabled();

		ImGui.EndChild();
	}
}
