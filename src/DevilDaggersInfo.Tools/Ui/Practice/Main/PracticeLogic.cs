using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.Engine.Maths;
using DevilDaggersInfo.Tools.Ui.Popups;
using DevilDaggersInfo.Tools.Ui.Practice.Main.Data;
using DevilDaggersInfo.Tools.User.Settings;
using DevilDaggersInfo.Tools.User.Settings.Model;
using Serilog;

namespace DevilDaggersInfo.Tools.Ui.Practice.Main;

public static class PracticeLogic
{
	public const int SpawnVersion = 6;

#pragma warning disable SA1401, S1104, S2223, CA2211
	public static PracticeState State = PracticeState.Default;
#pragma warning restore CA2211, S2223, S1104, SA1401

	public static void GenerateAndApplyPracticeSpawnset()
	{
		State.TimerStart = Math.Clamp(State.TimerStart, 0, 1400);

		SpawnsetBinary spawnset = ContentManager.Content.DefaultSpawnset;
		float shrinkStart = MathUtils.Lerp(spawnset.ShrinkStart, spawnset.ShrinkEnd, State.TimerStart / ((spawnset.ShrinkStart - spawnset.ShrinkEnd) / spawnset.ShrinkRate));

		SpawnsetBinary generatedSpawnset = spawnset.GetWithHardcodedEndLoop(70).GetWithTrimmedStart(State.TimerStart) with
		{
			HandLevel = State.HandLevel,
			AdditionalGems = State.AdditionalGems,
			TimerStart = State.TimerStart,
			SpawnVersion = SpawnVersion,
			ShrinkStart = shrinkStart,
		};

		try
		{
			File.WriteAllBytes(UserSettings.ModsSurvivalPath, generatedSpawnset.ToBytes());

			const string messageText = """
				The practice spawnset has been applied!

				Make sure to press the restart button in-game, or grab the dagger in the lobby, to apply the change.

				There is no need to restart the game itself.
				""";

			PopupManager.ShowMessageWithHideOption(
				"Practice spawnset applied",
				messageText,
				UserSettings.Model.DoNotShowAgainPracticeSpawnsetApplied,
				b => UserSettings.Model = UserSettings.Model with { DoNotShowAgainPracticeSpawnsetApplied = b });
		}
		catch (IOException ex)
		{
			const string message = "Could not write to the mods/survival file, probably because Devil Daggers is currently reading the file. Please try again.";
			Log.Error(ex, message);
			PopupManager.ShowError(message, ex);
		}
	}

	public static void DeleteModdedSpawnset()
	{
		if (File.Exists(UserSettings.ModsSurvivalPath))
			File.Delete(UserSettings.ModsSurvivalPath);

		const string messageText = """
			The default game has been restored!

			Make sure to press the restart button in-game, or grab the dagger in the lobby, to apply the change.

			There is no need to restart the game itself.
			""";

		PopupManager.ShowMessageWithHideOption(
			"Default game restored",
			messageText,
			UserSettings.Model.DoNotShowAgainPracticeSpawnsetDeleted,
			b => UserSettings.Model = UserSettings.Model with { DoNotShowAgainPracticeSpawnsetDeleted = b });
	}

	public static bool IsActive(UserSettingsPracticeTemplate customTemplate)
	{
		return IsActive(customTemplate.HandLevel, customTemplate.AdditionalGems, customTemplate.TimerStart);
	}

	public static bool IsActive(NoFarmTemplate noFarmTemplate)
	{
		return IsActive(noFarmTemplate.HandLevel, noFarmTemplate.AdditionalGems, noFarmTemplate.TimerStart);
	}

	public static bool IsActive(HandLevel handLevel, int additionalGems, float timerStart)
	{
		if (!SurvivalFileWatcher.Exists)
			return false;

		return handLevel == SurvivalFileWatcher.HandLevel && additionalGems == SurvivalFileWatcher.AdditionalGems && Math.Abs(timerStart - SurvivalFileWatcher.TimerStart) < PracticeDataConstants.TimerStartTolerance;
	}
}
