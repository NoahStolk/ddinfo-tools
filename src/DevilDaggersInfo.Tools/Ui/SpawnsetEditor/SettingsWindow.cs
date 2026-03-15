using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.EditorFileState;
using DevilDaggersInfo.Tools.Engine.Maths.Numerics;
using DevilDaggersInfo.Tools.Ui.SpawnsetEditor.Utils;
using DevilDaggersInfo.Tools.Utils;
using ImGuiNET;
using System.Diagnostics;
using System.Numerics;

namespace DevilDaggersInfo.Tools.Ui.SpawnsetEditor;

internal sealed class SettingsWindow(FileStates fileStates, SpawnsetSaver spawnsetSaver)
{
	private static void InfoTooltipWhenDisabled(bool disabled, string tooltipText)
	{
		if (disabled)
		{
			ImGui.EndDisabled();
			InfoTooltip(tooltipText);
			ImGui.BeginDisabled(disabled);
		}
	}

	private static void InfoTooltip(string tooltipText)
	{
		ImGui.SameLine();
		ImGui.TextColored(Color.Gray(0.7f), "(?)");
		if (ImGui.IsItemHovered())
			ImGui.SetTooltip(tooltipText);
	}

	public void Render()
	{
		if (ImGui.Begin("Settings"))
		{
			RenderFormat();
			RenderGameMode();
			RenderRaceDagger();
			RenderArena();
			RenderPractice();

			ImGui.Unindent();
		}

		ImGui.End();
	}

	private void RenderFormat()
	{
		ImGui.Text("Format");
		InfoTooltip("There is generally no reason to change the spawnset format,\nunless you want to play spawnsets in an old version of the game.\n\nThese options are mainly here for backwards compatibility.");
		ImGui.Separator();
		ImGui.Indent(8);

		ImGui.Text("World version");
		ImGui.SameLine();
		for (int i = 8; i < 10; i++)
		{
			if (ImGui.RadioButton(Inline.Span(i), i == fileStates.Spawnset.Object.WorldVersion) && fileStates.Spawnset.Object.WorldVersion != i)
			{
				fileStates.Spawnset.Update(fileStates.Spawnset.Object with { WorldVersion = i });
				spawnsetSaver.Save(SpawnsetEditType.Format);
			}

			if (i < 9)
				ImGui.SameLine();
		}

		ImGui.Text("Spawn version");
		ImGui.SameLine();
		for (int i = 4; i < 7; i++)
		{
			if (ImGui.RadioButton(Inline.Span(i), i == fileStates.Spawnset.Object.SpawnVersion) && fileStates.Spawnset.Object.SpawnVersion != i)
			{
				fileStates.Spawnset.Update(fileStates.Spawnset.Object with { SpawnVersion = i });
				spawnsetSaver.Save(SpawnsetEditType.Format);
			}

			if (i < 6)
				ImGui.SameLine();
		}

		ImGui.Text("Supported in game version:");

		SpawnsetSupportedGameVersion supportedGameVersion = fileStates.Spawnset.Object.GetSupportedGameVersion();
		ImGui.Text(EnumUtils.SpawnsetSupportedGameVersionNames[supportedGameVersion]);
	}

	private void RenderGameMode()
	{
		ImGui.Spacing();
		ImGui.Indent(-8);
		ImGui.Text("Game mode");
		ImGui.Separator();
		ImGui.Indent(8);

		foreach (GameMode gameMode in EnumUtils.GameModes)
		{
			ReadOnlySpan<char> displayGameMode = gameMode switch
			{
				GameMode.Survival => "Survival",
				GameMode.TimeAttack => "Time Attack",
				GameMode.Race => "Race",
				_ => throw new UnreachableException(),
			};

			if (ImGui.RadioButton(displayGameMode, gameMode == fileStates.Spawnset.Object.GameMode) && fileStates.Spawnset.Object.GameMode != gameMode)
			{
				fileStates.Spawnset.Update(fileStates.Spawnset.Object with { GameMode = gameMode });
				spawnsetSaver.Save(SpawnsetEditType.GameMode);
			}

			if (gameMode != GameMode.Race)
				ImGui.SameLine();
		}
	}

