using DevilDaggersInfo.Core.Common;
using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Extensions;
using Hexa.NET.ImGui;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.Practice.Main;

internal sealed class CurrentSpawnsetChild(FontService fontService, SurvivalFileWatcher survivalFileWatcher)
{
	public void Render()
	{
		if (ImGui.BeginChild("CurrentSpawnset", new Vector2(0, 200), ImGuiChildFlags.Borders))
		{
			ImGui.SeparatorText("Current practice configuration");
			ImGui.Spacing();

			if (survivalFileWatcher.Exists)
			{
				ImGuiExt.PushFont(fontService.GoetheBold20);
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
						ImGui.TextColored(handColor, Inline.Utf8($"{survivalFileWatcher.EffectivePlayerSettings.HandLevel.AsUtf8Span()} ({survivalFileWatcher.EffectivePlayerSettings.HandMesh.AsUtf8Span()} hand mesh)"));
					else
						ImGui.TextColored(handColor, survivalFileWatcher.EffectivePlayerSettings.HandLevel.AsUtf8Span());

					ImGui.TableNextColumn();
					ImGui.Text("Gems/Homing");

					ImGui.TableNextColumn();
					if (survivalFileWatcher.EffectivePlayerSettings.HandLevel is HandLevel.Level3 or HandLevel.Level4)
						ImGui.TextColored(handColor, Inline.Utf8($"{survivalFileWatcher.EffectivePlayerSettings.GemsOrHoming} homing"));
					else
						ImGui.TextColored(Color.Red, Inline.Utf8($"{survivalFileWatcher.EffectivePlayerSettings.GemsOrHoming} gems"));

					ImGui.TableNextColumn();
					ImGui.Text("Starts at");

					ImGui.TableNextColumn();
					ImGui.Text(Inline.Utf8(survivalFileWatcher.TimerStart, StringFormats.TimeFormat));

					ImGui.EndTable();
				}
			}
			else
			{
				ImGuiExt.PushFont(fontService.GoetheBold20);
				ImGui.Text("Practice is disabled.");
				ImGui.PopFont();
				ImGui.Text("Click on a template to enable practice, then press the restart button in the game.");
			}
		}

		ImGui.EndChild();
	}
}
