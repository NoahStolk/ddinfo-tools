namespace DevilDaggersInfo.Tools;

public sealed class FrameCounter
{
	private int _currentSecond;

	private int _currentCount;

	public int CountPerSecond { get; private set; }

	public float TotalTime { get; private set; }

	public float LastRenderDelta { get; private set; }

	public void Increment(float deltaTime)
	{
		LastRenderDelta = deltaTime;
		TotalTime += deltaTime;

		int currentSecond = DateTime.UtcNow.Second;
		if (currentSecond != _currentSecond)
		{
			_currentSecond = currentSecond;
			CountPerSecond = _currentCount;
			_currentCount = 0;
		}
		else
		{
			_currentCount++;
		}
	}
}