	private void RenderRaceDagger()
	{
		ImGui.BeginDisabled(fileStates.Spawnset.Object.GameMode != GameMode.Race);

		ImGui.Spacing();
		ImGui.Indent(-8);
		ImGui.Text("Race dagger");
		InfoTooltipWhenDisabled(fileStates.Spawnset.Object.GameMode != GameMode.Race, "Changing the race dagger values doesn't do anything useful if the game mode is not set to Race.\nSet the game mode to Race to enable these inputs.");
		ImGui.Separator();
		ImGui.Indent(8);

		Vector2 raceDaggerPosition = fileStates.Spawnset.Object.RaceDaggerPosition;
		ImGui.InputFloat2("Position", ref raceDaggerPosition, "%.2f", ImGuiInputTextFlags.CharsDecimal);
		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			fileStates.Spawnset.Update(fileStates.Spawnset.Object with { RaceDaggerPosition = raceDaggerPosition });
			spawnsetSaver.Save(SpawnsetEditType.RaceDagger);
		}

		ImGui.EndDisabled();
	}

	private void RenderArena()
	{
		ImGui.Spacing();
		ImGui.Indent(-8);
		ImGui.Text("Arena");
		ImGui.Separator();
		ImGui.Indent(8);

		float shrinkStart = fileStates.Spawnset.Object.ShrinkStart;
		if (ImGui.SliderFloat("Shrink start", ref shrinkStart, 0, 150, "%.1f"))
			fileStates.Spawnset.Update(fileStates.Spawnset.Object with { ShrinkStart = shrinkStart });
		if (ImGui.IsItemDeactivatedAfterEdit())
			spawnsetSaver.Save(SpawnsetEditType.ShrinkStart);

		float shrinkEnd = fileStates.Spawnset.Object.ShrinkEnd;
		if (ImGui.SliderFloat("Shrink end", ref shrinkEnd, 0, 150, "%.1f"))
			fileStates.Spawnset.Update(fileStates.Spawnset.Object with { ShrinkEnd = shrinkEnd });
		if (ImGui.IsItemDeactivatedAfterEdit())
			spawnsetSaver.Save(SpawnsetEditType.ShrinkEnd);

		float shrinkRate = fileStates.Spawnset.Object.ShrinkRate;
		if (ImGui.SliderFloat("Shrink rate", ref shrinkRate, 0, 10, "%.3f"))
			fileStates.Spawnset.Update(fileStates.Spawnset.Object with { ShrinkRate = shrinkRate });
		if (ImGui.IsItemDeactivatedAfterEdit())
			spawnsetSaver.Save(SpawnsetEditType.ShrinkRate);

		float brightness = fileStates.Spawnset.Object.Brightness;
		if (ImGui.SliderFloat("Brightness", ref brightness, 0, 500, "%.1f"))
			fileStates.Spawnset.Update(fileStates.Spawnset.Object with { Brightness = brightness });
		if (ImGui.IsItemDeactivatedAfterEdit())
			spawnsetSaver.Save(SpawnsetEditType.Brightness);
	}

	private void RenderPractice()
	{
		ImGui.BeginDisabled(fileStates.Spawnset.Object.SpawnVersion <= 4);

		ImGui.Spacing();
		ImGui.Indent(-8);
		ImGui.Text("Practice");
		InfoTooltipWhenDisabled(fileStates.Spawnset.Object.SpawnVersion <= 4, "Practice values are not supported in spawn version 4.");
		ImGui.Separator();
		ImGui.Indent(8);

		for (int i = 0; i < EnumUtils.HandLevels.Count; i++)
		{
			HandLevel level = EnumUtils.HandLevels[i];
			if (ImGui.RadioButton(Inline.Span($"Lvl {(int)level}"), level == fileStates.Spawnset.Object.HandLevel) && fileStates.Spawnset.Object.HandLevel != level)
			{
				fileStates.Spawnset.Update(fileStates.Spawnset.Object with { HandLevel = level });
				spawnsetSaver.Save(SpawnsetEditType.HandLevel);
			}

			if (level != HandLevel.Level4)
				ImGui.SameLine();
		}

		int additionalGems = fileStates.Spawnset.Object.AdditionalGems;
		ImGui.InputInt("Added gems", ref additionalGems, 1);
		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			fileStates.Spawnset.Update(fileStates.Spawnset.Object with { AdditionalGems = additionalGems });
			spawnsetSaver.Save(SpawnsetEditType.AdditionalGems);
		}

		ImGui.EndDisabled();

		ImGui.BeginDisabled(fileStates.Spawnset.Object.SpawnVersion <= 5);

		float timerStart = fileStates.Spawnset.Object.TimerStart;
		ImGui.InputFloat("Timer start", ref timerStart, 1, 5, "%.4f");
		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			fileStates.Spawnset.Update(fileStates.Spawnset.Object with { TimerStart = timerStart });
			spawnsetSaver.Save(SpawnsetEditType.TimerStart);
		}

		ImGui.EndDisabled();
	}
}
