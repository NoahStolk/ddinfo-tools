using DevilDaggersInfo.Core.Spawnset;
using System.Runtime.InteropServices;

namespace DevilDaggersInfo.Tools.Ui.Practice.Main.Data;

[StructLayout(LayoutKind.Sequential)]
public record struct PracticeState
{
	public HandLevel HandLevel;
	public int AdditionalGems;
	public float TimerStart;

	public PracticeState(HandLevel handLevel, int additionalGems, float timerStart)
	{
		HandLevel = handLevel;
		AdditionalGems = additionalGems;
		TimerStart = timerStart;
	}

	public static PracticeState Default => new(HandLevel.Level1, 0, 0);
}
