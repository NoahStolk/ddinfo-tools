using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.App.Ui.SpawnsetEditor.State;
using DevilDaggersInfo.App.Ui.SpawnsetEditor.Utils;
using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Core.Spawnset.Extensions;
using DevilDaggersInfo.Core.Wiki;
using DevilDaggersInfo.Core.Wiki.Extensions;
using ImGuiNET;
using System.Numerics;

namespace DevilDaggersInfo.App.Ui.SpawnsetEditor;

public static class SettingsChild
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

	public static void Render()
	{
		ImGui.BeginChild("SettingsChild", new(288, 416));

		RenderFormat();
		RenderGameMode();
		RenderRaceDagger();
		RenderArena();
		RenderPractice();

		ImGui.Unindent();
		ImGui.EndChild();
	}

	private static void RenderFormat()
	{
		ImGui.Text("Format");
		InfoTooltip("There is generally no reason to change the spawnset format,\nunless you want to play spawnsets in an old version of the game.\n\nThese options are mainly here for backwards compatibility.");
		ImGui.Separator();
		ImGui.Indent(8);

		ImGui.Text("World version");
		ImGui.SameLine();
		for (int i = 8; i < 10; i++)
		{
			if (ImGui.RadioButton(i.ToString(), i == SpawnsetState.Spawnset.WorldVersion) && SpawnsetState.Spawnset.WorldVersion != i)
			{
				SpawnsetState.Spawnset = SpawnsetState.Spawnset with { WorldVersion = i };
				SpawnsetHistoryUtils.Save(SpawnsetEditType.Format);
			}

			if (i < 9)
				ImGui.SameLine();
		}

		ImGui.Text("Spawn version");
		ImGui.SameLine();
		for (int i = 4; i < 7; i++)
		{
			if (ImGui.RadioButton(i.ToString(), i == SpawnsetState.Spawnset.SpawnVersion) && SpawnsetState.Spawnset.SpawnVersion != i)
			{
				SpawnsetState.Spawnset = SpawnsetState.Spawnset with { SpawnVersion = i };
				SpawnsetHistoryUtils.Save(SpawnsetEditType.Format);
			}

			if (i < 6)
				ImGui.SameLine();
		}

		ImGui.Text("Supported in game version:");

		SpawnsetSupportedGameVersion supportedGameVersion = SpawnsetState.Spawnset.GetSupportedGameVersion();
		ImGui.Text(supportedGameVersion.ToDisplayString());
	}

	private static void RenderGameMode()
	{
		ImGui.Spacing();
		ImGui.Indent(-8);
		ImGui.Text("Game mode");
		ImGui.Separator();
		ImGui.Indent(8);

		foreach (GameMode gameMode in Enum.GetValues<GameMode>())
		{
			if (ImGui.RadioButton(gameMode.ToString(), gameMode == SpawnsetState.Spawnset.GameMode) && SpawnsetState.Spawnset.GameMode != gameMode)
			{
				SpawnsetState.Spawnset = SpawnsetState.Spawnset with { GameMode = gameMode };
				SpawnsetHistoryUtils.Save(SpawnsetEditType.GameMode);
			}

			if (gameMode != GameMode.Race)
				ImGui.SameLine();
		}
	}

	private static void RenderRaceDagger()
	{
		ImGui.BeginDisabled(SpawnsetState.Spawnset.GameMode != GameMode.Race);

		ImGui.Spacing();
		ImGui.Indent(-8);
		ImGui.Text("Race dagger");
		InfoTooltipWhenDisabled(SpawnsetState.Spawnset.GameMode != GameMode.Race, "Race dagger values can only be set when the game mode is set to Race.");
		ImGui.Separator();
		ImGui.Indent(8);

		Vector2 raceDaggerPosition = SpawnsetState.Spawnset.RaceDaggerPosition;
		ImGui.InputFloat2("Position", ref raceDaggerPosition, "%.2f", ImGuiInputTextFlags.CharsDecimal);
		if (Math.Abs(SpawnsetState.Spawnset.RaceDaggerPosition.X - raceDaggerPosition.X) > 0.001f ||
		    Math.Abs(SpawnsetState.Spawnset.RaceDaggerPosition.Y - raceDaggerPosition.Y) > 0.001f)
		{
			SpawnsetState.Spawnset = SpawnsetState.Spawnset with { RaceDaggerPosition = raceDaggerPosition };
			SpawnsetHistoryUtils.Save(SpawnsetEditType.RaceDagger);
		}

		ImGui.EndDisabled();
	}

	private static void RenderArena()
	{
		ImGui.Spacing();
		ImGui.Indent(-8);
		ImGui.Text("Arena");
		ImGui.Separator();
		ImGui.Indent(8);

		float shrinkStart = SpawnsetState.Spawnset.ShrinkStart;
		ImGui.InputFloat("Shrink start", ref shrinkStart, 1, 5, "%.1f");
		if (Math.Abs(SpawnsetState.Spawnset.ShrinkStart - shrinkStart) > 0.001f)
		{
			SpawnsetState.Spawnset = SpawnsetState.Spawnset with { ShrinkStart = shrinkStart };
			SpawnsetHistoryUtils.Save(SpawnsetEditType.ShrinkStart);
		}

		float shrinkEnd = SpawnsetState.Spawnset.ShrinkEnd;
		ImGui.InputFloat("Shrink end", ref shrinkEnd, 1, 5, "%.1f");
		if (Math.Abs(SpawnsetState.Spawnset.ShrinkEnd - shrinkEnd) > 0.001f)
		{
			SpawnsetState.Spawnset = SpawnsetState.Spawnset with { ShrinkEnd = shrinkEnd };
			SpawnsetHistoryUtils.Save(SpawnsetEditType.ShrinkEnd);
		}

		float shrinkRate = SpawnsetState.Spawnset.ShrinkRate;
		ImGui.InputFloat("Shrink rate", ref shrinkRate, 0.005f, 0.5f, "%.3f");
		if (Math.Abs(SpawnsetState.Spawnset.ShrinkRate - shrinkRate) > 0.001f)
		{
			SpawnsetState.Spawnset = SpawnsetState.Spawnset with { ShrinkRate = shrinkRate };
			SpawnsetHistoryUtils.Save(SpawnsetEditType.ShrinkRate);
		}

		float brightness = SpawnsetState.Spawnset.Brightness;
		ImGui.InputFloat("Brightness", ref brightness, 5, 20, "%.1f");
		if (Math.Abs(SpawnsetState.Spawnset.Brightness - brightness) > 0.001f)
		{
			SpawnsetState.Spawnset = SpawnsetState.Spawnset with { Brightness = brightness };
			SpawnsetHistoryUtils.Save(SpawnsetEditType.Brightness);
		}
	}

	private static void RenderPractice()
	{
		ImGui.BeginDisabled(SpawnsetState.Spawnset.SpawnVersion <= 4);

		ImGui.Spacing();
		ImGui.Indent(-8);
		ImGui.Text("Practice");
		InfoTooltipWhenDisabled(SpawnsetState.Spawnset.SpawnVersion <= 4, "Practice values are not supported in spawn version 4.");
		ImGui.Separator();
		ImGui.Indent(8);

		foreach (HandLevel level in Enum.GetValues<HandLevel>())
		{
			if (ImGui.RadioButton($"Lvl {(int)level}", level == SpawnsetState.Spawnset.HandLevel) && SpawnsetState.Spawnset.HandLevel != level)
			{
				SpawnsetState.Spawnset = SpawnsetState.Spawnset with { HandLevel = level };
				SpawnsetHistoryUtils.Save(SpawnsetEditType.HandLevel);
			}

			if (level != HandLevel.Level4)
				ImGui.SameLine();
		}

		int additionalGems = SpawnsetState.Spawnset.AdditionalGems;
		ImGui.InputInt("Added gems", ref additionalGems, 1);
		if (SpawnsetState.Spawnset.AdditionalGems != additionalGems)
		{
			SpawnsetState.Spawnset = SpawnsetState.Spawnset with { AdditionalGems = additionalGems };
			SpawnsetHistoryUtils.Save(SpawnsetEditType.AdditionalGems);
		}

		ImGui.EndDisabled();

		ImGui.BeginDisabled(SpawnsetState.Spawnset.SpawnVersion <= 5);

		float timerStart = SpawnsetState.Spawnset.TimerStart;
		ImGui.InputFloat("Timer start", ref timerStart, 1, 5, "%.4f");
		if (Math.Abs(SpawnsetState.Spawnset.TimerStart - timerStart) > 0.001f)
		{
			SpawnsetState.Spawnset = SpawnsetState.Spawnset with { TimerStart = timerStart };
			SpawnsetHistoryUtils.Save(SpawnsetEditType.TimerStart);
		}

		ImGui.EndDisabled();
	}
}
