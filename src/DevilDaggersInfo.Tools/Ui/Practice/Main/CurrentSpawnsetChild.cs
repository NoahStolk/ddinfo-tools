using DevilDaggersInfo.Core.Common;
using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;

namespace DevilDaggersInfo.Tools.Ui.Practice.Main;

public static class CurrentSpawnsetChild
{
	public static void Render()
	{
		if (ImGui.BeginChild("CurrentSpawnset", new(0, 200), ImGuiChildFlags.Border))
		{
			ImGui.SeparatorText("Current practice configuration");
			ImGui.Spacing();

			if (SurvivalFileWatcher.Exists)
			{
				ImGui.PushFont(Root.FontGoetheBold20);
				ImGui.Text("Practice is enabled!");
				ImGui.PopFont();
				ImGui.Spacing();
				ImGui.Text("Press the restart button in the game to start practicing.");
				ImGui.Spacing();
				ImGui.Text("The current practice spawnset is enabled:");
				ImGui.Spacing();

				if (ImGui.BeginTable("CurrentSpawnsetTable", 2, ImGuiTableFlags.Borders, new(320, 0)))
				{
					ImGui.TableNextColumn();
					ImGui.Text("Hand");

					ImGui.TableNextColumn();
					Color handColor = SurvivalFileWatcher.EffectivePlayerSettings.HandLevel.GetColor();
					if (SurvivalFileWatcher.EffectivePlayerSettings.HandLevel != SurvivalFileWatcher.EffectivePlayerSettings.HandMesh)
						ImGui.TextColored(handColor, Inline.Span($"{EnumUtils.HandLevelNames[SurvivalFileWatcher.EffectivePlayerSettings.HandLevel]} ({EnumUtils.HandLevelNames[SurvivalFileWatcher.EffectivePlayerSettings.HandMesh]} hand mesh)"));
					else
						ImGui.TextColored(handColor, EnumUtils.HandLevelNames[SurvivalFileWatcher.EffectivePlayerSettings.HandLevel]);

					ImGui.TableNextColumn();
					ImGui.Text("Gems/Homing");

					ImGui.TableNextColumn();
					if (SurvivalFileWatcher.EffectivePlayerSettings.HandLevel is HandLevel.Level3 or HandLevel.Level4)
						ImGui.TextColored(handColor, Inline.Span($"{SurvivalFileWatcher.EffectivePlayerSettings.GemsOrHoming} homing"));
					else
						ImGui.TextColored(Color.Red, Inline.Span($"{SurvivalFileWatcher.EffectivePlayerSettings.GemsOrHoming} gems"));

					ImGui.TableNextColumn();
					ImGui.Text("Starts at");

					ImGui.TableNextColumn();
					ImGui.Text(Inline.Span(SurvivalFileWatcher.TimerStart, StringFormats.TimeFormat));

					ImGui.EndTable();
				}
			}
			else
			{
				ImGui.PushFont(Root.FontGoetheBold20);
				ImGui.Text("Practice is disabled.");
				ImGui.PopFont();
				ImGui.Text("Click on a template to enable practice, then press the restart button in the game.");
			}
		}

		ImGui.EndChild(); // End CurrentSpawnset
	}
}
