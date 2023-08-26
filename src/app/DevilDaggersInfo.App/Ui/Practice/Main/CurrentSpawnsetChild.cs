using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Utils;
using DevilDaggersInfo.App.ZeroAllocation;
using DevilDaggersInfo.Common;
using ImGuiNET;

namespace DevilDaggersInfo.App.Ui.Practice.Main;

public static class CurrentSpawnsetChild
{
	public static void Render()
	{
		if (ImGui.BeginChild("CurrentSpawnset", new(400, 160), true))
		{
			if (ImGui.BeginChild("CurrentPracticeValues", new(400, 64)))
			{
				if (SurvivalFileWatcher.Exists)
				{
					if (ImGui.BeginChild("CurrentPracticeValuesLeft", new(160, 64)))
					{
						ImGui.TextColored(Color.Yellow, "Effective values");

						if (SurvivalFileWatcher.EffectivePlayerSettings.HandLevel != SurvivalFileWatcher.EffectivePlayerSettings.HandMesh)
							ImGui.Text(UnsafeSpan.Get($"{EnumUtils.HandLevelNames[SurvivalFileWatcher.EffectivePlayerSettings.HandLevel]} ({EnumUtils.HandLevelNames[SurvivalFileWatcher.EffectivePlayerSettings.HandMesh]} mesh)"));
						else
							ImGui.Text(EnumUtils.HandLevelNames[SurvivalFileWatcher.EffectivePlayerSettings.HandLevel]);

						ImGui.Text(UnsafeSpan.Get(SurvivalFileWatcher.EffectivePlayerSettings.GemsOrHoming));
						ImGui.Text(UnsafeSpan.Get(SurvivalFileWatcher.TimerStart, StringFormats.TimeFormat));
					}

					ImGui.EndChild(); // End CurrentPracticeValuesLeft

					ImGui.SameLine();

					if (ImGui.BeginChild("CurrentPracticeValuesRight", new(160, 64)))
					{
						ImGui.TextColored(Color.Yellow, "Spawnset values");

						ImGui.Text(EnumUtils.HandLevelNames[SurvivalFileWatcher.HandLevel]);
						ImGui.Text(UnsafeSpan.Get(SurvivalFileWatcher.AdditionalGems));
						ImGui.Text(UnsafeSpan.Get(SurvivalFileWatcher.TimerStart, StringFormats.TimeFormat));
					}

					ImGui.EndChild(); // End CurrentPracticeValuesRight
				}
				else
				{
					ImGui.Text("<No spawnset enabled>");
				}
			}

			ImGui.EndChild(); // End CurrentPracticeValues

			ImGui.BeginDisabled(!SurvivalFileWatcher.Exists);
			if (ImGui.Button("Delete spawnset (restore default)", new(0, 30)))
				PracticeLogic.DeleteModdedSpawnset();

			ImGui.EndDisabled();
		}

		ImGui.EndChild(); // End CurrentSpawnset
	}
}
