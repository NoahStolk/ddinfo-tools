using DevilDaggersInfo.App.Engine.Maths.Numerics;
using DevilDaggersInfo.Core.Spawnset;

namespace DevilDaggersInfo.App.Ui.Practice.Main.Data;

public readonly record struct NoFarmTemplate(string Name, Color Color, HandLevel HandLevel, int AdditionalGems, float TimerStart)
{
	public bool IsEqual(PracticeState practiceState)
	{
		return HandLevel == practiceState.HandLevel && AdditionalGems == practiceState.AdditionalGems && Math.Abs(TimerStart - practiceState.TimerStart) < PracticeDataConstants.TimerStartTolerance;
	}
}
