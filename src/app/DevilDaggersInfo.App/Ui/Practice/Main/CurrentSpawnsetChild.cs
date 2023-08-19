using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.Common;
using ImGuiNET;

namespace DevilDaggersInfo.App.Ui.Practice.Main;

public static class CurrentSpawnsetChild
{
	private static readonly char[] _handLevelWithMeshBuffer = new char[128];

	public static void Render()
	{
		ImGui.BeginChild("Current spawnset", new(400, 160), true);

		ImGui.BeginChild("Current practice values", new(400, 64));
		if (SurvivalFileWatcher.Exists)
		{
			if (ImGui.BeginChild("Current practice values left", new(160, 64)))
			{
				ImGui.TextColored(Color.Yellow, "Effective values");

				if (SurvivalFileWatcher.EffectivePlayerSettings.HandLevel != SurvivalFileWatcher.EffectivePlayerSettings.HandMesh)
				{
					UnsafeCharBufferWriter writer = new(_handLevelWithMeshBuffer);
					writer.Write(EnumUtils.HandLevelNames[SurvivalFileWatcher.EffectivePlayerSettings.HandLevel]);
					writer.Write(" (");
					writer.Write(EnumUtils.HandLevelNames[SurvivalFileWatcher.EffectivePlayerSettings.HandMesh]);
					writer.Write(" mesh)");
					ImGui.Text(writer);
				}
				else
				{
					ImGui.Text(EnumUtils.HandLevelNames[SurvivalFileWatcher.EffectivePlayerSettings.HandLevel]);
				}

				ImGui.Text(UnsafeSpan.Get(SurvivalFileWatcher.EffectivePlayerSettings.GemsOrHoming));
				ImGui.Text(UnsafeSpan.Get(SurvivalFileWatcher.TimerStart, StringFormats.TimeFormat));
			}

			ImGui.EndChild();

			ImGui.SameLine();

			if (ImGui.BeginChild("Current practice values right", new(160, 64)))
			{
				ImGui.TextColored(Color.Yellow, "Spawnset values");

				ImGui.Text(EnumUtils.HandLevelNames[SurvivalFileWatcher.HandLevel]);
				ImGui.Text(UnsafeSpan.Get(SurvivalFileWatcher.AdditionalGems));
				ImGui.Text(UnsafeSpan.Get(SurvivalFileWatcher.TimerStart, StringFormats.TimeFormat));
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
