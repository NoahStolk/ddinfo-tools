using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Tools.Engine.Maths;
using DevilDaggersInfo.Tools.Ui.Practice.Main.Data;
using DevilDaggersInfo.Tools.User.Settings;
using DevilDaggersInfo.Tools.User.Settings.Model;

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
		File.WriteAllBytes(UserSettings.ModsSurvivalPath, generatedSpawnset.ToBytes());
	}

	public static void DeleteModdedSpawnset()
	{
		if (File.Exists(UserSettings.ModsSurvivalPath))
			File.Delete(UserSettings.ModsSurvivalPath);
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
		return handLevel == SurvivalFileWatcher.HandLevel && additionalGems == SurvivalFileWatcher.AdditionalGems && Math.Abs(timerStart - SurvivalFileWatcher.TimerStart) < PracticeDataConstants.TimerStartTolerance;
	}
}
