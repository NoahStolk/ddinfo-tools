using DevilDaggersInfo.Core.Spawnset;
using System.Runtime.InteropServices;

namespace DevilDaggersInfo.Tools.Ui.Practice.Main.Data;

[StructLayout(LayoutKind.Sequential)]
internal record struct PracticeState(HandLevel HandLevel, int AdditionalGems, float TimerStart)
{
	public HandLevel HandLevel = HandLevel;
	public int AdditionalGems = AdditionalGems;
	public float TimerStart = TimerStart;

	public static PracticeState Default => new(HandLevel.Level1, 0, 0);
}
