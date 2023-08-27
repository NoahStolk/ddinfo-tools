namespace DevilDaggersInfo.App.Ui.ReplayEditor.Utils;

public static class TimeUtils
{
	public static float TickToTime(int tick, float startTime)
	{
		return startTime + tick / 60f;
	}

	public static int TimeToTick(float time, float startTime)
	{
		return (int)MathF.Round((time - startTime) * 60);
	}
}
