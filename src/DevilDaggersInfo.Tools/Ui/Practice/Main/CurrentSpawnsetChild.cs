using DevilDaggersInfo.Core.Common;
using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.Practice.Main;

internal sealed class CurrentSpawnsetChild(SurvivalFileWatcher survivalFileWatcher)
{
	public void Render()
	{
		if (ImGui.BeginChild("CurrentSpawnset", new Vector2(0, 200), ImGuiChildFlags.Border)) // TODO: Borders in ImGui update.
		{
			ImGui.SeparatorText("Current practice configuration");
			ImGui.Spacing();

			if (survivalFileWatcher.Exists)
			{
				ImGui.PushFont(Root.FontGoetheBold20);
				ImGui.Text("Practice is enabled!");
				ImGui.PopFont();
				ImGui.Spacing();
				ImGui.Text("Press the restart button in the game to start practicing.");
				ImGui.Spacing();
				ImGui.Text("The current practice spawnset is enabled:");
				ImGui.Spacing();

				if (ImGui.BeginTable("CurrentSpawnsetTable", 2, ImGuiTableFlags.Borders, new Vector2(320, 0)))
				{
					ImGui.TableNextColumn();
					ImGui.Text("Hand");

					ImGui.TableNextColumn();
					Color handColor = survivalFileWatcher.EffectivePlayerSettings.HandLevel.GetColor();
					if (survivalFileWatcher.EffectivePlayerSettings.HandLevel != survivalFileWatcher.EffectivePlayerSettings.HandMesh)
						ImGui.TextColored(handColor, Inline.Span($"{EnumUtils.HandLevelNames[survivalFileWatcher.EffectivePlayerSettings.HandLevel]} ({EnumUtils.HandLevelNames[survivalFileWatcher.EffectivePlayerSettings.HandMesh]} hand mesh)"));
					else
						ImGui.TextColored(handColor, EnumUtils.HandLevelNames[survivalFileWatcher.EffectivePlayerSettings.HandLevel]);

					ImGui.TableNextColumn();
					ImGui.Text("Gems/Homing");

					ImGui.TableNextColumn();
					if (survivalFileWatcher.EffectivePlayerSettings.HandLevel is HandLevel.Level3 or HandLevel.Level4)
						ImGui.TextColored(handColor, Inline.Span($"{survivalFileWatcher.EffectivePlayerSettings.GemsOrHoming} homing"));
					else
						ImGui.TextColored(Color.Red, Inline.Span($"{survivalFileWatcher.EffectivePlayerSettings.GemsOrHoming} gems"));

					ImGui.TableNextColumn();
					ImGui.Text("Starts at");

					ImGui.TableNextColumn();
					ImGui.Text(Inline.Span(survivalFileWatcher.TimerStart, StringFormats.TimeFormat));

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

		ImGui.EndChild();
	}
}
